using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    //base interactable class

    [SerializeField] protected GameObject interactableImage;
    [SerializeField] string tagFilter;
    [SerializeField] protected Collider interactableCollider;
    public bool singleUse;
    protected PlayerController player;


    public UnityEvent onInteraction;
    public void Interact()
    {
        onInteraction.Invoke();

        if (singleUse == true)
        {
            HideInteractable();
            interactableCollider.enabled = false;
        }
    }

   public void DisplayInteractable()
    {
        interactableImage.SetActive(true);
    }
    public void HideInteractable()
    {
        interactableImage.SetActive(false);
    }

}
