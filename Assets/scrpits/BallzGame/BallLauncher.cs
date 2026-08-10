using System;
using System.Collections;
using System.Collections.Generic;
using BallzGame.Balls;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace BallzGame.Managers
{
    public class BallLauncher : MonoBehaviour
    {
        public List<Ball> ballPrefabs;
        public Ball InitialBall;

        public float shootInterval = 0.1f;
        public float speed = 10f;


        public EventTrigger Clickable;

        int activeBalls;

        public float BouncedTime;
        public float SpeedIncrease;

        private Coroutine timer;

        // ✅ 新增：输入缓存
        private bool hasInput;
        private Vector2 inputTarget;

        private void Awake()
        {
            SetupEventTrigger();
        }

        // ✅ 自动绑定 EventTrigger（PointerUp）
        void SetupEventTrigger()
        {
            if (Clickable == null)
                return;

            Clickable.triggers.Clear();

            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener((data) =>
            {
                OnRelease();
            });

            Clickable.triggers.Add(entry);
        }

        // ✅ 替代鼠标点击的方法（松手触发）
        public void OnRelease()
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(
                Mouse.current.position.ReadValue()
            );

            // 限制必须向上发射
            if (worldPos.y >= transform.position.y + 0.02f)
            {
                inputTarget = worldPos;
                hasInput = true;
            }
        }

        public IEnumerator BounceTimer()
        {
            BouncedTime = 0;
            while (true)
            {
                BouncedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }

        // GameManager 调用
        public IEnumerator StartWaitForInput()
        {

            hasInput = false;

            // ✅ 等待 EventTrigger 输入
            while (!hasInput)
            {
                if (GameManager.Instance.feverController.FeverClicked)
                {
                    GameManager.Instance.feverController.FeverClicked = false;

                    GameManager.Instance.Dofever = true;
                    yield break;
                }

                yield return null;
            }

            Vector2 target = inputTarget;

            timer = StartCoroutine(BounceTimer());

            // 开始发球
            yield return StartCoroutine(ShootRoutine(target));

            // 等所有球回来
            while (activeBalls > 0)
            {
                yield return null;
            }

            StopCoroutine(timer);
        }

        IEnumerator ShootRoutine(Vector2 target)
        {

            Vector2 dir =
                (target - (Vector2)transform.position).normalized;

            activeBalls = ballPrefabs.Count;

            for (int i = 0; i < ballPrefabs.Count; i++)
            {
                var prefab = ballPrefabs[i];

                var obj =
                    Instantiate(
                        prefab,
                        transform.position,
                        Quaternion.identity
                    );

                obj.Init(this, speed);

                Rigidbody2D rb =
                    obj.GetComponent<Rigidbody2D>();

                rb.linearVelocity = dir * speed;

                yield return new WaitForSeconds(shootInterval);
            }
        }

        public void OnBallReturned(Ball ball)
        {
            activeBalls--;
        }

        public void AddBalls(List<Ball> balls)
        {
            if (balls == null) return;

            if (ballPrefabs == null)
            {
                ballPrefabs = new List<Ball>();
            }

            ballPrefabs.AddRange(balls);
        }

        public void Reset()
        {
            ballPrefabs.Clear();
            ballPrefabs.Add(InitialBall);

            if (timer != null)
            {
                StopCoroutine(timer);
                timer = null;
            }

            StopAllCoroutines();

            activeBalls = 0;
            BouncedTime = 0f;

            hasInput = false;
        }
    }
}

