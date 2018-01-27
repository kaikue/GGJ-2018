using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public GameObject player;
	public Camera cam;
	public Text remainingText;
	public Text winLoseText;

	//public bool UsingMouse = true;

	private List<GameObject> infected;
	private int playerIndex;
	private List<GameObject> uninfected;
	private bool won = false;
	private const float LEVEL_COMPLETE_TIME = 3.0f;

	void Start()
	{
		uninfected = new List<GameObject>(GameObject.FindGameObjectsWithTag("Person"));
		infected = new List<GameObject>();
		player.GetComponent<Person>().GetInfected();
		playerIndex = 0;
		SetControl(player, true);
	}
	
	void Update()
	{
		remainingText.text = "HEALTHY REMAINING: " + uninfected.Count;
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
		person.Playing = controlling;
		personObj.GetComponent<Rigidbody2D>().velocity = new Vector2();
	}

	public void AddInfected(GameObject person)
	{
		infected.Add(person);
		uninfected.Remove(person);

		int numRemaining = uninfected.Count;
		if (numRemaining == 0)
		{
			Win();
		}
	}

	public void RemoveDead(GameObject person)
	{
		infected.Remove(person);
	}

	private int IndexMod(int x, int m)
	{
		return (x % m + m) % m;
    }

	public void NextPlayer()
	{
		SwitchPlayer(IndexMod(playerIndex + 1, infected.Count));
    }

	public void PrevPlayer()
	{
		SwitchPlayer(IndexMod(playerIndex - 1, infected.Count));
	}

	public void SwitchPlayer(int newIndex)
	{
		SetControl(player, false);
		playerIndex = newIndex;
		SetControl(infected[playerIndex], true);
	}

	public void SwitchDead()
	{
		if (infected.Count == 0)
		{
			if (!won)
			{
				Lose();
			}
		}
		else
		{
			NextPlayer();
		}
	}

	public void Win()
	{
		won = true;
		winLoseText.gameObject.SetActive(true);
		winLoseText.text = "LEVEL COMPLETE!";
		StartCoroutine(NextLevel());
	}

	private IEnumerator NextLevel()
	{
		yield return new WaitForSeconds(LEVEL_COMPLETE_TIME);
		winLoseText.gameObject.SetActive(false);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void Lose()
	{
		winLoseText.gameObject.SetActive(true);
		winLoseText.text = "GAME OVER";
		StartCoroutine(RestartLevel());
	}

	private IEnumerator RestartLevel()
	{
		yield return new WaitForSeconds(LEVEL_COMPLETE_TIME);
		winLoseText.gameObject.SetActive(false);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
