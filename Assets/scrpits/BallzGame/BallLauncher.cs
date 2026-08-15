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
        public float ElapsedTimeSinceLaunch ;
        [SerializeField]private EventTrigger clickableArea;
        int activeBalls;
        private Coroutine timer;
        public bool HasInput;
        private Vector2 inputTarget;

        private void Awake()
        {
            SetupEventTrigger();
        }

        // ✅ 自动绑定 EventTrigger（PointerUp）
        void SetupEventTrigger()
        {
            if (clickableArea == null)
                return;

            clickableArea.triggers.Clear();

            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener((data) =>
            {
                OnRelease();
            });

            clickableArea.triggers.Add(entry);
        }

        // ✅ 替代鼠标点击的方法（松手触发）
        public void OnRelease()
        {
            Debug.Log("On Release");
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(
                Mouse.current.position.ReadValue()
            );

            // 限制必须向上发射
            if (worldPos.y >= transform.position.y + 0.02f)
            {
                inputTarget = worldPos;
                HasInput = true;
            }
        }

        public IEnumerator BounceTimer()
        {
            ElapsedTimeSinceLaunch  = 0;
            while (true)
            {
                ElapsedTimeSinceLaunch  += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }

        private Coroutine waitforinput;
        public void WaitForInput()
        {
            HasInput = false;
        }

        public void StopListenToInput()
        {
            HasInput = false;
        }


        public IEnumerator Launch()
        {
           var target=inputTarget;
            timer = StartCoroutine(BounceTimer());
            Transform parent = GameManager.Instance.BallsParent;
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
                        Quaternion.identity,parent
                    );

                obj.Init(this,dir,GameManager.Instance.BallConfig);



                yield return new WaitForSeconds(GameManager.Instance.BallConfig.ShootInterval);
            }
            while (activeBalls > 0)
            {
                yield return null;
            }

            StopCoroutine(timer);
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
            ElapsedTimeSinceLaunch = 0f;

            HasInput = false;
        }



    }
}

