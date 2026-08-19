using System.Collections;
using System.Collections.Generic;
using BallzGame.Balls;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BallzGame.Managers
{
    public class BallLauncher : MonoBehaviour
    {
        [SerializeField]private List<Ball> ballPrefabs;
        public Ball InitialBall;
        public float ElapsedTimeSinceLaunch ;
        public EventTrigger clickableArea;
        int activeBalls;
        private Coroutine timer;
        public bool HasInput;
        private Vector2 inputTarget;
        public int BallCount;
        public Dictionary<BallData,int> ballDatas=new();
        public Button SkipButton;
        private void Awake()
        {
            SetupEventTrigger();
        }


        // ✅ 自动绑定 EventTrigger（PointerUp）
        void SetupEventTrigger()
        {
            if (clickableArea == null)
                return;

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
            Vector2 worldPos = GameManager.Instance.MainCamera .ScreenToWorldPoint(
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

            // 初始化 ballPrefabs 列表（如果尚未创建）
            if (ballPrefabs == null)
            {
                ballPrefabs = new List<Ball>();
            }

            // 遍历每个球
            foreach (Ball ball in balls)
            {
                // 将球添加到预制体列表（根据你的原有逻辑保留）
                ballPrefabs.Add(ball);

                // 将球对应的 BallData 加入字典并计数
                if (!ballDatas.TryAdd(ball.Data, 1))
                {
                    ballDatas[ball.Data]++;
                }
            }

            // 更新总球数：实际添加的球的数量
            BallCount += balls.Count;

        }

        public void Reset()
        {
            ballPrefabs.Clear();
            BallCount = 0;
            ballDatas.Clear();
            ballPrefabs.Add(InitialBall);
            // 将球对应的 BallData 加入字典并计数
            if (!ballDatas.TryAdd(InitialBall.Data, 1))
            {
                ballDatas[InitialBall.Data]++;
            }
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

