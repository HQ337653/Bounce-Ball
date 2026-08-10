using System.Collections;
using BallzGame.Managers;
using UnityEngine;

namespace BallzGame.Effects
{
	public class BrickPoint : MonoBehaviour
	{
		public float flySpeed = 20f;

		private Coroutine flyCoroutine;

		public void Init(Vector2 position, Vector2 direction)
		{
			transform.position = position;

			if (flyCoroutine != null)
			{
				StopCoroutine(flyCoroutine);
			}

			flyCoroutine = StartCoroutine(FlyToTarget(direction.normalized));
		}

		private IEnumerator FlyToTarget(Vector2 initialDirection)
		{
			var target = GameManager.Instance.BrickPointTarget;
			initialDirection = new Vector2(initialDirection.x, -initialDirection.y);
			Vector2 velocity = initialDirection * flySpeed;
			float passedTime = 0;

			while (passedTime <= 2f)
			{
				// 当前目标方向
				Vector2 targetDir = ((Vector2)target.position - (Vector2)transform.position).normalized;

				// 平滑修正方向（保留一点初速度）
				velocity = Vector2.Lerp(velocity.normalized, targetDir, Time.deltaTime * 7f).normalized * flySpeed;

				transform.position += (Vector3)(velocity * Time.deltaTime);

				// 到达目标
				if (Vector2.Distance(transform.position, target.position) < 1)
				{
					transform.position = target.position;
					GameManager.Instance.shopController.GainCoin(1);
					Destroy(gameObject);
					yield break;
				}

				passedTime += Time.deltaTime;

				yield return null;
			}
			GameManager.Instance.shopController.GainCoin(1);
			Destroy(gameObject);
		}
	}
}