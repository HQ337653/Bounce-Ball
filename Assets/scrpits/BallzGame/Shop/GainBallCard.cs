
using BallzGame.Balls;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BallzGame.Managers.Shop
{

	public class GainBallCard : MonoBehaviour
	{
		public TextMeshProUGUI NameText;
		public Image IconImage;

		public void SetData(BallData data)
		{
			NameText.text = data.Name;
			IconImage.sprite = data.Icon;
		}
	}

}