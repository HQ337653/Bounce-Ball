using BallzGame.Bricks;
using BallzGame.Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace BallzGame.Balls
{

	public class Ball : MonoBehaviour
	{
		[SerializeField]private int BaseDamage = 1;

		public UnityEvent<Brick> OnBallHit;
		public BallExtraDamage.BallType Type;
		public BallData Data;
		[SerializeField]
		private Rigidbody2D rb;
		BallSystemConfig config;
		private BallLauncher launcher;
		private void Awake()
		{

			config = GameManager.Instance.BallConfig;
		}
		public void Init(BallLauncher l,Vector2 direction,BallSystemConfig config)
		{
			launcher = l;
			this.config = config;


			rb.linearVelocity = direction * GameManager.Instance.BallConfig.BallSpeed;
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			Brick brick = collision.gameObject.GetComponent<Brick>();
			if (brick != null)
			{
				var damage = BaseDamage + GameManager.Instance.BallExtraDamageController.GetExtraDamage(Type);
				brick.TakeDamage(damage, rb.linearVelocity);
				OnBallHit.Invoke(brick);
			}

			var extraTime = launcher.BouncedTime - config.BounceAccelerationThreshold;
			if (extraTime >= 0)
			{
				var speed = config.BallSpeed + extraTime * config.SpeedIncrementAfterBounce;
				rb.AddForce(Random.onUnitCircle * config.RandomForceScale * extraTime);
				rb.linearVelocity =
					rb.linearVelocity.normalized * speed;
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Bottom"))
			{
				launcher.OnBallReturned(this);
				Destroy(gameObject);
			}
		}

	}

}

[System.Serializable]
public class BallSystemConfig
{
	public float BounceAccelerationThreshold=2;
	public float SpeedIncrementAfterBounce=3;
	public float BallSpeed=13;
	public float RandomForceScale=0.1f;
}