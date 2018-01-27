using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public GameObject player;
	public Camera cam;

	void Start ()
	{
		TakeControl(player);
	}
	
	void Update ()
	{
		
	}

	void TakeControl(GameObject player)
	{
		Person person = player.GetComponent<Person>();
		person.Playable = true;
		cam.transform.SetParent(player.transform);
	}
}
