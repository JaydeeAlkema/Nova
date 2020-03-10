using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu_Behaviour : MonoBehaviour
{
	public AudioClip onWindowOpen;
	public AudioClip onWindowClose;

	public GameObject optionsMenu;
	public GameObject creditsPanel;

	AudioSource audioSource;
	bool optionsMenuOpen = false;
	bool creditsPanelOpen = false;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();

		optionsMenu.SetActive(false);
		creditsPanel.SetActive(false);
	}

	public void LoadScene(int sceneIndex)
	{
		PlaySound(onWindowOpen);
		SceneManager.LoadSceneAsync(sceneIndex);
	}

	public void ToggleOptionsMenu()
	{
		optionsMenuOpen = !optionsMenuOpen;
		PlaySound(optionsMenuOpen ? onWindowOpen : onWindowClose);
		optionsMenu.SetActive(optionsMenuOpen);
	}

	public void ToggleCreditsPanel()
	{
		creditsPanelOpen = !creditsPanelOpen;
		PlaySound(creditsPanelOpen ? onWindowOpen : onWindowClose);
		creditsPanel.SetActive(creditsPanelOpen);
	}

	public void QuitGame()
	{
		PlaySound(onWindowClose);
		Application.Quit();
	}

	public void PlaySound(AudioClip audioClip)
	{
		audioSource.PlayOneShot(audioClip);
	}
}
