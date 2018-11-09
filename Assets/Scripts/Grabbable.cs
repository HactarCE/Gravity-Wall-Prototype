using System;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(Rigidbody))]
public abstract class Grabbable : MonoBehaviour
{

	protected Renderer avatarRenderer;

	public bool IsColliding { get; private set; }

	private void OnCollisionEnter(Collision collision)
	{
		IsColliding = true;
	}

	private void OnTriggerStay(Collider other)
	{
		IsColliding = true;
	}

	private void OnCollisionExit(Collision collision)
	{
		IsColliding = false;
	}

	protected bool carried = false;

	public abstract void gravityTransition(Gravity.Direction oldGrav, Gravity.Direction newGrav);

	virtual public void grab (Renderer avatarRenderer)
	{
		carried = true;
		this.avatarRenderer = avatarRenderer;
	}

	virtual public void ungrab ()
	{
		carried = false;
	}

	public Rigidbody getRigidbody ()
	{
		return GetComponent<Rigidbody> ();
	}

}

