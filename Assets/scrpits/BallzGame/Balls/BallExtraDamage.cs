using System.Collections.Generic;
using UnityEngine;

namespace BallzGame.Balls
{

	public class BallExtraDamage : MonoBehaviour
	{
		[SerializeField]
		private List<int> extraDamage = new List<int>();
		[ContextMenu("Init list")]
		private void Awake()
		{
			int count = System.Enum.GetValues(typeof(BallType)).Length;
			if (extraDamage.Count != count)
			{
				extraDamage = new List<int>(new int[count]);
			}
		}
		public void Reset()
		{
			for (int i = 0; i < extraDamage.Count; i++)
			{
				extraDamage[i] = 0;
			}
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