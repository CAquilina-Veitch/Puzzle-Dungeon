using System.Collections;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Dungeon
{
    public class DraggableRoomUI : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;

        private DungeonMapUIController controller => DungeonMapUIController.Instance;
        private readonly CompositeDisposable disposables = new();
        private Coroutine snapCoroutine;

        public DungeonRoomDefiner RoomDefiner { get; set; }
        public Vector2Int CurrentGridPosition => RoomDefiner.Definition.Shape.Position;
        public RectTransform RectTransform => rectTransform;

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
