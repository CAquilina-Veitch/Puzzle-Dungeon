using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public List<Enemy> activeEnemies = new List<Enemy>();
    [SerializeField] UnityEvent EnemiesCleared;
    
    public void AddEnemy(Enemy enemy)
    {
        activeEnemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
      if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);

            if (activeEnemies.Count == 0)
            {
                EnemiesCleared.Invoke();
            }
        }
    }
}
