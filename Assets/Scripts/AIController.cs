using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

	public Vector2[] targets;
	public float expectedStationaryTime = 2.0f;

	private Person person;
	private Vector2 velocity; 
	private bool isWalking;
	private int targetIndex;

	void Start() {
		person = GetComponent<Person> ();
		velocity = new Vector2 (0.0f, 0.0f);
		isWalking = false;
		targetIndex = 0;
	}

	void FixedUpdate () {
		if (isWalking) {
			if (DistanceToHome () < person.Speed) {
				SetStationary ();
			} else {
				SetVelocity ();
			}
		} else {
			if (ShouldWalk ()) {
				SetWalking ();
			}
		}
 	}

	public void OnEnable() {
		SetStationary ();	
	}

	public Vector2 GetVelocity() {
		return velocity;
	}

	private void SetStationary() {
		isWalking = false;
		velocity.Set (0.0f, 0.0f);
	}

	private void SetWalking() {
		isWalking = true;
		targetIndex = (targetIndex + 1) % targets.Length;
		SetVelocity ();
	}

	private void SetVelocity() {
		velocity.Set (
			targets[targetIndex].x - gameObject.transform.position.x,
			targets[targetIndex].y - gameObject.transform.position.y);
		velocity.Normalize ();
		velocity.Scale (new Vector2 (person.Speed, person.Speed));
	}

	private bool ShouldWalk() {
		float walkProbability = 1.0f / (expectedStationaryTime / Time.deltaTime);
		return Random.value < walkProbability;
	}

	private float DistanceToHome() {
		return Vector2.Distance (targets[targetIndex], gameObject.transform.position);	
	}
}
