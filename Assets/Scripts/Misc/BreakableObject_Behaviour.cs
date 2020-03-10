using UnityEngine;

public class BreakableObject_Behaviour : MonoBehaviour
{
	public string tagToCheck;
	public GameObject brokenGameobjectPrefab;

	public void Break()
	{
		Instantiate(brokenGameobjectPrefab, transform.position, transform.rotation);
		Destroy(this.gameObject);
	}
}
