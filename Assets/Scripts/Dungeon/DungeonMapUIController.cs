using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Dungeon
{
    public class DungeonMapUIController : MonoBehaviour
    {
        [SerializeField] private RectTransform tilesContainer;
        [SerializeField] private RectTransform boundsBackground;
        [SerializeField] private AspectRatioFitter aspectRatioFitter;

        private Dictionary<DungeonRoomDefinition, RectTransform> roomUIGroups = new();
        private float tileSize;

        private void Awake() => DungeonLayoutManager.Instance.CurrentDungeon.Subscribe(OnCurrentDungeonChanged).AddTo(this);

        private void OnCurrentDungeonChanged(DungeonDefinition currentDungeon)
        {
            ClearUI();

            if (currentDungeon == null || tilesContainer == null || boundsBackground == null || aspectRatioFitter == null)
                return;

            Vector2Int bounds = currentDungeon.Bounds;
            if (bounds.x > 0 && bounds.y > 0) 
                aspectRatioFitter.aspectRatio = (float)bounds.x / (float)bounds.y;

            Canvas.ForceUpdateCanvases();
            Rect bgRect = boundsBackground.rect;
            tileSize = Mathf.Min(bgRect.width / bounds.x, bgRect.height / bounds.y);

            foreach (var room in currentDungeon.rooms)
            {
                if (room?.Definition?.Shape == null)
                    continue;

                var roomGroupObj = new GameObject($"Room_{room.name}");
                var roomRectTransform = roomGroupObj.AddComponent<RectTransform>();
                roomRectTransform.SetParent(boundsBackground, false);

                roomRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                roomRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                roomRectTransform.pivot = new Vector2(0.5f, 0.5f);
                roomRectTransform.anchoredPosition = Vector2.zero;

                var tileCoords = room.Definition.Shape.CoordinatesAsPosition;
                foreach (var coord in tileCoords)
                {
                    var tileObj = new GameObject($"Tile_{coord.x}_{coord.y}");
                    var tileRectTransform = tileObj.AddComponent<RectTransform>();
                    tileRectTransform.SetParent(roomRectTransform, false);
                    var tileImage = tileObj.AddComponent<Image>();
                    tileImage.color = room.EditorColour;

                    tileRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    tileRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    tileRectTransform.pivot = new Vector2(0.5f, 0.5f);
                    tileRectTransform.sizeDelta = new Vector2(tileSize, tileSize);

                    tileRectTransform.anchoredPosition = new Vector2(
                        (coord.x - (bounds.x - 1) * 0.5f) * tileSize,
                        (coord.y - (bounds.y - 1) * 0.5f) * tileSize
                    );
                }

                roomUIGroups[room.Definition] = roomRectTransform;
            }
        }

        private void ClearUI()
        {
            if (boundsBackground == null)
                return;

            foreach (Transform child in boundsBackground)
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }

            roomUIGroups.Clear();
        }
    }
}
