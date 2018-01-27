using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {

	public bool Infected;
	public bool Playing;
	public GameObject CoughPrefab;

	public string Name;
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
	
	private SpriteRenderer sr;
	private Sprite[] standSprites;
	private Sprite[][] walkAnims;
	private string[] DIRECTIONS = { "Down", "Left", "Up", "Right" };
	private const int NUM_FRAMES = 8;
	private int frameIndex = 0;
	private const float FRAME_TIME = 0.1f;
	private float frameTimer = FRAME_TIME;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		ai = GetComponent<AIController>();
		controller = GameObject.Find("GameController").GetComponent<GameController>();
		sr = GetComponent<SpriteRenderer>();
		standSprites = new Sprite[4];
		FillStandSprites();
		FillWalkSprites();
	}

	private void FillStandSprites()
	{
		walkAnims = new Sprite[4][];
		for (int i = 0; i < 4; i++)
		{
			standSprites[i] = Resources.Load<Sprite>(Name + "/" + DIRECTIONS[i] + "/frame1");
		}
	}

	private void FillWalkSprites()
	{
		for (int i = 0; i < 4; i++)
		{
			Sprite[] anim = new Sprite[NUM_FRAMES];
			for (int j = 0; j < NUM_FRAMES; j++)
			{
				anim[j] = Resources.Load<Sprite>(Name + "/" + DIRECTIONS[i] + "/frame" + (j + 1));
			}
			walkAnims[i] = anim;
		}
	}

	void Update()
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

		Vector2 vel = rb.velocity;
		if (vel.x == 0 && vel.y == 0)
		{
			frameIndex = 0;
			frameTimer = FRAME_TIME;
			sr.sprite = standSprites[GetFacingIndex()];
		}
		else
		{
			frameTimer -= Time.deltaTime;
			if (frameTimer <= 0)
			{
				frameTimer = FRAME_TIME;
				frameIndex = (frameIndex + 1) % NUM_FRAMES;
			}
			Sprite[] anim = walkAnims[GetFacingIndex()];
			sr.sprite = anim[frameIndex];
        }
	}

	private int GetFacingIndex()
	{
		if (facing[0] == -1)
		{
			return 1; //left
		}
		else if (facing[0] == 1)
		{
			return 3; //right
		}
		else if (facing[1] == -1)
		{
			return 0; //down
		}
		else if (facing[1] == 1)
		{
			return 2; //up
		}
		return 0; //should never happen
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

			if (vert != 0)
			{
				facing = new Vector2(0, vert < 0 ? -1 : 1);
			}
			else if (horiz != 0)
			{
				facing = new Vector2(horiz < 0 ? -1 : 1, 0);
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
