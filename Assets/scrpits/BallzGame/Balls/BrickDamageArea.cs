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
			StartCoroutine(Destory(0.05f));
		}

		public IEnumerator Destory(float time)
		{
			yield return new WaitForSeconds(time);
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