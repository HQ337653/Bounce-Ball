using System.Collections;
using System.Collections.Generic;
using BallzGame.Bricks;
using BallzGame.Bricks.SpecialBricks;
using UnityEngine;
using Utils;

public class BlackHoleBrick : SpecialBrick
    {
        [SerializeField]private SpriteRenderer BlackholeSprite;
        [SerializeField]private TriggerCollector triggers;
        public List<Brick> SurroundingBricks = new List<Brick>();
        [SerializeField]private float DefenseInterval;
        [SerializeField]private float WaitInterval;
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
            BlackholeSprite.gameObject.SetActive(true);
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
                if(!VARIABLE)
                    return;
                var script = VARIABLE?.GetComponent<Brick>();
                if (script)
                {
                    script.AddStatus(Brick.BrickStatus.DisableEffect);
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
            BlackholeSprite.gameObject.SetActive(false);
            foreach (var brick in SurroundingBricks)
            {
                if (brick != null)
                {

                    brick.removeStatus(Brick.BrickStatus.DisableEffect);
                }
            }

            SurroundingBricks.Clear();
        }
    }
