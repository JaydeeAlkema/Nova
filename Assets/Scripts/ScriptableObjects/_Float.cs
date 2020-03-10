using UnityEngine;

[CreateAssetMenu(fileName = "_Float", menuName = "ScriptableObjects/New _Float")]
public class _Float : ScriptableObject
{
	private int startingValue = 0;
	[SerializeField] private bool constant = false;
	[SerializeField] private float value;
	public float Value { get => value; set => this.value = value; }

	private void OnEnable()
	{
		if(!constant)
			value = startingValue;
	}
}