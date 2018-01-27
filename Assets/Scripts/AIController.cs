using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

	public Vector2[] targets = new Vector2[] {new Vector2(0.0f, 0.0f)};
	public float expectedStationaryTime = 10.0f;
	public float coughRunTime = 5.0f;
	public float coughRunRange = 5.0f;

	private Person person;
	private Vector2 velocity; 
	private bool isRunning;
	private bool isWalking;
	private int targetIndex;
	private float coughRunTimeRemaining;

	void Start() {
		person = GetComponent<Person> ();
		velocity = new Vector2 (0.0f, 0.0f);
		isWalking = false;
		isRunning = false;
		targetIndex = 0;
	}

	void FixedUpdate () {
		if (isRunning) {
			if (coughRunTimeRemaining <= 0.0f) {
				SetStationary ();	
			} else {
				coughRunTimeRemaining -= Time.fixedDeltaTime;
			}
		} else {
			GameObject[] coughs = GameObject.FindGameObjectsWithTag ("Cough");
			foreach (GameObject cough in coughs) {
				if (Vector2.Distance (cough.transform.position, gameObject.transform.position) < coughRunRange) {
					SetRunning (cough);
					break;
				}
			}
		}

		if (isWalking) {
			if (DistanceToTarget () < person.Speed * Time.fixedDeltaTime) {
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
		SetWalking ();
	}

	public void OnDisable() {
		SetStationary ();
	}

	public Vector2 GetVelocity() {
		return velocity;
	}

	private void SetStationary() {
		isWalking = false;
		isRunning = false;
		velocity.Set (0.0f, 0.0f);
	}

	private void SetRunning(GameObject cough) {
		isRunning = true;
		isWalking = false;
		velocity = gameObject.transform.position - cough.transform.position;
		velocity.Normalize ();
		velocity.Scale (new Vector2(person.Speed * 2, person.Speed * 2));
	}

	private void SetWalking() {
		isWalking = true;
		isRunning = false;
		SetVelocity ();
   		targetIndex = (targetIndex + 1) % targets.Length;
	}

	private void SetVelocity() {
		velocity.Set (
			targets[targetIndex].x - gameObject.transform.position.x,
			targets[targetIndex].y - gameObject.transform.position.y);
		velocity.Normalize ();
		velocity.Scale (new Vector2 (person.Speed, person.Speed));
	}

	private bool ShouldWalk() {
		float walkProbability = 1.0f / (expectedStationaryTime / Time.fixedDeltaTime);
		return Random.value < walkProbability;
	}

	private float DistanceToTarget() {
		return Vector2.Distance (targets[targetIndex], gameObject.transform.position);	
	}
}
