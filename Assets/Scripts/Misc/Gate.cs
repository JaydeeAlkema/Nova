using UnityEngine;

public class Gate : MonoBehaviour
{
	public Transform itemToUnlockGate;
	public Animator anim;
	[Space]
	public Vector3 InteractionZonePosition;
	public LayerMask mask;
	public float InteractionZoneRadius;
	public bool canBeUnlocked = false;
	public bool isLocked = true;

	HUD_Behaviour hud;
	BoxCollider boxCollider;

	private void Start()
	{
		hud = FindObjectOfType<HUD_Behaviour>();
		boxCollider = GetComponent<BoxCollider>();
	}

	private void Update()
	{
		CheckForPlayerInteraction();
	}

	public void UnlockGate()
	{
		if(canBeUnlocked)
		{
			isLocked = false;
			boxCollider.enabled = false;
			anim.SetBool("IsLocked", isLocked);
		}
	}

	public void CheckForPlayerInteraction()
	{
		Collider[] collidersInRange = Physics.OverlapSphere(transform.position + InteractionZonePosition, InteractionZoneRadius, mask);

		if(collidersInRange.Length > 0)
		{
			hud.ToggleInteractionText(true);
			if(Input.GetButton("Interact"))
				UnlockGate();
		}
		else
		{
			hud.ToggleInteractionText(false);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if(itemToUnlockGate)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position + InteractionZonePosition, itemToUnlockGate.transform.position);
		}

		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position + InteractionZonePosition, InteractionZoneRadius);
	}
}
