using UnityEngine;
using UnityEngine.InputSystem;


public class transitionHandler : MonoBehaviour
{
    public string currentRoom;
    [SerializeField] triggerScript roomTrigger;
    public PlayerInput controller;

    void Start()
    {
        controller = GetComponent<PlayerInput>();
    } 

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 13)
            Debug.Log("trigger enter");
        {
            roomTrigger = other.gameObject.GetComponent<triggerScript>();

            if (roomTrigger.attatchedRoom != currentRoom)
            {
                currentRoom = roomTrigger.attatchedRoom;
                RoomTransition();
            }
        }
    }

    public void DisableControls()
    {
        controller.enabled = false;
    }
    public void EnableControls()
    {
        controller.enabled = true;
    }

    public void RoomTransition()
    {
        DisableControls();
        Invoke("EnableControls", 2f);
    }
}
