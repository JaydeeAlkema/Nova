using UnityEngine;

public class DebugUIElementsToggler : MonoBehaviour
{
	public GameObject[] UIElements;

	void Start()
	{
		if (Debug.isDebugBuild || Application.isEditor)
			ToggleDebugElements(true);
		else
			ToggleDebugElements(false);
	}

	void ToggleDebugElements(bool toggle)
	{

		for (int i = 0; i < UIElements.Length; i++)
		{
			UIElements[i].SetActive(toggle);
		}
	}
}
