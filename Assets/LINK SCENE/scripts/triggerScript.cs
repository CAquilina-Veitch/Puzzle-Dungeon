using UnityEngine;
using System;
using UnityEngine.Events;
using Scripts.Dungeon;
public class triggerScript : MonoBehaviour
{
    [SerializeField] bool destroyOnTriggerEnter;
    [SerializeField] string tagFilter;
    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;
    public string attatchedRoom;

    private void Start()
    {
        attatchedRoom = gameObject.transform.parent.name;    
    }
    void OnTriggerEnter(Collider other)
    {
        if (!String.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) return;

        onTriggerEnter.Invoke();
        
        
        if (destroyOnTriggerEnter)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!String.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) return;

        onTriggerExit.Invoke();
    }
}
