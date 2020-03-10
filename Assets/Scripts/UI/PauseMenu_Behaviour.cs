using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu_Behaviour : MonoBehaviour
{
	public GameObject EventSystem;
	[Space]
	public bool EnablePauseMenu = false;
	public GameObject pauseMenuObject;
	[Space]
	public bool EnableOptionsMenu = false;
	public GameObject optionsMenuObject;

	void Update()
	{
		TogglePauseMenu();
		ToggleOptionsMenu();
	}

	public void TogglePauseToggle()
	{
		EnablePauseMenu = !EnablePauseMenu;
	}

	void TogglePauseMenu()
	{
		if(Input.GetButtonDown("Pause"))
			EnablePauseMenu = !EnablePauseMenu;

		Time.timeScale = (EnablePauseMenu) ? 0.0f : 1f;
		EventSystem.SetActive(EnablePauseMenu);
		pauseMenuObject.SetActive(EnablePauseMenu);
	}

	public void ToggleOptionsToggle()
	{
		EnableOptionsMenu = !EnableOptionsMenu;
	}

	void ToggleOptionsMenu()
	{
		optionsMenuObject.SetActive(EnableOptionsMenu);
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
