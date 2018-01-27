using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {

	public bool Infected;
	public bool Playable;
	public GameObject CoughPrefab;

	public float Speed;
	public float CoughSpeed;
	public float CoughDistance;

	private Rigidbody2D rb;
	private GameController controller;
	private bool switchQueued = false;
	private bool coughQueued = false;
	private GameObject cough = null;
	private Vector2 facing;
	
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

			if ((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0))
			{
				controller.UsingMouse = true;
			}

			//if (Input.GetAxis( //right stick
			//controller.usingMouse = false;
		}
	}

	void FixedUpdate()
	{
		if (Playable)
		{
			Vector2 vel = rb.velocity;

			float horiz = Input.GetAxis("Horizontal");
			float vert = Input.GetAxis("Vertical");

            vel.x = horiz * Speed;
			vel.y = vert * Speed;

			if (horiz != 0)
			{
				facing = new Vector2(horiz < 0 ? -1 : 1, 0);
			}
			else if (vert != 0)
			{
				facing = new Vector2(0, vert < 0 ? -1 : 1);
			}

			print(facing);

			rb.MovePosition(rb.position + vel);

			if (coughQueued)
			{
				print("cough");
				coughQueued = false;
				if (cough == null)
				{
					CreateCough();
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

	private void CreateCough()
	{
		/*Vector2 coughVel = new Vector2();
		if (controller.UsingMouse)
		{
			coughVel.x = Input.mousePosition.x - Screen.width / 2;
			coughVel.y = Input.mousePosition.y - Screen.height / 2;
		}
		else
		{
			//right stick
		}
		coughVel.Normalize();
		*/

		Vector2 coughVel = facing.normalized;
		coughVel *= CoughSpeed;
		coughVel += rb.velocity;

		cough = Instantiate(CoughPrefab);
		cough.transform.position = gameObject.transform.position;
		cough.transform.parent = gameObject.transform;
		cough.GetComponent<Rigidbody2D>().velocity = coughVel;
		StartCoroutine("RemoveCoughDelay");
	}

	private IEnumerator RemoveCoughDelay()
	{
		yield return new WaitForSeconds(CoughDistance);
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
