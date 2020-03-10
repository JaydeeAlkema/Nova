using UnityEngine.UI;
using UnityEngine;

public class HUD_Behaviour : MonoBehaviour
{
	public static HUD_Behaviour instance;

	#region Scoring
	[Header("Scoring")]
	public _Float timer;
	public _Int score;
	public TMPro.TextMeshProUGUI timerText;
	public TMPro.TextMeshProUGUI scoreText;
	#endregion

	#region Sliders (Health & Energy)
	[Header("Sliders")]
	public _Float health;
	public _Float energy;
	public Image healthSlider;
	public Image energySlider;
	#endregion

	#region Popup Texts
	[Header("Popup Texts")]
	public TMPro.TextMeshProUGUI gateInteractionText;
	#endregion

	private void Awake()
	{
		if(!instance)
			instance = this;
	}

	private void Start()
	{
		gateInteractionText.enabled = false;
	}

	private void Update()
	{
		timer.Value += Time.deltaTime;

		UpdateScoringElements();
		UpdateSliderElements();
	}

	private void UpdateScoringElements()
	{
		timerText.text = "Time: " + timer.Value.ToString("F2");
		scoreText.text = "Score: " + score.Value.ToString();
	}

	private void UpdateSliderElements()
	{
		healthSlider.fillAmount = health.Value / 100f;
		energySlider.fillAmount = energy.Value / 100f;
	}

	public void ToggleInteractionText(bool isToggled)
	{
		gateInteractionText.enabled = isToggled;
	}

}
