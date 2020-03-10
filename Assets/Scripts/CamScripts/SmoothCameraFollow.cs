using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
	public Transform followTarget;
	[Space]
	public float followSmoothSpeed;

	private void LateUpdate()
	{
		FollowTarget();
	}

	private void FollowTarget()
	{
		if(followTarget == null)
		{
			followTarget = FindObjectOfType<Player_Behaviour>().transform;
		}
		else
		{
			Vector3 desiredPosition = new Vector3(followTarget.position.x, followTarget.position.y, followTarget.position.z);
			Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, followSmoothSpeed * Time.deltaTime);

			transform.position = smoothedPosition;
		}
	}

}
