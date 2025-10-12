using System.Collections.Generic;
using R3;
using Runtime.Extensions;
using Scripts.Behaviours;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Dungeon
{
    public class DungeonMapUIController : SingletonBehaviour<DungeonMapUIController>
    {
        [SerializeField] private RectTransform tilesContainer;
        public RectTransform BoundsBackground => boundsBackground;
        [SerializeField] private RectTransform boundsBackground;
        [SerializeField] private AspectRatioFitter aspectRatioFitter;
        [SerializeField] private RoomUIView roomUIPrefab;
        [SerializeField] private GameObject tilePrefab;

        private Dictionary<DungeonRoomDefinition, RoomUIView> roomUIViews = new();
        private DungeonDefinition currentDungeon;
        public float TileSize => tileSize;
        private float tileSize;
        public Vector2Int DungeonBounds => dungeonBounds;
        private Vector2Int dungeonBounds;

        public Observable<(DraggableRoomUI room, Vector2 position, bool snap)> RoomPositionUpdates =>
            roomPositionUpdates;

        private readonly Subject<(DraggableRoomUI room, Vector2 position, bool snap)> roomPositionUpdates = new();

        private DraggableRoomUI currentlyDraggedRoom;


        protected override void OnAwake() =>
            DungeonLayoutManager.Instance.CurrentDungeon.Subscribe(OnCurrentDungeonChanged).AddTo(this);

        private void OnCurrentDungeonChanged(DungeonDefinition dungeon)
        {
            ClearUI();

            if (dungeon == null || tilesContainer == null || boundsBackground == null || aspectRatioFitter == null)
                return;

            currentDungeon = dungeon;
            dungeonBounds = dungeon.Bounds;
            if (dungeonBounds.x > 0 && dungeonBounds.y > 0)
                aspectRatioFitter.aspectRatio = (float)dungeonBounds.x / (float)dungeonBounds.y;

            Canvas.ForceUpdateCanvases();
            var bgRect = boundsBackground.rect;
            tileSize = Mathf.Min(bgRect.width / dungeonBounds.x, bgRect.height / dungeonBounds.y);

            foreach (var room in currentDungeon.rooms)
            {
                if (room?.Definition?.Shape == null)
                    continue;

                var roomUI = Instantiate(roomUIPrefab, boundsBackground);
                roomUI.name = $"Room_{room.name}";

                //positioning
                var roomRect = roomUI.RectTransform;
                roomRect.anchorMin = new Vector2(0.5f, 0.5f);
                roomRect.anchorMax = new Vector2(0.5f, 0.5f);
                roomRect.pivot = new Vector2(0.5f, 0.5f);
                roomRect.anchoredPosition = GridToUIPosition(room.Definition.StartPosition);

                roomUI.Initialize(room, this, tilePrefab);

                roomUIViews[room.Definition] = roomUI;
            }
        }

        private void ClearUI()
        {
            if (boundsBackground == null)
                return;

            foreach (Transform child in boundsBackground)
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);

            roomUIViews.Clear();
        }

        public Vector2 GridToUIPosition(Vector2Int gridPos)
        {
            float halfBoundsX = (dungeonBounds.x - 1) * 0.5f;
            float halfBoundsY = (dungeonBounds.y - 1) * 0.5f;

            return new Vector2(
                (gridPos.x - halfBoundsX) * tileSize,
                (gridPos.y - halfBoundsY) * tileSize
            );
        }

        public Vector2Int CalculateGridPosition(Vector2 uiPosition)
        {
            float halfBoundsX = (dungeonBounds.x - 1) * 0.5f;
            float halfBoundsY = (dungeonBounds.y - 1) * 0.5f;

            int gridX = Mathf.RoundToInt((uiPosition.x / tileSize) + halfBoundsX);
            int gridY = Mathf.RoundToInt((uiPosition.y / tileSize) + halfBoundsY);

            return new Vector2Int(
                Mathf.Clamp(gridX, 0, dungeonBounds.x - 1),
                Mathf.Clamp(gridY, 0, dungeonBounds.y - 1)
            );
        }

        private Vector2Int mouseOffset;

        public void OnRoomMouseDown(DraggableRoomUI room, Vector2 mousePosition)
        {
            currentlyDraggedRoom = room;
            var mouseAsGrid = CalculateGridPosition(mousePosition);
            mouseOffset = mouseAsGrid - room.CurrentGridPosition;
        }

        public void OnRoomMouseDrag(Vector2 mousePosition)
        {
            var mouseAsGrid = CalculateGridPosition(mousePosition);
            mouseAsGrid -= mouseOffset;


            var newPosIsValid = IsValidRoomPosition(currentlyDraggedRoom, mouseAsGrid);

            if (newPosIsValid)
            {
                if (mouseAsGrid == currentlyDraggedRoom.CurrentGridPosition) return;

                LerpPos(mouseAsGrid);
                currentlyDraggedRoom.RoomDefiner.Definition.SetPosition(mouseAsGrid);
            }

            else
            {
                var adjacentPos = currentlyDraggedRoom.CurrentGridPosition + (mouseAsGrid - currentlyDraggedRoom.CurrentGridPosition).Normalised();
                var adjacentTileIsValid = IsValidRoomPosition(currentlyDraggedRoom, adjacentPos);
                
                if (adjacentTileIsValid)
                {
                    if (adjacentPos == currentlyDraggedRoom.CurrentGridPosition) return;

                    LerpPos(adjacentPos);
                    currentlyDraggedRoom.RoomDefiner.Definition.SetPosition(adjacentPos);
                }
            }
            
        }

        private void LerpPos(Vector2Int newTilePos)
        {
            var newPos = GridToUIPosition(newTilePos);
            
            currentlyDraggedRoom.RectTransform.anchoredPosition = newPos;
            
        }

        public void OnRoomMouseUp()
        {
            
        }

        private bool IsValidRoomPosition(DraggableRoomUI room, Vector2Int newPos)
        {
            if (room?.RoomDefiner?.Definition?.Shape == null || currentDungeon == null)
                return false;

            var shape = room.RoomDefiner.Definition.Shape;
            var shapeCoordinates = shape.ShapeCoordinates;

            foreach (var coord in shapeCoordinates)
            {
                var absolutePos = newPos + coord;

                if (absolutePos.x < 0 || absolutePos.x >= dungeonBounds.x ||
                    absolutePos.y < 0 || absolutePos.y >= dungeonBounds.y)
                    return false;
            }

            foreach (var otherRoom in currentDungeon.rooms)
            {
                if (otherRoom == room.RoomDefiner)
                    continue;

                if (otherRoom?.Definition?.Shape == null)
                    continue;

                var otherShape = otherRoom.Definition.Shape;
                var otherPositions = otherShape.CoordinatesAsPosition;

                foreach (var coord in shapeCoordinates)
                {
                    var absolutePos = newPos + coord;

                    foreach (var otherPos in otherPositions)
                        if (absolutePos == otherPos)
                            return false;
                }
            }

            return !(Vector2Int.Distance(newPos, room.CurrentGridPosition) > 1);
        }

    }
}
