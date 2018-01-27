using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persistent : MonoBehaviour {

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}
