using System;
using System.Collections;
using BallzGame.Bricks;
using BallzGame.Managers;
using BallzGame.Minigame.SnakeGame;
using GameMeta;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

namespace BallzGame.Minigame
{
    public class Snake : IFeverGame
    {
        private Action onEnd;
        public SnakeController snake;

        private FeverGameContext context;

        public GameObject WallPrefab;
        public GameObject FoodPrefab;
        private (GameObject, bool)[,] visuals;
        public Transform CameraPosition;

        [SerializeField]
        private int hpThreshold;

        public Gradient TimerColor;
        public GameObject background;
        public GameObject[] EdgeWalls;
        public TextMeshProUGUI TimerText;
        public Image TimerImage;
        public int GameMaxTime = 10;

        public override void StartGame(FeverGameContext context, Action onEnd)
        {
            MainMenu.Instance.InGamePanel.SetBallzGameUI(false);
            this.onEnd = onEnd;
            Debug.Log("Starting Snake Game");

            gameObject.SetActive(true);
            this.context = context;
            snake.gameObject.SetActive(false);
            // 只初始化数组（不生成）
            InitVisualArrayOnly();

            // 播放开场动画（并行）
            StartCoroutine(PlayStartAnimation());
            snake.moveInterval = 0.35f;
            TimerImage.color = TimerColor.Evaluate(1);
        }

        public Coroutine EndGameCoroutine;

        IEnumerator Timer()
        {
            float time = GameMaxTime;
            while (time > 0)
            {
                time -= Time.deltaTime;
                yield return null;
                TimerText.text = ((int)time).ToString();
                float ratio = Mathf.Sin(time / GameMaxTime * Mathf.PI * 0.5f);
                snake.moveInterval = Mathf.Lerp(0.01f, 0.35f, ratio);
                TimerImage.color = TimerColor.Evaluate(time / GameMaxTime);
            }

            TryEndGame();
        }

        private void TryEndGame()
        {
            if (EndGameCoroutine == null)
            {
                snake.Die();
                EndGameCoroutine = StartCoroutine(EndGame());
            }

        }

        private void Start()
        {
            snake.OnMoveToGrid += (Vector2Int val) => { SnakeWentTo(val.x, val.y); };
        }

        // =========================
        // 🎬 开场动画
        // =========================
        IEnumerator PlayStartAnimation()
        {
            Coroutine bg = StartCoroutine(AnimateBackground());
            Coroutine bricks = StartCoroutine(SpawnBricksSequentially());
            Coroutine edge = StartCoroutine(SpawnEdgeWalls());
            Coroutine Camera = StartCoroutine(MoveCamera(CameraPosition));
            yield return bg;
            yield return bricks;
            yield return edge;
            yield return Camera;
            Debug.Log("Start Animation Finished");
        }

        IEnumerator MoveCamera(Transform target, float duration = 0.6f)
        {
            Transform cam = GameManager.Instance.MainCamera.transform;

            Vector3 startPos = cam.position;
            Quaternion startRot = cam.rotation;

            Vector3 targetPos = target.position;
            Quaternion targetRot = target.rotation;

            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                // 👉 缓动（更自然）
                t = Mathf.SmoothStep(0, 1, t);

                cam.position = Vector3.Lerp(startPos, targetPos, t);
                cam.rotation = Quaternion.Slerp(startRot, targetRot, t);

                yield return null;
            }

            cam.position = targetPos;
            cam.rotation = targetRot;
        }


        IEnumerator SpawnEdgeWalls()
        {
            int w = context.Grid.GetLength(0);
            int h = context.Grid.GetLength(1);

            float delay = 0.015f;

            int minX = -1;
            int maxX = w;
            int minY = -1;
            int maxY = h;

            EdgeWalls = new GameObject[(w + h + 2) * 2];
            int index = 0;

            // =========================
            // ➡️ 底边 (-1,-1) → (w,-1)
            // =========================
            for (int x = minX; x <= maxX; x++)
            {
                SpawnEdge(x, minY, ref index);
                yield return new WaitForSeconds(delay);
            }

            // =========================
            // ⬆️ 右边 (w,0) → (w,h)
            // =========================
            for (int y = 0; y <= maxY; y++)
            {
                SpawnEdge(maxX, y, ref index);
                yield return new WaitForSeconds(delay);
            }

            // =========================
            // ⬅️ 上边 (w-1,h) → (-1,h)
            // =========================
            for (int x = maxX - 1; x >= minX; x--)
            {
                SpawnEdge(x, maxY, ref index);
                yield return new WaitForSeconds(delay);
            }

            // =========================
            // ⬇️ 左边 (-1,h-1) → (-1,0)
            // =========================
            for (int y = maxY - 1; y >= 0; y--)
            {
                SpawnEdge(minX, y, ref index);
                yield return new WaitForSeconds(delay);
            }
        }

