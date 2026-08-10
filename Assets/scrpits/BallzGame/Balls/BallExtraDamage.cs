using System.Collections.Generic;
using UnityEngine;

namespace BallzGame.Balls
{

	public class BallExtraDamage : MonoBehaviour
	{
		[SerializeField]
		private List<int> extraDamage = new List<int>();

		public void Reset()
		{
			extraDamage.Clear();
		}
		public int GetExtraDamage(BallType type)
		{
			int index = (int)type;

			if (index >= extraDamage.Count)
				return 0;

			return extraDamage[index];
		}

		public void SetExtraDamage(BallType type, int damage)
		{
			int index = (int)type;

			while (extraDamage.Count <= index)
			{
				extraDamage.Add(0);
			}

			extraDamage[index] = damage;
		}
		public void AddExtraDamage(BallType type, int damage)
		{
			int index = (int)type;

			while (extraDamage.Count <= index)
			{
				extraDamage.Add(0);
			}

			extraDamage[index] += damage;
		}

		public enum BallType
		{
			NormalBall,
			ExplosionBall

		}
	}
}