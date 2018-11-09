using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CylinderGravitySurface : MonoBehaviour, GravitySurface
{

	public Vector3 cylinderCenter;
	public bool concave;

	public Material wall1;
	public Material wall2;

	public void Start ()
	{
		Mesh mesh = Instantiate<Mesh> (GetComponent<MeshFilter> ().sharedMesh);
		Vector3[] vertices = mesh.vertices;
		Color[] colors = new Color[vertices.Length];
		for (int i = 0; i < vertices.Length; i++) {
			float x = (vertices [i].x + 0.8f) / 2;
			float y = (vertices [i].y + 0.8f) / 2;
			colors [i] = lerp2 (wall1.color, wall2.color, x, y);
		}
		mesh.colors = colors;
		GetComponent<MeshFilter> ().sharedMesh = mesh;
	}

	public Vector3 getNormal (Vector3 position)
	{
		Vector3 relativePositon = transform.InverseTransformPoint (position) - cylinderCenter;
		relativePositon.z = 0;
		relativePositon = transform.rotation * relativePositon;
		if (concave)
			return -relativePositon.normalized;
		return relativePositon.normalized;
	}

	Color lerp2 (Color color1, Color color2, float weight1, float weight2)
	{
		weight1 = Mathf.Clamp01 (weight1);
		weight2 = Mathf.Clamp01 (weight2);
		float sum = weight1 + weight2;
		return Color.Lerp (color1, color2, weight2 / sum);
	}

}
