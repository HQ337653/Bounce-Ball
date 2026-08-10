using System;
using BallzGame.Bricks;
using BallzGame.Managers;
using UnityEngine;

namespace BallzGame.Minigame
{
	public abstract class IFeverGame : MonoBehaviour
	{
		public abstract void StartGame(FeverGameContext context, FeverController source);

	}
	public struct FeverGameContext
	{
		public int CurrentLevel;
		public Brick[,] Grid;
	}
}