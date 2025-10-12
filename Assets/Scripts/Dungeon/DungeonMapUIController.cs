using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Dungeon
{
    public class DungeonMapUIController : MonoBehaviour
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

        //dragging
        private DraggableRoomUI currentlyDraggedRoom;
        private Vector2 dragStartMousePosition;
        private Vector2 lastMousePosition;
        private Vector2Int dragStartGridPosition;
        private Vector2Int currentValidGridPosition;
        private enum DragAxis { None, Horizontal, Vertical }
        private DragAxis lockedAxis = DragAxis.None;
        private const float directionThreshold = 10f;

        
        
        public Observable<(DraggableRoomUI room, Vector2 position, bool snap)> RoomPositionUpdates => roomPositionUpdates;
        private readonly Subject<(DraggableRoomUI room, Vector2 position, bool snap)> roomPositionUpdates = new();


        private void Awake() => DungeonLayoutManager.Instance.CurrentDungeon.Subscribe(OnCurrentDungeonChanged).AddTo(this);

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

        public void OnRoomMouseDown(DraggableRoomUI room, Vector2 mousePosition)
        {
            if (currentlyDraggedRoom != null)
                return;

            currentlyDraggedRoom = room;
            dragStartMousePosition = mousePosition;
            lastMousePosition = mousePosition;
            dragStartGridPosition = room.CurrentGridPosition;
            currentValidGridPosition = room.CurrentGridPosition;
            lockedAxis = DragAxis.None;

            Debug.Log($"Started dragging room at grid position {dragStartGridPosition}");
        }

        public void OnRoomMouseDrag(Vector2 mousePosition)
        {
            if (currentlyDraggedRoom == null)
                return;

            Vector2 delta = mousePosition - dragStartMousePosition;

            //determine axis lock if not yet locked
            if (lockedAxis == DragAxis.None && delta.magnitude > directionThreshold)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    lockedAxis = DragAxis.Horizontal;
                else
                    lockedAxis = DragAxis.Vertical;
            }

            if (lockedAxis == DragAxis.None)
                return;

            var roomShape = currentlyDraggedRoom.RoomDefiner.Definition.Shape;
            var shapeCoords = roomShape.ShapeCoordinates;
            int minX = shapeCoords.Min(c => c.x);
            int maxX = shapeCoords.Max(c => c.x);
            int minY = shapeCoords.Min(c => c.y);
            int maxY = shapeCoords.Max(c => c.y);

            //calc desired pos
            Vector2 currentUIPosition = currentlyDraggedRoom.RectTransform.anchoredPosition;
            Vector2 incrementalMouseDelta = mousePosition - lastMousePosition;
            Vector2 desiredUIPosition;

            if (lockedAxis == DragAxis.Horizontal)
            {
                //horiz
                desiredUIPosition = new Vector2(
                    currentUIPosition.x + incrementalMouseDelta.x,
                    currentUIPosition.y
                );
            }
            else //verrt
            {
                desiredUIPosition = new Vector2(
                    currentUIPosition.x,
                    currentUIPosition.y + incrementalMouseDelta.y
                );
            }

            //shape aware bounds
            Vector2 minBoundsUI = GridToUIPosition(new Vector2Int(-minX, -minY));
            Vector2 maxBoundsUI = GridToUIPosition(new Vector2Int(dungeonBounds.x - 1 - maxX, dungeonBounds.y - 1 - maxY));

            desiredUIPosition.x = Mathf.Clamp(desiredUIPosition.x, minBoundsUI.x, maxBoundsUI.x);
            desiredUIPosition.y = Mathf.Clamp(desiredUIPosition.y, minBoundsUI.y, maxBoundsUI.y);

            //check room collisions
            Vector2Int desiredGridPos = CalculateGridPositionUnclamped(desiredUIPosition);

            Vector2Int validGridPos = FindFurthestValidPosition(
                currentlyDraggedRoom,
                currentValidGridPosition,
                desiredGridPos,
                lockedAxis == DragAxis.Horizontal
            );

            //update pos to valid
            currentValidGridPosition = validGridPos;

            //clamp tile overlap when collision detected
            Vector2 validGridUIPos = GridToUIPosition(validGridPos);
            float halfTile = tileSize * 0.5f;
            Vector2 finalUIPosition = desiredUIPosition;

            if (lockedAxis == DragAxis.Horizontal)
            {
                //check if next is valid
                Vector2Int nextGridRight = new Vector2Int(validGridPos.x + 1, validGridPos.y);
                Vector2Int nextGridLeft = new Vector2Int(validGridPos.x - 1, validGridPos.y);

                bool canSlideRight = CheckPositionValid(currentlyDraggedRoom, nextGridRight);
                bool canSlideLeft = CheckPositionValid(currentlyDraggedRoom, nextGridLeft);

                //calc boundaries
                float minBoundary = canSlideLeft ? validGridUIPos.x - halfTile : validGridUIPos.x;
                float maxBoundary = canSlideRight ? validGridUIPos.x + halfTile : validGridUIPos.x;

                finalUIPosition.x = Mathf.Clamp(desiredUIPosition.x, minBoundary, maxBoundary);
            }
            else //vert
            {
                //check if next is valid
                Vector2Int nextGridUp = new Vector2Int(validGridPos.x, validGridPos.y + 1);
                Vector2Int nextGridDown = new Vector2Int(validGridPos.x, validGridPos.y - 1);

                bool canSlideUp = CheckPositionValid(currentlyDraggedRoom, nextGridUp);
                bool canSlideDown = CheckPositionValid(currentlyDraggedRoom, nextGridDown);

                //calc boundaries
                float minBoundary = canSlideDown ? validGridUIPos.y - halfTile : validGridUIPos.y;
                float maxBoundary = canSlideUp ? validGridUIPos.y + halfTile : validGridUIPos.y;

                finalUIPosition.y = Mathf.Clamp(desiredUIPosition.y, minBoundary, maxBoundary);
            }

            roomPositionUpdates.OnNext((currentlyDraggedRoom, finalUIPosition, false));
            lastMousePosition = mousePosition;
        }

        private Vector2Int CalculateGridPositionUnclamped(Vector2 uiPosition)
        {
            var halfBounds = new Vector2(
                (dungeonBounds.x - 1) * 0.5f,
                (dungeonBounds.y - 1) * 0.5f
            );

            var grid = new Vector2Int(
                Mathf.RoundToInt((uiPosition.x / tileSize) + halfBounds.x),
                Mathf.RoundToInt((uiPosition.y / tileSize) + halfBounds.y)
            );

            return grid;
        }

        private Vector2Int FindFurthestValidPosition(DraggableRoomUI room, Vector2Int startPos, Vector2Int targetPos, bool isHorizontal)
        {
            //return if pos valid
            if (CheckPositionValid(room, targetPos))
                return targetPos;

            // move direction from start toward target pos
            var direction = isHorizontal ?
                (targetPos.x > startPos.x ? 1 : -1) :
                (targetPos.y > startPos.y ? 1 : -1);

            Vector2Int currentPos = startPos;
            Vector2Int lastValidPos = startPos;

            int steps = isHorizontal ?
                Mathf.Abs(targetPos.x - startPos.x) :
                Mathf.Abs(targetPos.y - startPos.y);

            for (int i = 1; i <= steps; i++)
            {
                currentPos = isHorizontal ? new Vector2Int(startPos.x + (direction * i), startPos.y) : new Vector2Int(startPos.x, startPos.y + (direction * i));
                if (CheckPositionValid(room, currentPos))
                    lastValidPos = currentPos;
                else
                    break; // collision
            }

            return lastValidPos;
        }

        public void OnRoomMouseUp()
        {
            if (currentlyDraggedRoom == null)
                return;

            //calc final grid pos
            Vector2 currentUIPosition = currentlyDraggedRoom.RectTransform.anchoredPosition;
            Vector2Int finalGridPosition = CalculateGridPosition(currentUIPosition);

            // TODO: Add collision resolution here - find nearest valid position
            finalGridPosition = FindNearestValidPosition(currentlyDraggedRoom, finalGridPosition);

            currentlyDraggedRoom.CurrentGridPosition = finalGridPosition;

            //update room pos
            if (currentlyDraggedRoom.RoomDefiner?.Definition != null)
            {
                currentlyDraggedRoom.RoomDefiner.Definition.SetStartPosition(finalGridPosition);
                Debug.Log($"Room {currentlyDraggedRoom.RoomDefiner.name} moved to grid position {finalGridPosition}");
            }

            //snap pos
            Vector2 finalUIPosition = GridToUIPosition(finalGridPosition);
            roomPositionUpdates.OnNext((currentlyDraggedRoom, finalUIPosition, true));

            lockedAxis = DragAxis.None;
            currentlyDraggedRoom = null;
        }

        private bool CheckPositionValid(DraggableRoomUI room, Vector2Int gridPosition)
        {
            if (room?.RoomDefiner?.Definition?.Shape == null || currentDungeon == null)
                return false;

            //get occupied space.
            var roomShape = room.RoomDefiner.Definition.Shape;
            var roomTiles = new HashSet<Vector2Int>();

            foreach (var coord in roomShape.ShapeCoordinates)
            {
                Vector2Int absolutePos = gridPosition + coord;

                //check bounds
                if (absolutePos.x < 0 || absolutePos.x >= dungeonBounds.x ||
                    absolutePos.y < 0 || absolutePos.y >= dungeonBounds.y)
                    return false;

                roomTiles.Add(absolutePos);
            }

            //check collision
            foreach (var otherRoom in currentDungeon.rooms)
            {
                if (otherRoom == room.RoomDefiner)
                    continue;

                if (otherRoom?.Definition?.Shape == null)
                    continue;

                //get other room tiles
                var otherShape = otherRoom.Definition.Shape;
                if (otherShape.ShapeCoordinates
                    .Select(coord => otherRoom.Definition.StartPosition + coord)
                    .Any(otherAbsolutePos => 
                        roomTiles.Contains(otherAbsolutePos)))
                    return false;
            }

            return true;
        }

        private Vector2Int FindNearestValidPosition(DraggableRoomUI room, Vector2Int currentPosition)
        {
            //check current valid
            if (CheckPositionValid(room, currentPosition))
                return currentPosition;
            
            var cardinalDirections = new[]
            {
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, -1)
            };

            //return first valid neighbour
            foreach (var direction in cardinalDirections)
            {
                var neighborPos = currentPosition + direction;
                if (CheckPositionValid(room, neighborPos))
                    return neighborPos;
            }

            //fallback
            return currentPosition;
        }

    }
}
