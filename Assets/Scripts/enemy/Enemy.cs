using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] public RoomManager RM;


    public void reportDeath()
    {
        if (RM != null) RM.RemoveEnemy(this);
    }

    public abstract void dropItems();
    
    
    

    public void Start()
    {
        RM = GameObject.FindFirstObjectByType<RoomManager>();
        if (RM != null) RM.AddEnemy(this);
    }
}

