using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {

	public bool Infected;
	public bool Playable;

	private const float SPEED_MULT = 0.05f;

	private Rigidbody2D rb;
	private bool coughQueued = false;
	
	void Start ()
	{
		rb = GetComponent<Rigidbody2D>();
	}
	
	void Update () {
		if (Playable && 
			(Input.GetKeyDown(KeyCode.Mouse0) ||
			 Input.GetKeyDown(KeyCode.Space) ||
			 Input.GetKeyDown(KeyCode.JoystickButton0)))
		{
			coughQueued = true;
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
			}
		}
	}
}
