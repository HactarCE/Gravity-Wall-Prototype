using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SphereGravitySurface : MonoBehaviour, GravitySurface
{

	public Vector3 sphereCenter;
	public bool concave;

	public Material wall1;
	public Material wall2;
	public Material wall3;

	public void Start ()
	{
		Mesh mesh = Instantiate<Mesh> (GetComponent<MeshFilter> ().sharedMesh);
		Vector3[] vertices = mesh.vertices;
		Color[] colors = new Color[vertices.Length];
		for (int i = 0; i < vertices.Length; i++) {
			float x = (vertices [i].x + 1) / 2;
			float y = (vertices [i].y + 1) / 2;
			float z = (vertices [i].z + 1) / 2;
			colors [i] = lerp3 (wall1.color, wall2.color, wall3.color, x, y, z);
		}
		mesh.colors = colors;
		GetComponent<MeshFilter> ().sharedMesh = mesh;
	}

	public Vector3 getNormal (Vector3 position)
	{
		Vector3 relativePositon = position - transform.TransformPoint (sphereCenter);
		if (concave)
			return -relativePositon.normalized;
		return relativePositon.normalized;
//		print (transform.rotation * sphereCenter.normalized;
//		return transform.rotation * sphereCenter.normalized;
	}

	Color lerp3 (Color color1, Color color2, Color color3, float weight1, float weight2, float weight3)
	{
		weight1 = Mathf.Clamp01 (weight1);
		weight2 = Mathf.Clamp01 (weight2);
		weight3 = Mathf.Clamp01 (weight3);
		float weight12 = weight1 + weight2;
		float sum = weight12 + weight3;
		if (weight12 == 0)
			return color3;
		return Color.Lerp (Color.Lerp (color1, color2, weight2 / weight12), color3, weight3 / sum);
	}

}
