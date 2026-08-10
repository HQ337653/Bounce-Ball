using System;
using System.Collections;
using System.Collections.Generic;
using BallzGame.Minigame;
using UnityEngine;
using UnityEngine.Serialization;
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
        }
        public FeverController(List<IFeverGame> feverGameContexts)
        {
            FeverGames = feverGameContexts;
        }

        public bool AddFeverPoints(int amount)
        {
            CurrentFever += amount;
            FeverFill.fillAmount = (float)CurrentFever / MaxFever;
            if (CurrentFever >= MaxFever)
            {
                FeverButton.interactable = true;
                return true;
            }

            FeverButton.interactable = false;

            return false;
        }

        public IEnumerator StartFeverGame(FeverGameContext context)
        {
            feverEnded = false;
            Debug.Log("Starting Fever Game");
            gameObject.SetActive(true);
            if (FeverGames.Count == 0)
                yield break;

            IFeverGame game =
                FeverGames[
                    UnityEngine.Random.Range(
                        0,
                        FeverGames.Count
                    )
                ];
            game.StartGame(context, this);


            // 等待变量变成 true
            yield return new WaitUntil(() => feverEnded);

            gameObject.SetActive(false);
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
    }


}