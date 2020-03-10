using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectableType
{
	Generic,
	KeyCard
}

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public class Collectable : MonoBehaviour
{
	public CollectableType collectableType;
	public Gate gateToUnlock;
	[Space]
	public int collectableWorth;
	public bool pickupParticles;
	public ParticleSystem onPickupParticles;
	public AudioClip onPickupSound;
	[Space]
	public bool shouldRotate;
	public float rotationSpeed;
	public Vector3 rotationVector;
	[Space]
	public bool shouldBounce;
	public float bounceStrength;
	public float bounceSpeed;

	float startY;

	private void Start()
	{
		startY = transform.position.y;
	}

	private void Update()
	{
		if(shouldRotate)
			transform.Rotate(rotationVector * rotationSpeed * Time.deltaTime);
		if(shouldBounce)
			transform.position = new Vector3(transform.position.x, startY + ((float)Mathf.Sin(Time.time * bounceSpeed) * bounceStrength), transform.position.z);
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			AudioSource.PlayClipAtPoint(onPickupSound, transform.position);
			if(pickupParticles)
				Instantiate(onPickupParticles, transform.position, Quaternion.identity);
			switch(collectableType)
			{
				case CollectableType.Generic:
					FindObjectOfType<HUD_Behaviour>().score.Value += collectableWorth;
					break;
				case CollectableType.KeyCard:
					gateToUnlock.canBeUnlocked = true;
					break;
				default:
					break;
			}
			Destroy(gameObject);
		}
	}
}

