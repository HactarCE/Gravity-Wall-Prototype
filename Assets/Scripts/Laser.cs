using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Laser : MonoBehaviour {

	private Transform laserBeam;
	private int layerMask;

	// Use this for initialization
	void Start () {
		foreach (Transform child in transform) {
			if (child.tag == "LaserBeam") {
				laserBeam = child;
			}
		}
		layerMask = (1 << LayerMask.NameToLayer ("GravitySurface")) + (1 << LayerMask.NameToLayer ("Grabbable"));
	}

	void FixedUpdate () {
		RaycastHit hit;
		Physics.Raycast (transform.position, transform.forward, out hit, Mathf.Infinity, layerMask);
		float distance = transform.InverseTransformPoint (hit.point).z;
		laserBeam.localScale = new Vector3(1, 1, distance);
		laserBeam.localPosition = new Vector3(0, 0, distance / 2);
	}

}
