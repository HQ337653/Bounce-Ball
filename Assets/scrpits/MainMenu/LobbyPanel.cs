using BallzGame.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GameMeta
{
	public class LobbyPanel : Panel
	{
		public Button StartGameButton;

		private void Start()
		{
			StartGameButton.onClick.AddListener(StartGame);
		}

		private void StartGame()
		{


			GameManager.Instance.NewGame();

		}

		public override void Activate()
		{
			gameObject.SetActive(true);
		}

		public override void Disable()
		{
			gameObject.SetActive(false);
		}
	}
}