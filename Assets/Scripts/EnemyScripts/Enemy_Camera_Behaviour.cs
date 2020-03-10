using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Camera_Behaviour : MonoBehaviour
{
	public Transform target;
	public GameObject projectilePrefab;
	public float fireRate;
	public Transform muzzle1;
	public Transform muzzle2;
	public Light spotlight;
	public float timeToSpotPlayer;
	public float viewDistance;
	public string tagToCheck;
	public float rotationSpeed;
	public float timeUntilNextRotation;
	[Space]
	public Vector3 currentTarget;
	public Transform checkpointHolder;
	public Vector3[] checkpoints;
	public int startingCheckpoint = 0;

	private bool playerSpotted;
	private float fireRateStartValue;
	private float timeUntilNextRotationStartValue;
	private float viewAngle;
	private float playerVisibleTimer;
	private Vector3 playerCenterPos;
	private Color spotlightColor;
	private Vector3 origRot;
	private Vector3 curRot;
	private int targetCheckpointIndex;
	private bool nextCheckpoint;
	private Quaternion lookRotation;
	private Vector3 direction;

	private void Start()
	{
		GetCheckpoints();

		viewAngle = spotlight.spotAngle;
		spotlightColor = spotlight.color;

		origRot = transform.eulerAngles;

		target = FindObjectOfType<Player_Behaviour>().transform;
		fireRateStartValue = fireRate;
		timeUntilNextRotationStartValue = timeUntilNextRotation;
	}

	private void Update()
	{
		if(!target)
			target = GameObject.FindGameObjectWithTag("Player").transform;

		playerCenterPos = new Vector3(target.position.x, target.position.y + target.localScale.y, target.position.z);

		if(CanSeePlayer())
			playerVisibleTimer += Time.deltaTime;
		else
			playerVisibleTimer -= Time.deltaTime;

		playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
		spotlight.color = Color.Lerp(spotlightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);

		if(playerVisibleTimer >= timeToSpotPlayer)
			playerSpotted = true;
		else
			playerSpotted = false;

		if(playerSpotted && CanSeePlayer())
		{
			transform.LookAt(playerCenterPos);
			spotlight.color = Color.red;
			fireRate -= Time.deltaTime;

			if(fireRate <= Mathf.Epsilon)
			{
				StartCoroutine(AttackPlayer());
				fireRate = fireRateStartValue;
			}
		}
		else
		{
			timeUntilNextRotation -= Time.deltaTime;

			if(timeUntilNextRotation <= Mathf.Epsilon)
			{
				targetCheckpointIndex = (targetCheckpointIndex + 1) % checkpoints.Length;
				currentTarget = checkpoints[targetCheckpointIndex];

				timeUntilNextRotation = timeUntilNextRotationStartValue;
			}

			StartCoroutine(TurnToFaceNextCheckpoint());
		}
	}

	bool CanSeePlayer()
	{
		if(Vector3.Distance(transform.position, target.position) < viewDistance)
		{
			Vector3 dirToPlayer = (playerCenterPos - transform.position).normalized;
			float angleBetweenTurretAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
			if(angleBetweenTurretAndPlayer < viewAngle / 2f)
				if(Physics.Linecast(transform.position, playerCenterPos, out RaycastHit hit))
					if(hit.collider.CompareTag(tagToCheck))
						return true;
					else
						return false;
		}
		return false;
	}

	private IEnumerator AttackPlayer()
	{
		Instantiate(projectilePrefab, muzzle1.position, Quaternion.LookRotation(muzzle1.transform.forward));
		Instantiate(projectilePrefab, muzzle2.position, Quaternion.LookRotation(muzzle2.transform.forward));

		yield return null;
	}

	private void GetCheckpoints()
	{
		checkpoints = new Vector3[checkpointHolder.childCount];
		for(int i = 0; i < checkpoints.Length; i++)
		{
			checkpoints[i] = checkpointHolder.GetChild(i).position;
			checkpoints[i] = new Vector3(checkpoints[i].x, transform.position.y, checkpoints[i].z);
		}

		currentTarget = checkpoints[startingCheckpoint];
	}

	private IEnumerator TurnToFaceNextCheckpoint()
	{
		if(!playerSpotted)
		{
			//find the vector pointing from our position to the target
			direction = (currentTarget - transform.position).normalized;

			//create the rotation we need to be in to look at the target
			lookRotation = Quaternion.LookRotation(direction);

			//rotate us over time according to speed until we are in the required rotation
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
			yield return null;
		}
		yield return null;
	}

	private void ReturnToOriginalRotation()
	{
		curRot = Vector3.Lerp(transform.eulerAngles, origRot, rotationSpeed * Time.deltaTime);
		transform.eulerAngles = curRot;
	}

	private void OnDrawGizmos()
	{
		Vector3 startPosition = checkpointHolder.GetChild(0).position;
		Vector3 previousPosition = startPosition;
		foreach(Transform checkPoint in checkpointHolder)
		{
			Gizmos.DrawSphere(checkPoint.position, 0.3f);
			Gizmos.DrawLine(previousPosition, checkPoint.position);
			previousPosition = checkPoint.position;
		}
		Gizmos.DrawLine(previousPosition, startPosition);

		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
	}
}
