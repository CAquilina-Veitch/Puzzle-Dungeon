using UnityEngine;

namespace Runtime.Extensions
{
    public static class MathExtensions
    {
        public static Vector2Int ToInt2(this Vector2 vector) => new((int)vector.x, (int)vector.y);
    }
}