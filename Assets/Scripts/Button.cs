using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Button : MonoBehaviour {

	private Animator animator;
	public AudioClip pushSound;
	private float lastPress = 0;

	protected void Start () {
		animator = GetComponentInChildren<Animator> ();
	}

	public void Press() {
		if (lastPress + 0.5 < Time.time) {
			animator.SetTrigger ("Push");
			animator.speed = 2;
			AudioSource.PlayClipAtPoint (pushSound, transform.position);
			Activate ();
			lastPress = Time.time;
		}
	}

	public abstract void Activate ();

}
