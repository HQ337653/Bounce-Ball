using System.Collections;
using System.Collections.Generic;
using BallzGame.Minigame;
using UnityEngine;
using UnityEngine.UI;

namespace BallzGame.Managers
{
    public class FeverController : MonoBehaviour
    {
        public List<IFeverGame> FeverGames;
        public Image FeverFill;
        public int CurrentFever;
        public int MaxFever;
        public Button FeverButton;
        public void Reset()
        {
            CurrentFever = 0;
            FeverFill.fillAmount = 0;
            FeverButton.interactable = false;
            FeverClicked = false;
            feverEnded = false;
            AddFeverPoints(0);
        }

        public bool AddFeverPoints(int amount)
        {
            CurrentFever += amount;
            FeverFill.fillAmount = (float)CurrentFever / MaxFever;
            if (CurrentFever >= MaxFever)
            {
                CurrentFever = MaxFever;
                return true;


            }

            return false;
        }

        public IEnumerator StartFeverGame(FeverGameContext context)
        {
            feverEnded = false;
            CurrentFever = 0;
            FeverFill.fillAmount = 0;
            Debug.Log("Starting Fever Game");
            if (FeverGames.Count == 0)
                yield break;

            IFeverGame game =
                FeverGames[
                    Random.Range(
                        0,
                        FeverGames.Count
                    )
                ];
            game.gameObject.SetActive(true);
            yield return null;
            game.StartGame(context, this);


            // 等待变量变成 true
            yield return new WaitUntil(() => feverEnded);

            Debug.Log("Fever Manager Game Ended");
        }

        bool feverEnded = false;

        public void FeverEnd()
        {
            Debug.Log("Fever Ended");
            feverEnded = true;
        }

        public void Start()
        {
            FeverButton.onClick.AddListener(FeverClick);
        }

        private void FeverClick()
        {
            FeverClicked = true;
        }

        public bool FeverClicked;

        public void WaitForInput()
        {
            FeverClicked=false;
            if (CurrentFever >= MaxFever)
            {
                FeverButton.interactable = true;
            }
        }

        public void StopListenToInput()
        {
            FeverClicked=false;
            FeverButton.interactable = false;
        }
    }


}