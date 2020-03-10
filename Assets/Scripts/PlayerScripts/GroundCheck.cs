using UnityEngine;

public class GroundCheck : MonoBehaviour
{
	public Player_Behaviour playerController;
	public new SphereCollider collider;
	public LayerMask mask;

	private void Update()
	{
		Collider[] collisions = Physics.OverlapSphere(transform.position, collider.radius, mask);
		if(collisions.Length == 0)
			playerController.actuallyGrounded = false;
		else
			playerController.actuallyGrounded = true;
	}
}
