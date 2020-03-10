using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class DeathZone : MonoBehaviour
{
	[SerializeField] private string tagToCheck = "";
	[SerializeField] private GameObject playerPrefab = null;
	[SerializeField] private Transform respawnSpot = null;
	[SerializeField] private Vector3 respawnZoneSize = new Vector3();
	[SerializeField] private ParticleSystem respawnParticles;

	private void Awake()
	{
		BoxCollider bc = gameObject.GetComponent<BoxCollider>();
		Rigidbody rb = gameObject.GetComponent<Rigidbody>();

		bc.isTrigger = true;
		bc.size = respawnZoneSize;

		rb.isKinematic = true;
		rb.useGravity = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag(tagToCheck))
		{
			RespawnPlayer(other);
		}
	}

	private void RespawnPlayer(Collider other)
	{
		Destroy(other.gameObject);
		Instantiate(playerPrefab, respawnSpot.position, Quaternion.identity);
		Instantiate(respawnParticles, respawnSpot.position, Quaternion.identity);
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, respawnZoneSize);
	}
#endif
}
