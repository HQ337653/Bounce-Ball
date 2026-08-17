using System.Collections;
using System.Collections.Generic;
using BallzGame.Managers;
using UnityEngine;
using Utils;

namespace BallzGame.Bricks.SpecialBricks
{
    public class HealBrick : SpecialBrick

    {
        public TriggerCollector triggers;
        public GameObject HealVisual;
        public int HealAmount = 3;
        private bool activated;

        [SerializeField]
        private int moveCount;

        public override void OnRowMoved()
        {
            base.OnRowMoved();
            moveCount += 1;
            if (moveCount == 1)
            {
                HealVisual.SetActive(true);
            }
            else if (moveCount > 1)
            {

                StartCoroutine(Heal());
            }

        }

        public override void OnMiniGameStart()
        {
            base.OnMiniGameStart();
            HealVisual.SetActive(false);

        }

        public override void OnMiniGameEnd()
        {
            if (moveCount >= 1)
            {
                HealVisual.SetActive(true);
            }

        }

        public IEnumerator Heal()
        {
            triggers.gameObject.SetActive(true);
            yield return new WaitForFixedUpdate();
            AddHP();
            triggers.gameObject.SetActive(false);
        }



        void AddHP()
        {
            foreach (var VARIABLE in triggers.Colliders)
            {
                if(!VARIABLE)
                    return;
                var script = VARIABLE.GetComponent<Brick>();
                if (script)
                {
                    GameManager.DoHealText(script.transform.position, HealAmount);
                    script.AddHP(HealAmount);
                }
            }
        }
    }

}
