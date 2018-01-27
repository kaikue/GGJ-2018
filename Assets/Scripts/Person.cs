using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {

	public bool Infected;
	public bool Playing;
	public GameObject CoughPrefab;

	public float Speed;
	public float CoughSpeed;
	public float CoughDistance;
	public float Lifespan;

	private Rigidbody2D rb;
	private AIController ai;
	private GameController controller;
	private bool nextQueued = false;
	private bool prevQueued = false;
	private bool coughQueued = false;
	private GameObject cough = null;
	private Vector2 facing = new Vector2(0, -1);
	private bool dead = false;
	
	void Start ()
	{
		rb = GetComponent<Rigidbody2D> ();
		ai = GetComponent<AIController> ();
		controller = GameObject.Find("GameController").GetComponent<GameController>();
	}
	
	void Update ()
	{
		if (dead)
		{
			return;
		}

		if (Playing)
		{
			if (Input.GetKeyDown(KeyCode.Mouse0) ||
				Input.GetKeyDown(KeyCode.Space) ||
				Input.GetKeyDown(KeyCode.JoystickButton0))
			{
				coughQueued = true;
			}

			if (Input.GetKeyDown(KeyCode.RightShift) ||
				Input.GetKeyDown(KeyCode.JoystickButton5))
			{
				nextQueued = true;
			}

			if (Input.GetKeyDown(KeyCode.LeftShift) ||
				Input.GetKeyDown(KeyCode.JoystickButton4))
			{
				prevQueued = true;
			}

			/*if ((Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0))
			{
				controller.UsingMouse = true;
			}

			//if (Input.GetAxis( //right stick
			//controller.usingMouse = false;
			*/
		}
	}

	void FixedUpdate()
	{
		if (dead)
		{
			return;
		}

		if (Playing)
		{
			if (ai != null) {
				ai.enabled = false;
			}

			Vector2 vel = new Vector2();

			float horiz = Input.GetAxisRaw("Horizontal");
			float vert = Input.GetAxisRaw("Vertical");

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
			
			rb.velocity = vel;
			
			if (coughQueued)
			{
				coughQueued = false;
				if (cough == null)
				{
					CreateCough();
				}
			}

			if (nextQueued)
			{
				nextQueued = false;
				RemoveCough();
				controller.NextPlayer();
			}
			if (prevQueued)
			{
				prevQueued = false;
				RemoveCough();
				controller.PrevPlayer();
			}
		}
		else
		{
			if (ai != null) {
				ai.enabled = true;
				rb.velocity = ai.GetVelocity ();
			}
		}

		if (Infected)
		{
			Lifespan -= Time.fixedDeltaTime;
			if (Lifespan <= 0)
			{
				Die();
			}
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
			GetInfected();
		}
	}

	private void GetInfected()
	{
		Infected = true;
		controller.AddInfected(gameObject);
	}

	private void Die()
	{
		//TODO make it look dead
		dead = true;
		Destroy(rb);
		Destroy(gameObject.GetComponent<BoxCollider2D>()); //or maybe not, if people still interact
		controller.RemoveDead(gameObject);

		if (Playing)
		{
			Playing = false;
			controller.SwitchDead();
		}
	}
}
