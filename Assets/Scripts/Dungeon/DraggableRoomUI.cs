using System.Collections;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Dungeon
{
    public class DraggableRoomUI : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;

        private DungeonMapUIController controller;
        private readonly CompositeDisposable disposables = new();
        private Coroutine snapCoroutine;

        public DungeonRoomDefiner RoomDefiner { get; set; }
        public Vector2Int CurrentGridPosition { get; set; }
        public RectTransform RectTransform => rectTransform;

        public void Initialize(DungeonMapUIController mapController)
        {
            controller = mapController;

            //subscribe to position updates from the controller
            controller.RoomPositionUpdates
                .Where(update => update.room == this)
                .Subscribe(update =>
                {
                    if (update.snap)
                    {
                        //snap
                        if (snapCoroutine != null)
                            StopCoroutine(snapCoroutine);
                        snapCoroutine = StartCoroutine(LerpToPosition(update.position, 0.15f));
                    }
                    else
                    {
                        //slide with mouse
                        if (snapCoroutine != null)
                        {
                            StopCoroutine(snapCoroutine);
                            snapCoroutine = null;
                        }
                        rectTransform.anchoredPosition = update.position;
                    }
                })
                .AddTo(disposables);
        }

        private IEnumerator LerpToPosition(Vector2 targetPosition, float duration)
        {
            Vector2 startPosition = rectTransform.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                //ease outquad
                t = 1f - (1f - t) * (1f - t);

                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            rectTransform.anchoredPosition = targetPosition;
            snapCoroutine = null;
        }

        public void SubscribeToTileEvents(GameObject tile)
        {
            if (controller == null)
            {
                Debug.LogError("Controller is null. Make sure Initialize is called before subscribing to tile events.");
                return;
            }

            var eventTrigger = tile.GetComponent<EventTrigger>();
            if (eventTrigger == null)
                eventTrigger = tile.AddComponent<EventTrigger>();

            //mouse down
            var mouseDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            mouseDownEntry.callback.AddListener((eventData) =>
            {
                var pointerData = (PointerEventData)eventData;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    controller.BoundsBackground,
                    pointerData.position,
                    pointerData.pressEventCamera,
                    out Vector2 localPoint))
                {
                    controller.OnRoomMouseDown(this, localPoint);
                }
            });
            eventTrigger.triggers.Add(mouseDownEntry);

            //drag
            var dragEntry = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
            dragEntry.callback.AddListener((eventData) =>
            {
                var pointerData = (PointerEventData)eventData;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    controller.BoundsBackground,
                    pointerData.position,
                    pointerData.pressEventCamera,
                    out Vector2 localPoint))
                {
                    controller.OnRoomMouseDrag(localPoint);
                }
            });
            eventTrigger.triggers.Add(dragEntry);

            //mouseup
            var mouseUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            mouseUpEntry.callback.AddListener((eventData) =>
            {
                controller.OnRoomMouseUp();
            });
            eventTrigger.triggers.Add(mouseUpEntry);

            //end Drag
            var endDragEntry = new EventTrigger.Entry { eventID = EventTriggerType.EndDrag };
            endDragEntry.callback.AddListener((eventData) =>
            {
                controller.OnRoomMouseUp();
            });
            eventTrigger.triggers.Add(endDragEntry);
        }

        private void OnDestroy()
        {
            disposables?.Dispose();
        }
    }
}
