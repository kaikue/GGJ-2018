using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public GameObject player;
	public Camera cam;
	public AudioSource audioSrc;
	public Text remainingText;
	public Text winLoseText;
	public Image[] infectedIndicators;
	public Slider[] infectedHealth;
	public Slider playerHealth;
	
	private Dictionary<string, Sprite> sprites;
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
		loadInfectedIndicatorSprites ();
		SetControl(player, true);
	}

	void loadInfectedIndicatorSprites() {
		sprites = new Dictionary<string, Sprite> ();

		List<GameObject> people = new List<GameObject> (GameObject.FindGameObjectsWithTag ("Person"));
		foreach (GameObject person in people) {
			string name = person.GetComponent<Person> ().Name;
			if (!sprites.ContainsKey (name)) {
				sprites.Add (name, Resources.Load<Sprite> (name + "/portrait"));
			}
		}
	}
	
	void Update()
	{
		remainingText.text = "" + uninfected.Count;

		int startIndex;
		int endIndex;
		if (infected.Count <= infectedIndicators.Length) {
			// Does not fill all indicators
			startIndex = 0;
			endIndex = infected.Count;
		} else {
			startIndex = Math.Min (playerIndex, infected.Count - infectedIndicators.Length);
			endIndex = startIndex + infectedIndicators.Length;
		}

		if (infected.Count == 0)
		{
			playerHealth.gameObject.SetActive(false);
		}
		else
		{
            FillHealthBar(playerHealth, infected[playerIndex].GetComponent<Person>());
		}

		for (int i = 0; i < infectedIndicators.Length; i++) {
			if (i + startIndex >= endIndex) {
				infectedIndicators [i].gameObject.SetActive (false);
				infectedHealth [i].gameObject.SetActive (false);
			} else {
				Person person = infected [i + startIndex].GetComponent<Person> ();

				Color tint;
				if (i + startIndex == playerIndex) {
					tint = new Color (1.0f, 1.0f, 1.0f, 1.0f);
				} else {
					tint = new Color (1.0f, 1.0f, 1.0f, 0.5f);
				}
				infectedIndicators [i].sprite = sprites[person.Name];
				infectedIndicators [i].color = tint; 
				infectedIndicators [i].gameObject.SetActive (true);

				FillHealthBar(infectedHealth[i], person);
			}
		}
	}

	private void FillHealthBar(Slider slider, Person person)
	{
		float lifePercent = person.TimeToLive / person.Lifespan;
		slider.value = lifePercent;
		slider.gameObject.SetActive(true);
		var fill = slider.GetComponentsInChildren<Image>().FirstOrDefault(t => t.name == "Fill");
		if (fill != null)
		{
			fill.color = Color.Lerp(Color.red, Color.green, lifePercent);
		}
	}

	public void PlaySound(string soundName)
	{
		audioSrc.clip = Resources.Load<AudioClip>("Sounds/" + soundName);
		audioSrc.Play();
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
		if (infected.IndexOf(person) < playerIndex)
		{
			playerIndex--;
		}
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
		PlaySound("win");
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
		PlaySound("lose");
		StartCoroutine(RestartLevel());
	}

	private IEnumerator RestartLevel()
	{
		yield return new WaitForSeconds(LEVEL_COMPLETE_TIME);
		winLoseText.gameObject.SetActive(false);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
