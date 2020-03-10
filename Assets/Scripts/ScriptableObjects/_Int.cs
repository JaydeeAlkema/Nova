using UnityEngine;

[CreateAssetMenu(fileName = "_Int", menuName = "ScriptableObjects/New _Int")]
public class _Int : ScriptableObject
{
	private int startingValue = 0;
	[SerializeField] private bool constant = false;
	[SerializeField] private int value;
	public int Value { get => value; set => this.value = value; }

	private void OnEnable()
	{
		if(!constant)
			value = startingValue;
	}
}

