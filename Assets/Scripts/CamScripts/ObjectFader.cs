using UnityEngine;

public class ObjectFader : MonoBehaviour
{
	public Transform target;
	public LayerMask collidableLayermask;
	[Space]
	public GameObject currentObject;
	public GameObject previousObject;

	void Update()
	{
		CheckForCollision();
	}

	private void CheckForCollision()
	{
		if(target == null)
			target = GameObject.FindGameObjectWithTag("CameraTarget").transform;

		Debug.DrawLine(transform.position, target.position, Color.yellow);
		if(Physics.Linecast(transform.position, target.position, out RaycastHit hit, collidableLayermask))
		{
			currentObject = hit.transform.gameObject;
		}
		else
		{
			currentObject = null;
		}
		DisableObjectRendering(currentObject, previousObject);
		previousObject = currentObject;
	}

	private void DisableObjectRendering(GameObject objectToDisable, GameObject objectToEnable)
	{
		if(objectToEnable)
			SetMaterialOpaque(objectToEnable);

		if(objectToDisable)
			SetMaterialTransparent(objectToDisable);
	}

	private void SetMaterialTransparent(GameObject objectToFade)
	{
		if(objectToFade != null)
		{
			Material m = objectToFade.GetComponent<Renderer>().material;

			m.SetFloat("_Mode", 2);
			m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			m.SetInt("_ZWrite", 0);
			m.DisableKeyword("_ALPHATEST_ON");
			m.EnableKeyword("_ALPHABLEND_ON");
			m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			m.renderQueue = 3000;
		}
	}

	private void SetMaterialOpaque(GameObject objectToMakeOpqaue)
	{
		if(objectToMakeOpqaue != null)
		{
			Material m = objectToMakeOpqaue.GetComponent<Renderer>().material;

			m.SetFloat("_Mode", 0);
			m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
			m.SetInt("_ZWrite", 1);
			m.DisableKeyword("_ALPHATEST_ON");
			m.DisableKeyword("_ALPHABLEND_ON");
			m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			m.renderQueue = -1;
		}
	}
}
