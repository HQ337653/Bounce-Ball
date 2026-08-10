using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BallzGame.Minigame.SnakeGame
{
    public class SnakeController : MonoBehaviour
    {
        public float moveInterval = 0.15f;


        private Vector2Int direction = Vector2Int.right;
        private Vector2Int nextDirection;


        private FeverGameContext context;


        private Vector2Int currentPosition;


        private bool active;

        private bool hasMoved;


        private Coroutine moveCoroutine;


        public Action<Vector2Int> OnMoveToGrid;


        public GameObject SnakeHeadVisual;


        // =========================
        // Tail System
        // =========================


        public GameObject SnakeTailPrefab;


        // 只保存尾巴，不包含头
        private List<Vector2Int> tailPositions =
            new List<Vector2Int>();


        // 保存生成出来的尾巴物体
        private List<GameObject> tailVisuals =
            new List<GameObject>();



        // 外部查看撞尾
        // 不包含头
        public IReadOnlyList<Vector2Int> TailPositions
        {
            get { return tailPositions; }
        }




        public void Init(
            FeverGameContext ctx,
            Vector2Int startPosition
        )
        {
            gameObject.SetActive(true);


            context = ctx;


            currentPosition = startPosition;


            direction = Vector2Int.right;
            nextDirection = direction;


            hasMoved = false;


            active = true;


            // 清理旧尾巴
            ClearTail();



            transform.position =
                GridToWorld(currentPosition);



            UpdateHeadRotation();



            moveCoroutine = StartCoroutine(MoveRoutine());
        }

        public void StartMove()
        {

            moveCoroutine = StartCoroutine(MoveRoutine());
        }




        IEnumerator MoveRoutine()
        {
            while (active)
            {
                yield return new WaitForSeconds(moveInterval);

                Move();
            }
        }





        void Update()
        {
            if (!active)
                return;


            HandleInput();
        }





        void HandleInput()
        {
            Keyboard keyboard = Keyboard.current;


            if (keyboard == null)
                return;



            if ((keyboard.upArrowKey.wasPressedThisFrame ||
                 keyboard.wKey.wasPressedThisFrame)
                &&
                (!hasMoved || direction != Vector2Int.up))
            {
                nextDirection = Vector2Int.down;
            }



            if ((keyboard.downArrowKey.wasPressedThisFrame ||
                 keyboard.sKey.wasPressedThisFrame)
                &&
                (!hasMoved || direction != Vector2Int.down))
            {
                nextDirection = Vector2Int.up;
            }



            if ((keyboard.leftArrowKey.wasPressedThisFrame ||
                 keyboard.aKey.wasPressedThisFrame)
                &&
                (!hasMoved || direction != Vector2Int.right))
            {
                nextDirection = Vector2Int.left;
            }



            if ((keyboard.rightArrowKey.wasPressedThisFrame ||
                 keyboard.dKey.wasPressedThisFrame)
                &&
                (!hasMoved || direction != Vector2Int.left))
            {
                nextDirection = Vector2Int.right;
            }
        }






        void Move()
        {
            if (!active)
                return;



            direction = nextDirection;


            hasMoved = true;



            Vector2Int oldHead =
                currentPosition;



            currentPosition += direction;



            // =====================
            // 移动尾巴
            // =====================

            MoveTail(oldHead);



            transform.position =
                GridToWorld(currentPosition);



            UpdateHeadRotation();

            OnMoveToGrid?.Invoke(currentPosition);
        }

        void MoveTail(Vector2Int previousHead)
        {

            if (tailPositions.Count == 0)
                return;

            // 从最后往前移动

            for (int i = tailPositions.Count - 1; i > 0; i--)
            {
                tailPositions[i] =
                    tailPositions[i - 1];
            }


            // 第一节尾巴去旧头位置

            tailPositions[0] =
                previousHead;



            // 更新视觉

            for (int i = 0; i < tailVisuals.Count; i++)
            {
                tailVisuals[i].transform.position =
                    GridToWorld(tailPositions[i]);
            }
        }


        // =========================
        // 增加身体
        // =========================

        public void AddBody()
        {
            Vector2Int pos;


            if (tailPositions.Count == 0)
            {
                pos =
                    currentPosition - direction;
            }
            else
            {
                pos =
                    tailPositions[
                        tailPositions.Count - 1
                    ];
            }

            tailPositions.Add(pos);
            GameObject tail = null;

            if (SnakeTailPrefab != null)
            {
                tail = Instantiate(
                    SnakeTailPrefab,
                    GridToWorld(pos),
                    Quaternion.identity
                );


                tail.SetActive(true);


                tailVisuals.Add(tail);
            }
        }



        public bool IsHitTail(Vector2Int pos)
        {
            return tailPositions.Contains(pos);
        }


        void UpdateHeadRotation()
        {
            if (SnakeHeadVisual == null)
                return;


            float angle = 0;


            if (direction == Vector2Int.right)
                angle = 0;

            else if (direction == Vector2Int.up)
                angle = -90;

            else if (direction == Vector2Int.left)
                angle = 180;

            else if (direction == Vector2Int.down)
                angle = 90;



            SnakeHeadVisual.transform.rotation =
                Quaternion.Euler(
                    0,
                    0,
                    angle
                );
        }


        Vector3 GridToWorld(Vector2Int pos)
        {
            return new Vector3(
                pos.x,
                -pos.y,
                0
            );
        }



        public void ClearTail()
        {
            for (int i = 0; i < tailVisuals.Count; i++)
            {
                if (tailVisuals[i] != null)
                {
                    Destroy(tailVisuals[i]);
                }
            }

            tailVisuals.Clear();
            tailPositions.Clear();
        }


        public void Die()
        {
            Debug.Log("SnakeDie");
            if (!active)
                return;
            active = false;
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }

            ClearTail();
            gameObject.SetActive(false);
        }

        public void Stop()
        {
            active = false;
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }

            ClearTail();
            gameObject.SetActive(false);
        }
    }
}