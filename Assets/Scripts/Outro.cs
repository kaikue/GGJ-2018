using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Outro : MonoBehaviour {
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space) ||
			Input.GetKeyDown(KeyCode.JoystickButton0))
		{
			Application.Quit();
        }
	}
}
