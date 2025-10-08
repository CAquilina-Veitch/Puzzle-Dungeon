using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public abstract class DungeonRoom : MonoBehaviour
{
    public abstract RoomType RoomType { get; }
    public Vector2Int[] ShapeCoordinatesDefinition => shapeCoordinatesDefinition;
    [SerializeField] private Vector2Int[] shapeCoordinatesDefinition;

    public Vector2Int Position => position;
    [SerializeField] private Vector2Int position;

    public bool IsInCoordinate(Vector2Int positionToCheck)
    {
        var positions = shapeCoordinatesDefinition.Select(coord => coord + position).ToList();
        return positions.Contains(positionToCheck);
    }
    
    
}