using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour {

	public GameObject loadingText;

	void Update () {
		if (Input.GetKeyDown(KeyCode.Space) ||
			Input.GetKeyDown(KeyCode.JoystickButton0))
		{
			if (loadingText != null)
			{
				loadingText.SetActive(true);
			}
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}
	}
}
