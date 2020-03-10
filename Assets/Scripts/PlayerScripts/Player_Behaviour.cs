using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class Player_Behaviour : MonoBehaviour
{
	#region Character Stats Setup
	[Header("Character Stats Setup")]
	public float startingHealth;
	public float startingEnergy;
	public _Float health;
	public _Float energy;
	#endregion

	#region Character Movement Setup
	[Header("Character Movement Setup")]
	public Transform groundCheck;
	public float walkSpeed = 2f;
	public float runSpeed = 4f;
	public float jumpHeight = 1f;
	public float rotateSpeed = 5f;
	public float gravity = -12f;
	#endregion

	#region Character Shooting Setup
	[Header("Character Shooting Setup")]
	public float fireRate;
	public Transform firingPoint;
	public GameObject projectilePrefab;
	public ParticleSystem muzzleParticles;
	public AudioClip firingFX;
	#endregion

	#region Character Movement Smoothing
	[Header("Character Movement Smoothing")]
	public float turnSmoothTime = 0.2f;
	float turnSmoothVelocity;
	public float speedSmoothTime = 0.2f;
	float speedSmoothVelocity;
	public float jumpSmoothTime = 0.1f;
	#endregion

	float _currentSpeed;
	float _animationSpeedPercent;
	float _animationJumpPercent;
	float velocityY;
	float _fireRate;
	bool _running;
	bool _shooting;
	[HideInInspector] public bool actuallyGrounded;

	Vector2 input;
	Vector2 inputDir;
	Vector3 velocity;

	Animator anim;
	CharacterController controller;
	AudioSource audioSource;

	void Start()
	{
		health.Value = startingHealth;
		energy.Value = startingEnergy;

		anim = GetComponent<Animator>();
		controller = GetComponent<CharacterController>();
		audioSource = GetComponent<AudioSource>();

		_fireRate = fireRate;
	}

	void Update()
	{
		if(Time.timeScale >= 1f)
		{
			// Get Input from player.
			input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			inputDir = input.normalized;
			_running = Input.GetButton("Running");
			_shooting = Input.GetButton("Fire");


			// Move the character calling the function
			Move(inputDir, _running);

			// Jumping
			if(Input.GetButtonDown("Jump") && !_shooting)
				Jump();

			// Animate the Character
			anim.SetBool("Grounded", actuallyGrounded);

			anim.SetBool("Shooting", _shooting);

			_animationSpeedPercent = ((_running) ? .5f : 1) * inputDir.magnitude;
			anim.SetFloat("SpeedPercent", _animationSpeedPercent, speedSmoothTime, Time.deltaTime);

			_animationJumpPercent = Mathf.Clamp(velocityY, -1f, 1f);
			anim.SetFloat("VelocityY", _animationJumpPercent, jumpSmoothTime, Time.deltaTime);
		}
	}

	private void Shoot()
	{
		audioSource.PlayOneShot(firingFX);

		Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation);
		Instantiate(muzzleParticles, firingPoint.position, firingPoint.rotation);
	}

	void Move(Vector2 input, bool running)
	{
		// Rotate the Character.
		if(input != Vector2.zero || _shooting)
		{
			float targetRotation = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
				transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
		}

		// Change speed depending on Input.
		float targetSpeed = ((running) ? walkSpeed : runSpeed) * input.magnitude;
		float trueSpeed = (_shooting) ? 0f : targetSpeed;
		_currentSpeed = Mathf.SmoothDamp(_currentSpeed, trueSpeed, ref speedSmoothVelocity, speedSmoothTime);

		// Calculate Gravity
		velocityY += Time.deltaTime * gravity;
		velocity = transform.forward * _currentSpeed + Vector3.up * velocityY;

		// Make the Character move with Gravity applied.
		controller.Move(velocity * Time.deltaTime);

		// No need to apply Gravity when grounded.
		if(controller.isGrounded)
			velocityY = 0f;
	}

	void Jump()
	{
		if(controller.isGrounded)
		{
			// Apply force once and let the gravity handle the rest.
			float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
			velocityY = jumpVelocity;
		}
	}

	public void Damage(float damageAmount)
	{
		health.Value -= damageAmount;
		if(health.Value <= 0f)
			Respawn();
	}

	public void Respawn()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

}
