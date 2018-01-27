using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private Rigidbody2D rb;
	private const float SPEED_MULT = 0.05f;
	
	void Start ()
	{
		rb = GetComponent<Rigidbody2D>();
	}
	
	void Update () {
		
	}

	void FixedUpdate()
	{
		Vector2 vel = rb.velocity;

		vel.x = Input.GetAxisRaw("Horizontal") * SPEED_MULT;
		vel.y = Input.GetAxisRaw("Vertical") * SPEED_MULT;

		rb.MovePosition(rb.position + vel);
	}
}
