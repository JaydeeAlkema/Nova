using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy_Guard_Behaviour : MonoBehaviour
{
	public float health = 100;
	public GameObject brokenModelPrefab;
	[Space]
	public NavMeshAgent agent;
	public Light spotLight;
	public LayerMask viewMask;
	[Space]
	public GameObject projectilePrefab;
	public Transform muzzle_1;
	public Transform muzzle_2;
	public AudioClip projectileFiringFX;
	[Space]
	public float viewDistance;
	public float instantDetectionRadius;
	public float attackingRadius;
	[Space]
	#region Movement
	[Header("Movement Setup")]
	[Range(0, 15)] public float speed = 3f;
	[Range(0, 15)] public float chaseSpeed = 5f;
	[Range(0, 180)] public float turnSpeed = 90f;
	[Range(0, 1)] public float waitTime = 0.5f;
	[Range(0, 10)] public float chasingTime = 6;
	[Range(0, 1)] public float timeToSpotPlayer = 0.5f;
	[Space]
	#endregion
	public Vector3 currentTarget;
	public Transform pathHolder;
	public Vector3[] waypoints;
	public int startingWaypoint = 0;

	#region Private Variables
	private Transform player;
	private Color spotLightColor;
	private Animator anim;
	private Vector3 playerCenterPos;
	private AudioSource audioSource;
	private bool playerSpotted;
	private float viewAngle;
	private float playerVisibleTimer;
	private float chaseTime;
	private int targetWaypointIndex;
	#endregion

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		audioSource = GetComponent<AudioSource>();

		agent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();

		viewAngle = spotLight.spotAngle;
		spotLightColor = spotLight.color;

		SetupEnemy();
	}

	private void SetupEnemy()
	{
		GetWaypoints();

		transform.position = waypoints[startingWaypoint];
		targetWaypointIndex = startingWaypoint + 1;
		currentTarget = waypoints[targetWaypointIndex];

		agent.speed = speed;
		agent.SetDestination(currentTarget);

		StartCoroutine(FollowPath());
	}

	void Update()
	{
		if(!player)
			player = GameObject.FindGameObjectWithTag("Player").transform;

		playerCenterPos = new Vector3(player.position.x, player.position.y + player.localScale.y - 0.5f, player.position.z);

		if(CanSeePlayer())
			playerVisibleTimer += Time.deltaTime;
		else
			playerVisibleTimer -= Time.deltaTime;

		playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
		spotLight.color = Color.Lerp(spotLightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);

		if(playerVisibleTimer >= timeToSpotPlayer)
		{
			playerSpotted = true;
			chaseTime = chasingTime;
		}
		else
		{
			chaseTime -= Time.deltaTime;
			if(chaseTime <= Mathf.Epsilon)
				playerSpotted = false;
		}

		if(playerSpotted)
			spotLight.color = Color.red;
	}

	private void GetWaypoints()
	{
		waypoints = new Vector3[pathHolder.childCount];
		for(int i = 0; i < waypoints.Length; i++)
		{
			waypoints[i] = pathHolder.GetChild(i).position;
			waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
		}
	}

	bool CanSeePlayer()
	{
		if(Vector3.Distance(transform.position, playerCenterPos) < instantDetectionRadius)
			return true;

		if(Vector3.Distance(transform.position, playerCenterPos) < viewDistance)
		{
			Vector3 dirToPlayer = (playerCenterPos - transform.position).normalized;
			float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
			if(angleBetweenGuardAndPlayer < viewAngle / 2f)
				if(Physics.Linecast(transform.position, playerCenterPos, viewMask))
					return true;
		}
		return false;
	}

	public void Damage(float damageToDeal)
	{
		health -= damageToDeal;
		if(health <= 0)
		{
			Instantiate(brokenModelPrefab, transform.position, transform.rotation);
			Destroy(this.gameObject);
		}
	}

	private IEnumerator FollowPath()
	{
		while(true)
		{
			if(playerSpotted)
			{
				if(Vector3.Distance(transform.position, player.position) <= attackingRadius)
					yield return StartCoroutine(AttackPlayer(0.5f));
				else
					yield return StartCoroutine(FollowPlayer(0.1f));
			}
			else
			{
				StopCoroutine(AttackPlayer(0.5f));
				StopCoroutine(FollowPlayer(0.5f));
				if(transform.position == agent.pathEndPosition)
				{
					agent.speed = speed;
					anim.SetFloat("MovementSpeed", 1f);
					anim.SetBool("Attacking", false);
					targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
					currentTarget = waypoints[targetWaypointIndex];

					yield return new WaitForSeconds(waitTime);
					yield return StartCoroutine(TurnToFace(currentTarget));

					agent.SetDestination(currentTarget);
				}
			}
			yield return null;
		}
	}

	private IEnumerator FollowPlayer(float updateInterval)
	{
		if(playerSpotted)
		{
			agent.speed = chaseSpeed;
			anim.SetBool("Attacking", false);
			anim.SetFloat("MovementSpeed", 2f);
			agent.SetDestination(player.position);

			yield return new WaitForSeconds(updateInterval);
		}
		else
		{
			yield return null;
		}
	}

	private IEnumerator AttackPlayer(float attackInterval)
	{
		if(playerSpotted)
		{
			agent.speed = 0f;
			anim.SetBool("Attacking", true);
			transform.LookAt(playerCenterPos);
			Instantiate(projectilePrefab, muzzle_1.position, Quaternion.LookRotation(muzzle_1.transform.forward));
			Instantiate(projectilePrefab, muzzle_2.position, Quaternion.LookRotation(muzzle_2.transform.forward));
			audioSource.PlayOneShot(projectileFiringFX);

			yield return new WaitForSeconds(attackInterval);
		}
		else
		{
			yield return null;
		}
	}

	private IEnumerator TurnToFace(Vector3 lookTarget)
	{
		if(!playerSpotted)
		{
			Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
			float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

			while(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
			{
				float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
				transform.eulerAngles = Vector3.up * angle;

				yield return null;
			}
		}
		yield return null;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if(pathHolder != null)
		{
			Vector3 startPosition = pathHolder.GetChild(0).position;
			Vector3 previousPosition = startPosition;
			foreach(Transform waypoint in pathHolder)
			{
				Gizmos.DrawSphere(waypoint.position, 0.3f);
				Gizmos.DrawLine(previousPosition, waypoint.position);
				previousPosition = waypoint.position;
			}
			Gizmos.DrawLine(previousPosition, startPosition);

			Gizmos.color = Color.red;
			Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
			Gizmos.DrawWireSphere(transform.position + transform.forward, instantDetectionRadius);

			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, attackingRadius);
			if(playerSpotted)
			{
				Gizmos.color = Color.magenta;
				Gizmos.DrawLine(transform.position, playerCenterPos);
			}
		}
	}
#endif
}