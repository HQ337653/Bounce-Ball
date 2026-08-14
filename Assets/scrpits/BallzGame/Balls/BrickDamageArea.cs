using System;
using System.Collections;
using BallzGame.Bricks;
using UnityEngine;
namespace BallzGame.Balls
{
	public class BrickDamageArea : MonoBehaviour
	{
		public int Damage;
		[SerializeField] private Collider2D collider;

		private void Start()
		{
			StartCoroutine(WaitAndDisableCollider());
		}

		public IEnumerator WaitAndDisableCollider()
		{
			yield return null;
			yield return new WaitForFixedUpdate();
			if (collider != null)
				collider.enabled = false;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			Brick brick = other.gameObject.GetComponent<Brick>();
			if (brick != null)
			{
				brick.TakeDamage(Damage);
			}

		}
	}
}