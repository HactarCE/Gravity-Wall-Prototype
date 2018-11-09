using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Renderer))]
[RequireComponent (typeof(Rigidbody))]
public class GravityCube : Grabbable
{

	public Gravity.Direction gravity;
	public Material mDown;
	public Material mUp;
	public Material mLeft;
	public Material mRight;
	public Material mFront;
	public Material mBack;
	public Material mNeutral;
	public Material mSensitized;
	public Material mLocked;


	new Renderer renderer;
	new Rigidbody rigidbody;
	public static Dictionary<Gravity.Direction, Material> indicatorMaterials = null;

	void Awake ()
	{
		if (indicatorMaterials == null) {
			indicatorMaterials = new Dictionary<Gravity.Direction, Material> ();
			indicatorMaterials.Add (Gravity.Direction.Down, mDown);
			indicatorMaterials.Add (Gravity.Direction.Up, mUp);
			indicatorMaterials.Add (Gravity.Direction.Left, mLeft);
			indicatorMaterials.Add (Gravity.Direction.Right, mRight);
			indicatorMaterials.Add (Gravity.Direction.Front, mFront);
			indicatorMaterials.Add (Gravity.Direction.Back, mBack);
			indicatorMaterials.Add (Gravity.Direction.Neutral, mNeutral);
		}
	}

	void Start ()
	{
		renderer = GetComponent<Renderer> ();
		rigidbody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate ()
	{
		rigidbody.AddForce (Gravity.vectors [gravity] * -Physics.gravity.y * Time.fixedDeltaTime, ForceMode.Impulse);
		updateGlow ();
	}

	void updateGlow ()
	{
		Material rim;
		if (carried)
			rim = Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl) ? mLocked : mSensitized;
		else
			rim = mNeutral;
		renderer.materials = new Material[] { renderer.materials [0], rim, indicatorMaterials [gravity] };
		if (carried)
			avatarRenderer.materials = renderer.materials;
	}

	public override void gravityTransition (Gravity.Direction oldGrav, Gravity.Direction newGrav)
	{
		if (!(Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl))) {
			setGravity (Gravity.applyTransition (Gravity.getTransition (oldGrav, newGrav), gravity));
		}
	}

	public void setGravity (Gravity.Direction grav)
	{
		if (gravity != grav) {
			gravity = grav;
		}
	}

}
