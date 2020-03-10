using UnityEngine;

public class SmoothCameraRotater : MonoBehaviour
{
	public float rotationSpeed;
	public float rotationSmoothTime;

	public Vector3 startingRotation;

	Vector3 currentRotation;
	Vector3 rotationSmoothVelocity;

	float yaw;
	float pitch;

	void Update()
	{
		if(Time.timeScale >= 1f)
		{
			if(Input.GetMouseButton(1))
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				yaw += Input.GetAxisRaw("Mouse X") * rotationSpeed * Time.deltaTime;
				pitch -= Input.GetAxisRaw("Mouse Y") * rotationSpeed * Time.deltaTime;
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				yaw += Input.GetAxis("CameraHorizontal") * rotationSpeed * Time.deltaTime;
				pitch += Input.GetAxisRaw("CameraVertical") * rotationSpeed * Time.deltaTime;
			}

			pitch = Mathf.Clamp(pitch, -35f, 25f);
			currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(startingRotation.x + pitch, startingRotation.y + yaw, startingRotation.z), ref rotationSmoothVelocity, rotationSmoothTime);
			transform.eulerAngles = currentRotation;
		}
	}
}
