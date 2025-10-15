using System;
using System.Linq;
using UnityEngine;

namespace Scripts.Dungeon
{
    [Serializable]public class TileShape
    {
        public void SetPosition(Vector2Int newPosition)
        {
            Debug.LogWarning($"new pos {newPosition}");
            position = newPosition;
        }

        public Vector2Int Position => position;
        [SerializeField] private Vector2Int position;
        public Vector2Int[] ShapeCoordinates => shapeCoordinates;
        [SerializeField] private Vector2Int[] shapeCoordinates = { new(0, 0) };

        public TileShape()
        {
            
        }

        public TileShape(Vector2Int position, Vector2Int[] shapeCoordinates)
        {
            this.position = position;
            this.shapeCoordinates = shapeCoordinates;
        }

        public bool IsInCoordinates(Vector2Int posCheck) => CoordinatesAsPosition.Contains(posCheck);

        public Vector2Int[] CoordinatesAsPosition => ShapeCoordinates.Select(shapeCoordinate => shapeCoordinate + Position ).ToArray();
    }
}