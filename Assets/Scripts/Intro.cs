using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour {
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space) ||
			Input.GetKeyDown(KeyCode.JoystickButton0))
		{
			SceneManager.LoadScene(1);
        }
	}
}
