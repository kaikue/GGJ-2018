using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public GameObject player;
	public Camera cam;

	public bool UsingMouse = true;

	private List<GameObject> infected;
	private int playerIndex;

	void Start ()
	{
		infected = new List<GameObject>();
		AddInfected(player);
		playerIndex = 0;
		SetControl(player, true);
	}
	
	void Update ()
	{
		
	}

	private void SetControl(GameObject personObj, bool controlling)
	{
		if (controlling)
		{
			player = personObj;
			cam.transform.SetParent(personObj.transform);
			cam.transform.position = new Vector3(personObj.transform.position.x,
												personObj.transform.position.y,
												cam.transform.position.z);
        }
		Person person = personObj.GetComponent<Person>();
		person.Playable = controlling;
	}

	public void AddInfected(GameObject person)
	{
		infected.Add(person);
	}

	public void SwitchPlayer()
	{
		SetControl(player, false);
		playerIndex = (playerIndex + 1) % infected.Count; //TODO: make a way to choose between infected
		SetControl(infected[playerIndex], true);
		print("Switch player " + playerIndex);
	}
}
