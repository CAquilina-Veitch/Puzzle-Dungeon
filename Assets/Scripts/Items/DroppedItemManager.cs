using UnityEngine;
using Scripts.Behaviours;
using Scripts.Items;
using Runtime.Extensions;
using System.Collections.Generic;

public class DroppedItemManager : SingletonBehaviour<DroppedItemManager>
{
    [Header("Prefab Reference")]
    [SerializeField] private List<EnumPair<ItemType, DroppedItem>> droppedItemPrefabs;

    private Dictionary<ItemType, DroppedItem> itemPrefabDict;

    [Header("Physics Settings")]
    [SerializeField] private float minVelocity = 2f;
    [SerializeField] private float maxVelocity = 5f;
    [SerializeField] private float upwardForce = 3f;

    protected override void OnAwake()
    {
        base.OnAwake();
        InitializePrefabDictionary();
    }

    private void InitializePrefabDictionary()
    {
        itemPrefabDict = new Dictionary<ItemType, DroppedItem>();
        if (droppedItemPrefabs != null)
        {
            foreach (var entry in droppedItemPrefabs)
            {
                if (!itemPrefabDict.ContainsKey(entry.enumKey))
                {
                    itemPrefabDict.Add(entry.enumKey, entry.Value);
                }
            }
        }
    }

    /// <summary>
    /// Spawns a dropped item at the specified position with random velocity
    /// </summary>
    /// <param name="spawnTransform">Position and rotation to spawn the item at</param>
    /// <param name="itemType">Type of item to spawn</param>
    public void SpawnDroppedItem(Transform spawnTransform, ItemType itemType)
    {
        if (itemPrefabDict == null) InitializePrefabDictionary();

        if (!itemPrefabDict.TryGetValue(itemType, out DroppedItem prefabToSpawn))
        {
            Debug.LogWarning($"DroppedItemManager: No prefab found for ItemType {itemType}");
            return;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogError($"DroppedItemManager: Prefab for {itemType} is null!");
            return;
        }

        // Instantiate the prefab at the spawn position
        DroppedItem droppedItem = Instantiate(prefabToSpawn, spawnTransform.position, Quaternion.identity);

        if (droppedItem != null)
        {
            droppedItem.itemType = itemType;

            // Calculate random velocity
            Vector3 randomVelocity = CalculateRandomVelocity();

            // Apply the velocity to the dropped item
            droppedItem.OnSpawn(randomVelocity);
        }
    }

    /// <summary>
    /// Calculates a random velocity for the dropped item
    /// </summary>
    private Vector3 CalculateRandomVelocity()
    {
        // Random direction in XZ plane
        float randomAngle = Random.Range(0f, 360f);
        float randomSpeed = Random.Range(minVelocity, maxVelocity);

        Vector3 horizontalVelocity = new Vector3(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomSpeed,
            0f,
            Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomSpeed
        );

        // Add upward component
        Vector3 velocity = horizontalVelocity + Vector3.up * upwardForce;

        return velocity;
    }

    /// <summary>
    /// Called when a player picks up an item. Currently empty - implement your inventory logic here.
    /// </summary>
    /// <param name="itemType">The type of item that was picked up</param>
    public void PickupItem(ItemType itemType)
    {
        // TODO: Implement pickup logic (add to inventory, play sound, show UI feedback, etc.)
        Debug.Log($"Picked up item: {itemType}");
    }
}
