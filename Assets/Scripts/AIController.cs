using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

	public Vector2[] targets = new Vector2[] {new Vector2(0.0f, 0.0f)};
	public float expectedStationaryTime = 5.0f;
	public float coughRunTime = 2.0f;
	public float coughRunRange = 5.0f;

	private Person person;
	private Vector2 initialPosition;
	private Vector2 velocity; 
	private bool isRunning;
	private bool isWalking;
	private int targetIndex;
	private float coughRunTimeRemaining;

	void Start() {
		person = GetComponent<Person> ();
		initialPosition = gameObject.transform.position;
		velocity = new Vector2 (0.0f, 0.0f);
		isWalking = false;
		isRunning = false;
		targetIndex = 0;
	}

	void FixedUpdate () {
		if (!person.Infected) {
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
						SetRunning (cough.transform.position, coughRunTime);
						break;
					}
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
		isWalking = true;
		isRunning = false;
	}

	public void OnDisable() {
		SetStationary ();
	}

	public Vector2 GetVelocity() {
		return velocity;
	}

	public void collision(Collider2D collider)
	{
		if (collider.gameObject.tag == "Person" && (!collider.gameObject.GetComponent<AIController>().enabled))
		{
			SetRunning (collider.gameObject.transform.position, 0.1f);
		}
	}

	private void SetStationary() {
		isWalking = false;
		isRunning = false;
		velocity.Set (0.0f, 0.0f);
   		targetIndex = (targetIndex + 1) % targets.Length;
	}

	private void SetRunning(Vector2 position, float runTime) {
		isRunning = true;
		isWalking = false;
		coughRunTimeRemaining = runTime;
		velocity = ((Vector2)gameObject.transform.position) - position;
		velocity.Normalize ();
		velocity.Scale (new Vector2(person.Speed * 2.0f, person.Speed * 2.0f));
	}

	private void SetWalking() {
		isWalking = true;
		isRunning = false;
		SetVelocity ();
	}

	private void SetVelocity() {
		velocity.Set (
			targets[targetIndex].x + initialPosition.x - gameObject.transform.position.x,
			targets[targetIndex].y + initialPosition.y - gameObject.transform.position.y);
		velocity.Normalize ();
		velocity.Scale (new Vector2 (person.Speed, person.Speed));
	}

	private bool ShouldWalk() {
		float walkProbability = 1.0f / (expectedStationaryTime / Time.fixedDeltaTime);
		return Random.value < walkProbability;
	}

	private float DistanceToTarget() {
		return Vector2.Distance (targets[targetIndex] + initialPosition, gameObject.transform.position);	
	}
}
