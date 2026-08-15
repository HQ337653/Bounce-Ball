using System.Collections;
using BallzGame.Balls;
using BallzGame.Bricks;
using BallzGame.InventorySystem;
using BallzGame.Managers.Shop;
using BallzGame.Minigame;
using GameMeta;
using UnityEngine;

namespace BallzGame.Managers
{


    public class GameManager : MonoBehaviour
    {

        [Header("controllers")]
        public BrickSpawner spawner;
        public BallLauncher launcher;
        public FeverController feverController;
        public ShopController shopController;
        public Inventory inventory;
        public BallExtraDamage BallExtraDamageController;




        public static GameManager Instance { get; private set; }
        [Header("scene reference")]
        public Transform BallzCameraPosition;
        public Transform BricksParent;
        public Transform BallsParent;
        public Transform VisualEffectsParent;
        public Transform BrickPointTarget;
        public Camera MainCamera;
        [Header("game setting")]
        public int width = 7;
        public int height = 15;
        [Header("current game status")]
        public Brick[,] grid;
        public int level = 1;
        public State state;
        public GameResultPanel.GameResult CurrentResult;
        [Header("Configs")]
        public BallSystemConfig BallConfig;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = FindAnyObjectByType<GameManager>();
            }
            var gamePanel = MainMenu.Instance.InGamePanel;
            gamePanel.DieSubPanelConfirmButton.onClick.AddListener(BackToMainMenuButtonClicked);
        }
        public void NewGame()
        {
            ResetAllController();
            if (gameLoop!=null)
            {
                StopCoroutine(gameLoop);
            }
            MainMenu.Instance.Goto(MainMenu.CurrentPanel.InGame);
            CurrentResult=new();
            grid = new Brick[width, height];
            gameLoop=StartCoroutine(GameLoop());
        }

        private Coroutine gameLoop;
        IEnumerator GameLoop()
        {
            state = State.TrySpawnRow;
            while (true)
            {
                switch (state)
                {
                    case State.WaitForFeverOrLaunchInput:
                        shopController.RefreshShopItems();
                        launcher.WaitForInput();
                        feverController.WaitForInput();
                        yield return new WaitUntil(() => launcher.HasInput || feverController.FeverClicked);
                        if (launcher.HasInput)
                        {
                            state = State.ShootBall;
                        }
                        else
                        {
                            state = State.Fever;
                        }
                        feverController.StopListenToInput();
                        launcher.StopListenToInput();
                        break;
                    case State.ShootBall:
                        yield return new WaitForEndOfFrame();
                        yield return StartCoroutine( launcher.Launch() );
                        state = State.TrySpawnRow;
                        break;
                    case State.Fever:
                        NotifyBricksMiniGameStart();
                        yield return StartCoroutine(feverController.StartFeverGame(GetContext()));
                        NotifyBricksMiniGameEnd();
                        state = State.WaitForFeverOrLaunchInput;
                        break;
                    case State.TrySpawnRow:
                        level++;
                        bool gameOver = !TryMoveBricksDown();

                        if (gameOver)
                        {
                            state = State.GameOver;
                        }
                        else
                        {
                            SpawnTopRow();
                            state = State.WaitForFeverOrLaunchInput;
                        }
                        break;
                    case State.GameOver:
                        MainMenu.Instance.InGamePanel.DieSubPanel.SetActive(true);
                        yield break;

                }

                yield return null;
            }
        }



        void BackToMainMenuButtonClicked()
        {
            if (state == State.GameOver)
            {
                CurrentResult.BallsCount  = launcher.ballPrefabs.Count;
                level = 1;
                ClearAllBricks();
                MainMenu.Instance.InGamePanel.DieSubPanel.SetActive(false);
                MainMenu.Instance.GameResultPanel.SetResult(CurrentResult);
                MainMenu.Instance.Goto(MainMenu.CurrentPanel.GameResult);
                ResetAllController();
            }
        }


        private void ResetAllController()
        {
            feverController.Reset();
            launcher.Reset();
            BallExtraDamageController.Reset();
        }

        void ClearAllBricks()
        {
            if (grid == null)
                return;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Brick brick = grid[x, y];

                    if (brick != null)
                    {
                        Destroy(brick.gameObject);
                        grid[x, y] = null;
                    }
                }
            }
        }


        void SpawnTopRow()
        {
            var bricks = spawner.SpawnRow(level, width);

            for (int x = 0; x < width; x++)
            {
                Brick brick = bricks[x];

                grid[x, 0] = brick;

                if (brick != null)
                {
                    brick.transform.position = new Vector3(x, 0, 0);
                }
            }
        }




        bool TryMoveBricksDown()
        {
            bool StillHaveSpace;
            for (int x = 0; x < width; x++)
            {
                if (grid[x, height - 1])
                {
                    StillHaveSpace = false;
                    return StillHaveSpace;
                }
            }

            // 再移动
            for (int y = height - 2; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    Brick brick = grid[x, y];

                    if (brick == null)
                        continue;

                    grid[x, y + 1] = brick;
                    grid[x, y] = null;

                    brick.transform.position =
                        new Vector3(x, -(y + 1), 0);
                    brick.OnRowMoved();
                }
            }

            StillHaveSpace = true;
            return StillHaveSpace;
        }

        void NotifyBricksMiniGameStart()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y])
                    {
                        grid[x, y].OnMiniGameStart();
                    }
                }
            }
        }


        void NotifyBricksMiniGameEnd()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] != null)
                    {
                        grid[x, y].OnMiniGameEnd();
                    }
                }
            }
        }
        private FeverGameContext GetContext()
        {
            var context = new FeverGameContext();
            context.CurrentLevel = level;
            context.Grid = grid;
            return context;
        }


        [ContextMenu("Print Grid")]
        private void PrintGrid()
        {
            int w = grid.GetLength(0);
            int h = grid.GetLength(1);

            string result = "";

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (grid[x, y] == null)
                    {
                        result += "0\t";
                    }
                    else
                    {
                        result += grid[x, y].Hp + "\t";
                    }
                }

                result += "\n";
            }

            Debug.Log(result);
        }
        public enum State
        {
            TrySpawnRow,
            WaitForFeverOrLaunchInput,
            Fever,
            GameOver,
            ShootBall,
        }
    }


}