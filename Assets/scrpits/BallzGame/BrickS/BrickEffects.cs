using UnityEngine;
using System.Collections;
using BallzGame.Managers;

namespace BallzGame.Bricks
{
	public class BrickEffects : MonoBehaviour
	{
		public SpriteRenderer Flash;

		public float flashSpeed = 5f;
		[SerializeField] private ParticleSystem EffectPrefab;
		public float knockbackTime = 0.03f;
		public float returnTime = 0.1f;
		public float knockbackMultiplier;
		private Coroutine flashCoroutine;
		private Coroutine knockbackCoroutine;
		public Gradient Colors;
		public SpriteRenderer CenterSprite;
		public Color ShieldColor;
		public Color NormalColor;
		public SpriteRenderer Outline;

		public void SetSheildEffect(bool shield)
		{
			Outline.color = shield ? ShieldColor : NormalColor;
		}

		[ContextMenu("hit")]
		public void doHit(Vector2 force)
		{
			// Flash
			if (flashCoroutine != null)
			{
				StopCoroutine(flashCoroutine);
			}

			flashCoroutine = StartCoroutine(FlashEffect());

			// Knockback
			if (knockbackCoroutine == null && force != Vector2.zero)
			{
				knockbackCoroutine = StartCoroutine(KnockbackEffect(force));
			}
		}

		public void ChangeVisual(bool showing)
		{
			gameObject.SetActive(showing);
		}

		public void MakeBreakEffect()
		{
			Instantiate(EffectPrefab, transform.position, Quaternion.identity,GameManager.Instance.VisualEffectsParent);
		}

		private IEnumerator FlashEffect()
		{
			Color color = Flash.color;

			while (color.a < 1f)
			{
				color.a = Mathf.MoveTowards(color.a, 1f, flashSpeed * Time.deltaTime);
				Flash.color = color;
				yield return null;
			}

			while (color.a > 0f)
			{
				color.a = Mathf.MoveTowards(color.a, 0f, flashSpeed * Time.deltaTime);
				Flash.color = color;
				yield return null;
			}

			flashCoroutine = null;
		}

		private IEnumerator KnockbackEffect(Vector2 force)
		{
			Vector3 startPos = Vector3.zero;
			Vector2 direction = new Vector2(force.x, -force.y);
			Vector3 targetPos = startPos + (Vector3)direction * knockbackMultiplier;

			// 确保开始位置是原点
			transform.localPosition = startPos;

			float timer = 0f;

			// 推出去
			while (timer < knockbackTime)
			{
				timer += Time.deltaTime;
				float t = timer / knockbackTime;

				transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
				yield return null;
			}

			transform.localPosition = targetPos;

			// 回原位
			timer = 0f;

			while (timer < returnTime)
			{
				timer += Time.deltaTime;
				float t = timer / returnTime;

				transform.localPosition = Vector3.Lerp(targetPos, startPos, t);
				yield return null;
			}

			transform.localPosition = startPos;
			knockbackCoroutine = null;
		}
	}
}