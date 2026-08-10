using UnityEngine;

namespace Utils
{
	[ExecuteAlways]
	[RequireComponent(typeof(SpriteRenderer))]
	public class SpriteBrightness : MonoBehaviour
	{
		[Range(0f, 5f)]
		public float brightness = 1f;

		private SpriteRenderer sr;
		private MaterialPropertyBlock mpb;

		void OnEnable()
		{
			sr = GetComponent<SpriteRenderer>();
			mpb = new MaterialPropertyBlock();
			Apply();
		}

		void OnValidate()
		{
			if (!sr) sr = GetComponent<SpriteRenderer>();
			if (mpb == null) mpb = new MaterialPropertyBlock();
			Apply();
		}

		void Apply()
		{
			sr.GetPropertyBlock(mpb);
			mpb.SetFloat("_Brightness", brightness);
			sr.SetPropertyBlock(mpb);
		}
	}
}