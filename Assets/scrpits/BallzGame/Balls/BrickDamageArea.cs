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
		[SerializeField] private Vector2 ForceDirection;
		[SerializeField] private float ForceFromCenterMagnitude;
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
				if(brick.Status.Contains(Brick.BrickStatus.DisableEffect))
					return;
				brick.TakeDamage(Damage);
				Vector2 force=Vector2.zero;
				if (ForceFromCenterMagnitude > -1)
				{
					Vector2 forceDirection = gameObject.transform.position - other.transform.position;
					forceDirection.Normalize();
					forceDirection=new Vector2(-forceDirection.x,forceDirection.y);
					force= forceDirection * ForceFromCenterMagnitude;
					brick.TakeDamage(Damage,force);
				}
				else if(ForceDirection != Vector2.zero)
				{
					force = ForceDirection;
					brick.TakeDamage(Damage,force);
				}
				brick.TakeDamage(Damage);
			}

		}
	}
}