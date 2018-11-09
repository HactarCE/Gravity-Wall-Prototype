using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiCrosshair : MonoBehaviour {

	public Texture2D crosshairTexture;
	public float scale = 1.0f;

	void OnGUI() {
		Rect position = new Rect (
			(Screen.width - crosshairTexture.width * scale) / 2,
			(Screen.height - crosshairTexture.height * scale) / 2,
			crosshairTexture.width * scale,
			crosshairTexture.height * scale
		);
		GUI.DrawTexture (position, crosshairTexture);
	}

}
