using UnityEngine;
using Scripts.Items;
using Scripts.Physics;
using R3;
using Runtime.Extensions;

public class DroppedItem : MonoBehaviour
{
    [SerializeField] public ItemType itemType = ItemType.None;

    [SerializeField] private Hitbox pickupHitbox;
    [SerializeField] private Rigidbody rb;
    private const int PlayerHurtboxLayer = 9;

    private void Start()
    {
        pickupHitbox.OnTrigger.RP
            .Where(c=> c.type == CollisionType.Enter)
            .Where(c=> c.collider != null && c.collider.gameObject != null)
            .Where(c => c.collider.gameObject.layer == PlayerHurtboxLayer)
            .Subscribe(OnPickedUp)
            .AddTo(this);
    }

    public void OnSpawn(Vector3 randomVelocity)
    {
        rb.linearVelocity = randomVelocity;
    }

    private void OnPickedUp()
    {
        DroppedItemManager.Instance.PickupItem(itemType);
        Destroy(gameObject);
    }
}
