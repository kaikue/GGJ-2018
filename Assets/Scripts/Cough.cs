using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cough : MonoBehaviour {

	public Person cougher;
	private SpriteRenderer sr;
	private const int NUM_FRAMES = 12;
	private const float FRAME_TIME = 0.05f;
	private int frameIndex = 0;
	private float frameTimer = FRAME_TIME;
	private Sprite[] anim;

	void Start()
	{
		sr = GetComponent<SpriteRenderer>();
		FillAnim();
	}

	private void FillAnim()
	{
		anim = new Sprite[NUM_FRAMES];
		for (int i = 0; i < NUM_FRAMES; i++)
		{
			anim[i] = Resources.Load<Sprite>("Cough" + "/frame" + (i + 1));
		}
	}
	
	void Update()
	{
		frameTimer -= Time.deltaTime;
		if (frameTimer <= 0)
		{
			frameTimer = FRAME_TIME;
			frameIndex = (frameIndex + 1) % NUM_FRAMES;
			if (frameIndex == 0)
			{
				Die();
			}
		}
		sr.sprite = anim[frameIndex];
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		Person p = collider.gameObject.transform.parent.GetComponent<Person>();
		if (p != null && !p.Infected)
		{
			p.GetInfected();
		}

	}

	private void Die()
	{
		cougher.Cough = null;
		Destroy(gameObject);
	}
}
