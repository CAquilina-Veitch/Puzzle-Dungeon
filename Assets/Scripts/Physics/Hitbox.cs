using System;
using UnityEngine;
using R3;
using Runtime.Extensions;

namespace Scripts.Physics
{
    public enum CollisionType
    {
        Enter,
        Exit,
    }
    public class Hitbox : MonoBehaviour
    {
        [SerializeField,HideInInspector] private Collider collider;

        private void Awake()
        {
            if (collider != null)
                return;
            collider = GetComponent<Collider>();
            if(collider == null)
                Debug.LogWarning($"hitbox {gameObject.name} has no Collider");
        }

        public readonly RORP<(CollisionType type, Collider collider)> OnTrigger = new();
        private void OnTriggerEnter(Collider other) => OnTrigger.Set((CollisionType.Enter, other));
        private void OnTriggerExit(Collider other) => OnTrigger.Set((CollisionType.Exit, other));
        
        public readonly RORP<(CollisionType type, Collision collision)> OnCollision = new();
        private void OnCollisionEnter(Collision other) => OnCollision.Set((CollisionType.Enter, other));
        private void OnCollisionExit(Collision other) => OnCollision.Set((CollisionType.Exit, other));
    }
}
