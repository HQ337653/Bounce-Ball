using System;
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
        public bool Dofever;
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
        }
        public void NewGame()
        {
            MainMenu.Instance.Goto(MainMenu.CurrentPanel.InGame);
            CurrentResult=new();
            grid = new Brick[width, height];

            StartCoroutine(GameLoop());
        }
        private FeverGameContext GetContext()
        {
            var context = new FeverGameContext();
            context.CurrentLevel = level;
            context.Grid = grid;
            return context;
        }

        IEnumerator GameLoop()
        {
            while (true)
            {
                switch (state)
                {

                    case State.GameOver:
                        GameOver();
                        yield break;

                    case State.Fever:
                        NotifyBricksMiniGameStart();
                        yield return StartCoroutine(
                            feverController.StartFeverGame(GetContext())
                        );
                        NotifyBricksMiniGameEnd();

                        state = State.WaitForInputAndComeback;
                        break;

                    case State.SpawnRow:
                        SpawnRowState();

                        state = State.WaitForInputAndComeback;
                        break;

                    case State.WaitForInputAndComeback:
                        shopController.RefreshShopItems();
                        yield return StartCoroutine(
                            launcher.StartWaitForInput()
                        );

                        level++;
                        if (Dofever)
                        {
                            state = State.Fever;
                            Dofever = false;
                            break;
                        }

                        bool isGameOver = MoveBricksDown();

                        if (isGameOver)
                        {
                            state = State.GameOver;
                        }
                        else
                        {
                            state = State.SpawnRow;
                        }

                        break;
                }

                yield return null;
            }
        }

        private void GameOver()
        {
            var gamePanel = MainMenu.Instance.InGamePanel;
            gamePanel.DieSubPanel.SetActive(true);
            gamePanel.DieSubPanelConfirmButton.onClick.AddListener(BackToMainMenu);

            void BackToMainMenu()
            {
                CurrentResult.BallsCount  = launcher.ballPrefabs.Count;


                level = 1;
                Dofever = false;
                state = State.SpawnRow;
                ClearBricks();
                gamePanel.DieSubPanel.SetActive(false);
                gamePanel.DieSubPanelConfirmButton.onClick.RemoveListener(BackToMainMenu);

                MainMenu.Instance.GameResultPanel.SetResult(CurrentResult);
                MainMenu.Instance.Goto(MainMenu.CurrentPanel.GameResult);
            }
        }

        private void ResetAllController()
        {
            feverController.Reset();
            launcher.Reset();
            BallExtraDamageController.Reset();
            ClearBricks();
        }

        void ClearBricks()
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


        void SpawnRowState()
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

        public void SetBrick(int x, int y, Brick brick)
        {
            grid[x, y] = brick;
        }



        bool MoveBricksDown()
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, height - 1] != null)
                {
                    return true; // GameOver
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

            return false;
        }

        void NotifyBricksMiniGameStart()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] != null)
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

        [ContextMenu("Print Grid")]
        public void PrintGrid()
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
                        result += grid[x, y].hp + "\t";
                    }
                }

                result += "\n";
            }

            Debug.Log(result);
        }

    }public enum State
    {
        SpawnRow,
        WaitForInputAndComeback,
        Fever,
        GameOver
    }


}