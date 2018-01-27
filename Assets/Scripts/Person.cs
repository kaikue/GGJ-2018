using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {

	public bool Infected;
	public bool Playable;
	public GameObject CoughPrefab;

	private const float SPEED_MULT = 0.05f;

	private Rigidbody2D rb;
	private GameController controller;
	private bool switchQueued = false;
	private bool coughQueued = false;
	private GameObject cough = null;
	
	void Start ()
	{
		rb = GetComponent<Rigidbody2D>();
		controller = GameObject.Find("GameController").GetComponent<GameController>();
	}
	
	void Update ()
	{
		if (Playable)
		{
			if (Input.GetKeyDown(KeyCode.Mouse0) ||
				Input.GetKeyDown(KeyCode.Space) ||
				Input.GetKeyDown(KeyCode.JoystickButton0))
			{
				coughQueued = true;
			}
			
			if (Input.GetKeyDown(KeyCode.Tab) ||
				Input.GetKeyDown(KeyCode.JoystickButton5))
			{
				switchQueued = true;
			}
		}
	}

	void FixedUpdate()
	{
		if (Playable)
		{
			Vector2 vel = rb.velocity;

			vel.x = Input.GetAxis("Horizontal") * SPEED_MULT;
			vel.y = Input.GetAxis("Vertical") * SPEED_MULT;

			rb.MovePosition(rb.position + vel);

			if (coughQueued)
			{
				print("cough");
				coughQueued = false;
				if (cough == null)
				{
					cough = Instantiate(CoughPrefab);
					cough.transform.position = gameObject.transform.position;
					cough.transform.parent = gameObject.transform;
					StartCoroutine("RemoveCoughDelay");
				}
			}

			if (switchQueued)
			{
				switchQueued = false;
				RemoveCough();
				controller.SwitchPlayer();
			}
		}
		else
		{
			//AI
			rb.velocity = new Vector2(0, 0);
		}
	}

	private IEnumerator RemoveCoughDelay()
	{
		yield return new WaitForSeconds(1);
		RemoveCough();
	}

	private void RemoveCough()
	{
		Destroy(cough);
		cough = null;
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (!Infected && collider.gameObject.tag == "Cough")
		{
			print("im infected");
			Infected = true;
			controller.AddInfected(gameObject);
		}
	}
}
