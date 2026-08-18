using BallzGame.Balls;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallCard : MonoBehaviour
{
    public Image IconImage;
    public TMP_Text Count;
    public Button Button;
    public TMP_Text NameText;
    public TMP_Text DescriptionText;
    public void SetData(BallData data)
    {
        if (NameText)
        {
            NameText.text = data.Name;
        }

        if (IconImage)
        {
            IconImage.sprite = data.Icon;
        }

        if (DescriptionText)
        {
            DescriptionText.text = data.Description;
        }

    }

    public void SetData(BallData data, int count)
    {
        SetData(data);
        if (Count)
        {
            Count.text =count.ToString();
        }
    }
}
