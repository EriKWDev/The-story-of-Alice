using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBackground : MonoBehaviour {

	public Material BG;
	float v = 0f;
	public float multiply = 0.1f;

	void Update () {
		v += Time.deltaTime * multiply;
		BG.mainTextureOffset = Vector2.Lerp (BG.mainTextureOffset, new Vector2 (v, v), Time.deltaTime * multiply);
	}
}
