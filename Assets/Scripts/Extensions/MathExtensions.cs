using UnityEngine;

namespace Runtime.Extensions
{
    public static class MathExtensions
    {
        public static Vector2Int ToInt2(this Vector2 vector) => new((int)vector.x, (int)vector.y);
        
        public static Vector2Int Normalised(this Vector2Int vector)
        {
            if (vector is { x: 0, y: 0 })
                return Vector2Int.zero;
    
            var max = Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    
            return new Vector2Int(
                Mathf.RoundToInt((float)vector.x / max),
                Mathf.RoundToInt((float)vector.y / max)
            );
        }    }
}