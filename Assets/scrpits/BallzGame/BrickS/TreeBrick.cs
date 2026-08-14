using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace BallzGame.Bricks.SpecialBricks
{


    public class TreeBrick : SpecialBrick
    {
        public SpriteRenderer TreeIcon;
        public TriggerCollector triggers;
        public List<Brick> SurroundingBricks = new List<Brick>();
        public float DefenseInterval;
        public float WaitInterval;
        Coroutine AddDefenseRoutine;

        private bool activated;

        public IEnumerator BrickEffect()
        {
            while (true)
            {
                yield return new WaitForSeconds(WaitInterval);
                AddDefenseRoutine = StartCoroutine(AddingDefense());

                // 等待这个协程执行完
                yield return AddDefenseRoutine;

            }
        }

        public override void OnMiniGameStart()
        {
            base.OnMiniGameStart();
            Stop();
        }

        override public void OnMiniGameEnd()
        {
            base.OnMiniGameEnd();
            StartEffect();
        }

        private void Start()
        {
            StartEffect();
        }

        public void StartEffect()
        {
            StartCoroutine(BrickEffect());
        }

        public void Stop()
        {
            if (activated)
            {
                StopCoroutine(AddDefenseRoutine);
                RemoveDefense();
            }
        }

        public IEnumerator AddingDefense()
        {
            TreeIcon.gameObject.SetActive(true);
            yield return new WaitForFixedUpdate();
            AddDefense();
            activated = true;
            yield return new WaitForSeconds(DefenseInterval);
            RemoveDefense();
            activated = false;
        }

        void AddDefense()
        {
            Debug.Log("Adding Defense" + triggers.Colliders.Count);

            foreach (var VARIABLE in triggers.Colliders)
            {

                var script = VARIABLE?.GetComponent<Brick>();
                if (script)
                {
                    script.SetDefence(script.DefensePoint + 3);
                    SurroundingBricks.Add(script);
                }
            }
        }

        private void OnDestroy()
        {
            RemoveDefense();
        }

        void RemoveDefense()
        {
            TreeIcon.gameObject.SetActive(false);
            foreach (var brick in SurroundingBricks)
            {
                if (brick != null)
                {

                    brick.SetDefence(brick.DefensePoint - Mathf.Clamp(3, 0, brick.DefensePoint));
                }
            }

            SurroundingBricks.Clear();
        }
    }
}