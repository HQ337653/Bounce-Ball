using BallzGame.Bricks;
using BallzGame.Managers;
using UnityEngine;

public class AirSlashBall : MonoBehaviour
{
	[SerializeField]private Rigidbody2D BallRigidBody;
	[SerializeField]private Rigidbody2D SlashPrefabRigidBody;
	[SerializeField]private float speed;
	int damage;
	public float ExplosionPossibility;

	public void OnBallHit(Brick brick)
	{
		if (Random.Range(0f, 1f) < ExplosionPossibility
		    )
		{
			if (brick.Status.Contains(Brick.BrickStatus.DisableEffect))
			{
				GameManager.DoVoidFloatText(brick.transform.position);
				return;
			}
			var obj = Instantiate(
				SlashPrefabRigidBody,
				brick.transform.position,
				Quaternion.identity,
				GameManager.Instance.BallsParent
			);

			// Ball 当前的速度方向
			Vector2 direction =brick.transform.position - transform.position;

			// 让 Slash 朝向速度方向
			// Slash 默认朝向 Y 轴正方向，所以需要 -90° + 方向角
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
			obj.transform.rotation = Quaternion.Euler(0f, 0f, angle);

			// 让 Slash 按照 Ball 当前速度飞行
			obj.linearVelocity = direction.normalized*speed;
		}

	}
}