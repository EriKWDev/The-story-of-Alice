using UnityEngine;

[ExecuteInEditMode]
public class ImageEffectOne : MonoBehaviour {

	public Material effectMaterial;
	public Material effectMaterialTwo;
	[Range(0, 10)]
	public int iterations = 1;
	[Range(0, 5)]
	public int downRes = 1;

	public void OnRenderImage (RenderTexture src, RenderTexture dst) {

		int width = src.width >> downRes;
		int height = src.height >> downRes;

		RenderTexture tmp1 = RenderTexture.GetTemporary (width, height);
		Graphics.Blit (src, tmp1);

		for (int i = 0; i < iterations; i++) {
			RenderTexture tmp2 = RenderTexture.GetTemporary (width, height);
			Graphics.Blit (tmp1, tmp2, effectMaterial);
			RenderTexture.ReleaseTemporary (tmp1);
			tmp1 = tmp2;
		}

		if (effectMaterialTwo == null)
			Graphics.Blit (tmp1, dst);
		else
			Graphics.Blit (tmp1, dst, effectMaterialTwo);
		
		RenderTexture.ReleaseTemporary (tmp1);
	}
}