        // 背景缩放动画
        IEnumerator AnimateBackground()
        {
            float duration = 0.5f;
            float time = 0f;

            background.transform.localScale = Vector3.zero;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                float scale = Mathf.Lerp(0, 50, t);

                // 👉 加一点弹性（可删）
                scale += Mathf.Sin(t * Mathf.PI) * 2f;

                background.transform.localScale = new Vector3(scale, scale, 1);

                yield return null;
            }

            background.transform.localScale = new Vector3(50, 50, 1);
        }

        // 砖块逐个生成
        IEnumerator SpawnBricksSequentially()
        {
            int w = context.Grid.GetLength(0);
            int h = context.Grid.GetLength(1);

            float delay = 0.05f; // 初始慢一点
            float minDelay = 0.005f; // 最快限制
            float decay = 0.95f; // 每次乘这个（越小加速越快）

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Brick brick = context.Grid[x, y];

                    if (brick == null)
                    {
                        visuals[x, y] = (null, false);
                        continue;
                    }

                    brick.VisualEffect.SetVisibility(false);

                    GameObject obj;

                    if (brick.Hp > hpThreshold)
                    {
                        obj = Instantiate(
                            FoodPrefab,
                            brick.transform.position,
                            Quaternion.identity,
                            transform
                        );

                        var text = obj.GetComponentInChildren<TextMeshPro>();
                        if (text != null)
                            text.text = (brick.Hp - hpThreshold).ToString();

                        visuals[x, y] = (obj, false);
                    }
                    else
                    {
                        obj = Instantiate(
                            WallPrefab,
                            brick.transform.position,
                            Quaternion.identity,
                            transform
                        );

                        visuals[x, y] = (obj, true);
                    }

                    obj.SetActive(true);

                    obj.transform.localScale = Vector3.zero;
                    StartCoroutine(ScalePop(obj.transform));

                    // 👇 动态加速
                    yield return new WaitForSeconds(delay);
                    delay = Mathf.Max(minDelay, delay * decay);
                }
            }

            Vector2Int start = FindStartPosition();

            yield return new WaitForSeconds(0.1f);
            snake.gameObject.SetActive(true);
            snake.Init(context, start);
            yield return new WaitForSeconds(0.2f);
            snake.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            snake.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            snake.StartMove();
            StartCoroutine(Timer());
        }

        void SpawnEdge(int x, int y, ref int index)
        {
            Vector3 pos = new Vector3(x, -y, 0);

            GameObject wall = Instantiate(
                WallPrefab,
                pos,
                Quaternion.identity,
                transform
            );

            wall.SetActive(true);

            // 👉 记录
            if (EdgeWalls != null && index < EdgeWalls.Length)
            {
                EdgeWalls[index++] = wall;
            }

            // 👉 动画（pop）
            wall.transform.localScale = Vector3.zero;
            StartCoroutine(ScalePop(wall.transform));
        }

        IEnumerator ScalePop(Transform t)
        {
            float time = 0f;
            float duration = 0.15f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float s = Mathf.Lerp(0, 1, time / duration);
                t.localScale = Vector3.one * s;
                yield return null;
            }

            t.localScale = Vector3.one;
        }

        // =========================
        // 🧱 初始化数组
        // =========================
        void InitVisualArrayOnly()
        {
            int w = context.Grid.GetLength(0);
            int h = context.Grid.GetLength(1);

            visuals = new (GameObject, bool)[w, h];
        }

        // =========================
        // 🐍 蛇逻辑
        // =========================
        public void SnakeWentTo(int x, int y)
        {
            int w = context.Grid.GetLength(0);
            int h = context.Grid.GetLength(1);

            Vector2Int pos = new Vector2Int(x, y);


            if (x >= w || x < 0 || y >= h || y < 0 || (snake.IsHitTail(pos)))
            {

                TryEndGame();
                return;
            }




            var tile = visuals[x, y];


            if (tile.Item1 == null)
                return;


            if (tile.Item2 || snake.IsHitTail(pos))
            {

                TryEndGame();
            }
            else
            {
                var text = tile.Item1.GetComponentInChildren<TextMeshPro>();

                if (text == null)
                    return;


                int hp = int.Parse(text.text);
                int half = Mathf.CeilToInt((float)hp / 2);

                snake.AddBody();
                GameManager.DoDamageText(tile.Item1.transform.position, half);
                hp -= half;
                if (hp <= 0)
                {
                    GameObject wall = Instantiate(
                        WallPrefab,
                        tile.Item1.transform.position,
                        Quaternion.identity,
                        transform
                    );


                    wall.SetActive(true);


                    Destroy(tile.Item1);


                    visuals[x, y] = (wall, true);
                }
                else
                {
                    text.text = hp.ToString();

                    visuals[x, y] = (tile.Item1, false);
                }
            }
        }

        // =========================
        // 📍 找出生点
        // =========================
        Vector2Int FindStartPosition()
        {
            int w = context.Grid.GetLength(0);
            int h = context.Grid.GetLength(1);

            for (int x = w / 2; x < w; x++)
            {
                for (int y = h / 2; y < h; y++)
                {
                    if (context.Grid[x, y] == null)
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }

            return new Vector2Int(0, 0);
        }



        // =========================
        // 🛑 结束游戏
        // =========================
        IEnumerator EndGame()
        {
            if (visuals != null)
            {
                int w = visuals.GetLength(0);
                int h = visuals.GetLength(1);
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        var tile = visuals[x, y];

                        if (tile.Item1 != null)
                        {
                            tile.Item1.SetActive(false);

                        }
                    }
                }

                if (EdgeWalls != null)
                {

                    for (int i = 0; i < EdgeWalls.Length; i++)
                    {
                        if (EdgeWalls[i] != null)
                        {
                            Destroy(EdgeWalls[i]);
                            EdgeWalls[i] = null;
                        }
                    }
                }

                background.transform.localScale = new Vector3(0, 0, 1);
                StartCoroutine(MoveCamera(GameManager.Instance.BallzCameraPosition));
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        var tile = visuals[x, y];
                        var brick = context.Grid[x, y];
                        if (brick != null)
                        {
                            brick.VisualEffect.SetVisibility(true);
                        }

                        if (tile.Item1 != null)
                        {
                            // 👉 如果是食物（不是墙）
                            if (!tile.Item2)
                            {
                                var text = tile.Item1.GetComponentInChildren<TextMeshPro>();

                                if (text != null)
                                {
                                    int current = int.Parse(text.text);

                                    int original = brick.Hp - hpThreshold;

                                    int damage = original - current;

                                    yield return new WaitForSeconds(0.05f);
                                    if (damage > 0)
                                    {
                                        brick.TakeDamage(damage,this);

                                        GameManager.DoDamageText(brick.transform.position, damage);

                                    }
                                }

                            }
                            else
                            {
                                int current = 0;

                                int original = brick.Hp - hpThreshold;

                                int damage = original - current;

                                yield return new WaitForSeconds(0.05f);
                                if (damage > 0)
                                {
                                    brick.TakeDamage(damage,this);

                                    GameManager.DoDamageText(brick.transform.position, damage);

                                }
                            }

                            Destroy(tile.Item1);
                        }

                    }
                }
            }

            visuals = null;

            yield return null;
            Debug.Log("End Snake Game");
            if (snake != null)
            {
                snake.Stop();
            }
            MainMenu.Instance.InGamePanel.SetBallzGameUI(true);

            onEnd?.Invoke();
            EndGameCoroutine = null;
            gameObject.SetActive(false);
        }

    }
}