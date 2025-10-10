using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Dungeon
{
    [ExecuteAlways]
    public class DungeonRoomDefiner : MonoBehaviour
    {
        private const float editorScale = 10;
        [SerializeField] private Color editorColour = Color.burlywood;
        [SerializeField] private RoomType roomType;
        [SerializeField, HideInInspector] private RoomType lastSelectedRoomType = RoomType.None;

        public DungeonRoomDefinition Definition => definition;
        [SerializeReference] private DungeonRoomDefinition definition = new StandardRoomDefinition();
        [SerializeField, HideInInspector] private Vector3 lastPosition;
        [SerializeField, HideInInspector] private Vector2Int lastStartPosition;
        [SerializeField, HideInInspector] private Vector2Int[] cachedCoordinates;
        [SerializeField, HideInInspector] private Vector2Int[] lastShapeCoordinates;

        private void Reset()
        {
            definition = new EmptyRoomDefinition();
            transform.name = roomType.ToString();
            editorColour = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f),1f);
        }

        private void Update()
        {
            if (Application.isPlaying) return;

            //check if position changed and snap to grid
            if (transform.position != lastPosition)
            {
                SyncStartPositionAndSnap();
                UpdateCachedCoordinates();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Only create new definition if it's completely missing
            if (definition == null)
            {
                if (roomType != RoomType.None) 
                    UpdateRoomDefinition();
                UpdateCachedCoordinates();
                return;
            }

            // If Shape is null, just initialize it (don't recreate entire definition)
            if (definition.Shape == null)
            {
                definition.SetShape(new TileShape());
                UpdateCachedCoordinates();
                return;
            }

            if (roomType != lastSelectedRoomType)
            {
                UpdateRoomDefinition();
                UpdateCachedCoordinates();
                return;
            }

            //sync StartPosition when transform moves and snap to grid
            if (transform.position != lastPosition) 
                SyncStartPositionAndSnap();
            
            Vector2Int expectedStartPos = new Vector2Int(
                Mathf.RoundToInt(transform.position.x / editorScale),
                Mathf.RoundToInt(transform.position.z / editorScale)
            );
            if (definition.StartPosition != expectedStartPos) 
                SyncStartPositionAndSnap();

            if (CoordinatesChanged())
                UpdateCachedCoordinates();
        }

        private void SyncStartPositionAndSnap()
        {
            if (definition == null) return;

            //snap transform position to grid
            Vector3 snappedPosition = new Vector3(
                Mathf.Round(transform.position.x / editorScale) * editorScale,
                0,
                Mathf.Round(transform.position.z / editorScale) * editorScale
            );

            transform.position = snappedPosition;

            definition.SetStartPosition(new Vector2Int(
                Mathf.RoundToInt(snappedPosition.x / editorScale),
                Mathf.RoundToInt(snappedPosition.z / editorScale)
            ));
        }

        private bool CoordinatesChanged()
        {
            if (definition?.Shape?.ShapeCoordinates == null) return false;

            var current = definition.Shape.ShapeCoordinates;

            if (lastShapeCoordinates == null || current.Length != lastShapeCoordinates.Length)
                return true;

            if (transform.position != lastPosition)
                return true;

            if (definition.StartPosition != lastStartPosition)
                return true;

            //check if any coordinates changed
            for (int i = 0; i < current.Length; i++)
                if (current[i] != lastShapeCoordinates[i])
                    return true;

            return false;
        }

        private void UpdateRoomDefinition()
        {
            //old data
            Vector2Int oldStartPosition = definition?.StartPosition ?? Vector2Int.zero;
            TileShape oldShape = definition?.Shape;

            //get new definition
            definition = DungeonRoomDefinitionLibrary.GetDef(roomType);
            lastSelectedRoomType = roomType;
            transform.name = roomType.ToString();

            //copy preserved data to new definition
            if (definition != null)
            {
                definition.SetStartPosition(oldStartPosition);
                if (oldShape != null)
                    definition
                        .SetShape(
                            new TileShape(
                                position: oldShape.Position, 
                                shapeCoordinates: oldShape.ShapeCoordinates?.ToArray() ?? new[] { new Vector2Int(0, 0) }));
            }
        }

        private void UpdateCachedCoordinates()
        {
            if (definition?.Shape == null) return;

            definition.UpdateShapePosition();
            cachedCoordinates = definition.Shape.CoordinatesAsPosition;
            lastPosition = transform.position;
            lastStartPosition = definition.StartPosition;

            //store a copy of the coords
            if (definition.Shape.ShapeCoordinates == null) return;
            
            lastShapeCoordinates = new Vector2Int[definition.Shape.ShapeCoordinates.Length];
            Array.Copy(definition.Shape.ShapeCoordinates, lastShapeCoordinates, definition.Shape.ShapeCoordinates.Length);
        }

        private void OnDrawGizmos()
        {
            if (cachedCoordinates == null || cachedCoordinates.Length == 0) UpdateCachedCoordinates();

            if (cachedCoordinates == null || cachedCoordinates.Length == 0) return;

            Gizmos.color = editorColour;

            foreach (var g in cachedCoordinates)
            {
                Vector3 position = new Vector3(g.x, 0, g.y) * editorScale;
                Vector3 size = Vector3.one * editorScale;
                Gizmos.DrawWireCube(position, size);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (cachedCoordinates == null || cachedCoordinates.Length == 0) return;

            Gizmos.color = editorColour;

            foreach (var g in cachedCoordinates)
            {
                Vector3 position = new Vector3(g.x, 0, g.y) * editorScale;
                Vector3 size = Vector3.one * editorScale;
                Gizmos.DrawCube(position, size);
            }
        }
#endif
    }
}