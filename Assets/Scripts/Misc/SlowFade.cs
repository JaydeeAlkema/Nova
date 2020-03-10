using UnityEngine;

public class SlowFade : MonoBehaviour
{
	public float fadeSpeedMultiplier;

	float startSize;
	Vector3 targetSize;

	private void Start()
	{
		startSize = transform.localScale.x;
	}

	private void Update()
	{
		startSize -= Time.deltaTime * fadeSpeedMultiplier;
		targetSize = new Vector3(startSize, startSize, startSize);
		transform.localScale = targetSize;

		if(startSize <= Mathf.Epsilon)
		{
			Destroy(this.gameObject);
		}
	}
}
