using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameMeta
{

	public class GameResultPanel : Panel
	{
		public GameResult Result;

		public GameObject Panel;

		public TextMeshProUGUI PointsText;
		public TextMeshProUGUI BricksCountText;
		public TextMeshProUGUI BallsCountText;

		public Button ConfirmButton;

		private void Start()
		{
			ConfirmButton.onClick.AddListener(BackToMainMenu);
		}

		public void SetResult(GameResult result)
		{
			Result = result;

			PointsText.text = "Points: "+ result.Points.ToString();
			BricksCountText.text = "Brick Count: "+ result.BricksCount.ToString();
			BallsCountText.text = "Balls Own: "+result.BallsCount.ToString();
		}

		private void BackToMainMenu()
		{
			MainMenu.Instance.Goto(MainMenu.CurrentPanel.Lobby);
		}

		public override void Activate()
		{

			Panel.SetActive(true);
		}

		public override void Disable()
		{
			Panel.SetActive(false);
		}

		public struct GameResult
		{
			public int BallsCount;
			public int BricksCount;
			public int Points;

			public GameResult(
				int ballsCount,
				int bricksCount,
				int points)
			{
				BallsCount = ballsCount;
				BricksCount = bricksCount;
				Points = points;
			}
		}
	}
}