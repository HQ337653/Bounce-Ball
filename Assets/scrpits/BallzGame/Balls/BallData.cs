using BallzGame.Balls;
using UnityEngine;

namespace BallzGame.Balls
{
	[CreateAssetMenu(fileName = "BallName", menuName = "New Ball Info", order = 0)]
	public class BallData : ScriptableObject
	{
		public Sprite Icon;
		public string Name;
		public string Description;

	}
}