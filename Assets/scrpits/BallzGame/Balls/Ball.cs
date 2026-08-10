using BallzGame.Bricks;
using BallzGame.Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace BallzGame.Balls
{

	public class Ball : MonoBehaviour
	{
		public int BasedDamage = 1;
		public float BaseSpeed = 1;
		private BallLauncher launcher;
		public UnityEvent<Brick> OnBallHit;
		public BallExtraDamage.BallType Type;

		public void Init(BallLauncher l, float baseSpeed)
		{
			launcher = l;
			BaseSpeed = baseSpeed;
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			Brick brick = collision.gameObject.GetComponent<Brick>();
			Rigidbody2D rb = GetComponent<Rigidbody2D>();
			if (brick != null)
			{
				var damage = BasedDamage + GameManager.Instance.BallExtraDamageController.GetExtraDamage(Type);
				brick.TakeDamage(damage, rb.linearVelocity);
				OnBallHit.Invoke(brick);
			}

			var extraTime = launcher.BouncedTime - 2;
			if (extraTime >= 0)
			{
				float speed = BaseSpeed + extraTime * launcher.SpeedIncrease;
				rb.AddForce(Random.onUnitCircle * 0.1f * extraTime);
				rb.linearVelocity =
					rb.linearVelocity.normalized * speed;
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Bottom"))
			{
				launcher.OnBallReturned(this);

				// 如果不用对象池就销毁
				Destroy(gameObject);

				// 如果以后用对象池：
				// gameObject.SetActive(false);
			}
		}

	}

}