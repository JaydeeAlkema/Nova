using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float speed;
	public float damageToDeal;
	public float lineCastLength;
	public ParticleSystem hitParticles;

	private void Start()
	{
		Destroy(gameObject, 10f);
	}

	private void Update()
	{
		Move();
	}

	public void Move()
	{
		transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, speed * Time.deltaTime);

		if(Physics.Linecast(transform.position, transform.position + transform.forward * lineCastLength, out RaycastHit hit))
		{
			if(hit.collider.gameObject != this.gameObject && hit.collider.name != "GroundCheck")
			{
				if(hit.collider.CompareTag("Player"))
					hit.collider.gameObject.GetComponent<Player_Behaviour>().Damage(damageToDeal);
				else if(hit.collider.GetComponent<BreakableObject_Behaviour>() != null)
					hit.collider.GetComponent<BreakableObject_Behaviour>().Break();
				else if(hit.collider.CompareTag("EnemyGuard"))
					hit.collider.GetComponent<Enemy_Guard_Behaviour>().Damage(damageToDeal);

				Instantiate(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));
				Destroy(gameObject);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * lineCastLength);
	}
}
