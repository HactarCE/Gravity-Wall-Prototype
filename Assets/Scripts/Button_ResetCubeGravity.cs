using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Renderer))]
public class Button_ResetCubeGravity : Button
{

	public GameObject cube;
	public Gravity.Direction gravity;

	new public void Start ()
	{
		base.Start ();
		Renderer renderer = transform.GetChild (0).GetComponent<Renderer> ();
		renderer.materials = new Material[] { renderer.materials [0], GravityCube.indicatorMaterials [gravity] };
	}

	public override void Activate ()
	{
		cube.GetComponent<GravityCube> ().setGravity (gravity);
	}

}
