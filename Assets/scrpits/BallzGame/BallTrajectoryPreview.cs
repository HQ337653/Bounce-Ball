using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace BallzGame.Managers
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;

    namespace BallzGame.Managers
    {
        public class BallTrajectoryPreview : MonoBehaviour
        {
            [Header("References")]
            [SerializeField] private BallLauncher launcher;

            [SerializeField] private LineRenderer lineRenderer;

            [Header("Trajectory")]
            [SerializeField] private float maxDistance = 100f;

            [SerializeField] private float skin = 0.01f;

            private EventTrigger area;

            private EventTrigger.Entry pointerDownEntry;
            private EventTrigger.Entry dragEntry;
            private EventTrigger.Entry pointerUpEntry;

            private bool dragging;

            private Vector2 inputTarget;
            [SerializeField] int bounceTime;

            private void Awake()
            {
                if (launcher == null)
                    launcher = GetComponent<BallLauncher>();

                lineRenderer.positionCount = 0;
                lineRenderer.enabled = false;
            }

            private void Start()
            {
                area = launcher.clickableArea;

                SetupEventTrigger();
            }

            private void OnDestroy()
            {
                RemoveEventTrigger();
            }

            // =========================================================
            // EventTrigger
            // =========================================================

            private void SetupEventTrigger()
            {
                if (area == null)
                    return;

                pointerDownEntry = AddEventTrigger(
                    EventTriggerType.PointerDown,
                    OnPointerDown
                );

                dragEntry = AddEventTrigger(
                    EventTriggerType.Drag,
                    OnDrag
                );

                pointerUpEntry = AddEventTrigger(
                    EventTriggerType.PointerUp,
                    OnPointerUp
                );
            }

            private EventTrigger.Entry AddEventTrigger(
                EventTriggerType type,
                UnityEngine.Events.UnityAction<BaseEventData> callback
            )
            {
                EventTrigger.Entry entry = new EventTrigger.Entry
                {
                    eventID = type
                };

                entry.callback.AddListener(callback);

                area.triggers.Add(entry);

                return entry;
            }

            private void RemoveEventTrigger()
            {
                if (area == null)
                    return;

                if (pointerDownEntry != null)
                {
                    area.triggers.Remove(pointerDownEntry);
                    pointerDownEntry = null;
                }

                if (dragEntry != null)
                {
                    area.triggers.Remove(dragEntry);
                    dragEntry = null;
                }

                if (pointerUpEntry != null)
                {
                    area.triggers.Remove(pointerUpEntry);
                    pointerUpEntry = null;
                }
            }

            // =========================================================
            // Input
            // =========================================================

            private void OnPointerDown(BaseEventData data)
            {
                dragging = true;

                UpdateInputTarget();
                UpdateTrajectory();
            }

            private void OnDrag(BaseEventData data)
            {
                if (!dragging)
                    return;

                UpdateInputTarget();
                UpdateTrajectory();
            }

            private void OnPointerUp(BaseEventData data)
            {
                dragging = false;

                HideTrajectory();
            }

            private void UpdateInputTarget()
            {
                inputTarget =
                    GameManager.Instance.MainCamera.ScreenToWorldPoint(
                        Mouse.current.position.ReadValue()
                    );
            }

            // =========================================================
            // Trajectory
            // =========================================================

            private void UpdateTrajectory()
            {
                Vector2 startPosition =
                    launcher.transform.position;

                Vector2 direction =
                    inputTarget - startPosition;

                if (direction.sqrMagnitude < 0.0001f)
                {
                    HideTrajectory();
                    return;
                }

                direction.Normalize();

                List<Vector3> points = new()
                {
                    startPosition
                };

                Vector2 currentPosition = startPosition;

                int bounceCount = 0;

                while (bounceCount < bounceTime+1)
                {
                    RaycastHit2D hit =
                        GetSolidHit(
                            currentPosition,
                            direction,
                            maxDistance
                        );

                    // 没有碰到墙
                    // 直接画到 maxDistance，然后结束
                    if (hit.collider == null)
                    {
                        points.Add(
                            currentPosition +
                            direction * maxDistance
                        );

                        break;
                    }

                    // 画到碰撞点
                    points.Add(hit.point);

                    // 发生一次反弹
                    bounceCount++;

                    // =================================================
                    // 已经达到最大反弹次数
                    // 停在这里，不再延伸
                    // =================================================

                    if (bounceCount >= bounceTime+1)
                    {
                        break;
                    }

                    // =================================================
                    // 继续反弹
                    // =================================================

                    direction =
                        Vector2.Reflect(
                            direction,
                            hit.normal
                        ).normalized;

                    currentPosition =
                        hit.point +
                        direction * skin;
                }

                lineRenderer.positionCount = points.Count;
                lineRenderer.SetPositions(points.ToArray());

                lineRenderer.enabled = true;
            }

            // =========================================================
            // 获取 Ball 真正会撞到的 Collider
            // =========================================================

            private RaycastHit2D GetSolidHit(
                Vector2 origin,
                Vector2 direction,
                float distance
            )
            {
                RaycastHit2D[] hits =
                    Physics2D.RaycastAll(
                        origin,
                        direction,
                        distance
                    );

                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider == null)
                        continue;

                    // Trigger 不参与 Ball 反弹
                    if (hit.collider.isTrigger)
                        continue;

                    return hit;
                }

                return default;
            }

            // =========================================================
            // Hide
            // =========================================================

            private void HideTrajectory()
            {
                lineRenderer.positionCount = 0;
                lineRenderer.enabled = false;
            }
        }
    }
}