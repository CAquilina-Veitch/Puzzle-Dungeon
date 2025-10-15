using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Dungeon
{
    public class DungeonDefinition : MonoBehaviour
    {
        private const float editorScale = 10;

        public DungeonID DungeonID => dungeonID;
        [SerializeField] private DungeonID dungeonID;

        public Vector2Int Bounds => bounds;
        [SerializeField] private Vector2Int bounds;

        public DungeonRoomDefinition[] Definitions => rooms.Select(definer => definer.Definition).ToArray();
        [SerializeField] public DungeonRoomDefiner[] rooms = {};
        [SerializeField, HideInInspector] private List<DungeonRoomDefiner> lastRooms = new();
        
        
        private int numRooms;
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            
            if (numRooms != rooms.Length) 
                HandleRoomDifferences();
        }

        private void HandleRoomDifferences()
        {
            if (Application.isPlaying) return;

            if (rooms.Length > numRooms)
            {
                var seen = new HashSet<DungeonRoomDefiner>();
                
                for (var i = 0; i < rooms.Length; i++)
                {
                    var room = rooms[i];
                    if (room == null || !seen.Add(room))
                    {
                        var obj = Instantiate(new GameObject(), transform);
                        var dungeonRoom = obj.AddComponent<DungeonRoomDefiner>();
                        rooms[i] = dungeonRoom;
                    }
                }
            }
            else
            {
                var removedRooms = lastRooms.Except(rooms).ToList();
                foreach (var room in removedRooms)
                    DestroyImmediate(room.gameObject);
            }
            
            lastRooms = rooms.ToList();
            numRooms = rooms.Length;
        }
        

        #endif

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.gray2;

            float halfTile = editorScale * 0.5f;
            Vector3 offset = new Vector3(-halfTile, 0, -halfTile);
            
            Vector3 bottomLeft = transform.position + offset;
            Vector3 bottomRight = transform.position + offset + new Vector3(bounds.x * editorScale, 0, 0);
            Vector3 topRight = transform.position + offset + new Vector3(bounds.x * editorScale, 0, bounds.y * editorScale);
            Vector3 topLeft = transform.position + offset + new Vector3(0, 0, bounds.y * editorScale);

            Gizmos.DrawLine(bottomLeft + Vector3.up * halfTile, bottomRight + Vector3.up * halfTile);
            Gizmos.DrawLine(bottomRight + Vector3.up * halfTile, topRight + Vector3.up * halfTile);
            Gizmos.DrawLine(topRight + Vector3.up * halfTile, topLeft + Vector3.up * halfTile);
            Gizmos.DrawLine(topLeft + Vector3.up * halfTile, bottomLeft + Vector3.up * halfTile);
            Gizmos.DrawLine(bottomLeft + Vector3.down * halfTile, bottomRight + Vector3.down * halfTile);
            Gizmos.DrawLine(bottomRight + Vector3.down * halfTile, topRight + Vector3.down * halfTile);
            Gizmos.DrawLine(topRight + Vector3.down * halfTile, topLeft + Vector3.down * halfTile);
            Gizmos.DrawLine(topLeft + Vector3.down * halfTile, bottomLeft + Vector3.down * halfTile);
            Gizmos.DrawLine(bottomLeft + Vector3.up * halfTile, bottomLeft + Vector3.down * halfTile);
            Gizmos.DrawLine(bottomRight + Vector3.up * halfTile, bottomRight + Vector3.down * halfTile);
            Gizmos.DrawLine(topRight + Vector3.up * halfTile, topRight + Vector3.down * halfTile);
            Gizmos.DrawLine(topLeft + Vector3.up * halfTile, topLeft + Vector3.down * halfTile);
        }
    }
}