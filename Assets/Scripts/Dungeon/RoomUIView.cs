using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Dungeon
{
    public class RoomUIView : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private DraggableRoomUI draggableRoomUI;
        public RectTransform RectTransform => rectTransform;
        [SerializeField] private RectTransform rectTransform;

        public void Initialize(DungeonRoomDefiner roomDefiner, DungeonMapUIController controller, GameObject tilePrefab)
        {
            if (roomDefiner?.Definition?.Shape == null) return;

            float tileSize = controller.TileSize;

            draggableRoomUI.RoomDefiner = roomDefiner;
            draggableRoomUI.CurrentGridPosition = roomDefiner.Definition.StartPosition;
            draggableRoomUI.Initialize(controller);

            //make background transparent but still receive events
            if (backgroundImage != null)
                backgroundImage.color = new Color(0, 0, 0, 0);

            //create tiles using RELATIVE coordinates (ShapeCoordinates, not CoordinatesAsPosition)
            var tileCoords = roomDefiner.Definition.Shape.ShapeCoordinates;
            foreach (var coord in tileCoords)
            {
                GameObject tileObj;
                if (tilePrefab != null)
                {
                    tileObj = Instantiate(tilePrefab, transform);
                    tileObj.name = $"Tile_{coord.x}_{coord.y}";
                }
                else
                {
                    tileObj = new GameObject($"Tile_{coord.x}_{coord.y}");
                    tileObj.transform.SetParent(transform, false);
                }

                var tileRectTransform = tileObj.GetComponent<RectTransform>();
                if (tileRectTransform == null)
                    tileRectTransform = tileObj.AddComponent<RectTransform>();

                var tileImage = tileObj.GetComponent<Image>();
                if (tileImage == null)
                    tileImage = tileObj.AddComponent<Image>();

                tileImage.color = roomDefiner.EditorColour;

                tileRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                tileRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                tileRectTransform.pivot = new Vector2(0.5f, 0.5f);
                tileRectTransform.sizeDelta = new Vector2(tileSize, tileSize);

                tileRectTransform.anchoredPosition = new Vector2(
                    coord.x * tileSize,
                    coord.y * tileSize
                );

                draggableRoomUI.SubscribeToTileEvents(tileObj);
            }
        }

        public void ClearTiles()
        {
            //destroy all child tiles
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);

                //skip components that aren't tiles
                if (child == backgroundImage?.transform || child == draggableRoomUI?.transform)
                    continue;

                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
        }
    }
}
