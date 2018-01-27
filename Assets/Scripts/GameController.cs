using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public GameObject player;
	public Camera cam;
	public Text txt;

	public bool UsingMouse = true;

	private List<GameObject> infected;
	private int playerIndex;
	private List<GameObject> uninfected;

	void Start()
	{
		uninfected = new List<GameObject>(GameObject.FindGameObjectsWithTag("Person"));
		infected = new List<GameObject>();
		AddInfected(player);
		playerIndex = 0;
		SetControl(player, true);
	}
	
	void Update()
	{
		txt.text = "Uninfected Remaining: " + uninfected.Count;
	}

	void FixedUpdate()
	{
		int numRemaining = uninfected.Count;
		if (numRemaining == 0)
		{
			print("YOU WIN!!!");
			//TODO win stuff
		}
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
		uninfected.Remove(person);
	}

	public void SwitchPlayer()
	{
		SetControl(player, false);
		playerIndex = (playerIndex + 1) % infected.Count; //TODO: make a way to choose between infected
		SetControl(infected[playerIndex], true);
		print("Switch player " + playerIndex);
	}

}
