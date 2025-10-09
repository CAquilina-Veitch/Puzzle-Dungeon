using System;
using R3;
using UnityEngine;

namespace Scripts.Dungeon
{
    public class DungeonMapUIController : MonoBehaviour
    {
        private void Awake() => DungeonLayoutManager.Instance.CurrentDungeon.Subscribe(OnCurrentDungeonChanged).AddTo(this);

        private void OnCurrentDungeonChanged(DungeonDefinition currentDungeon)
        {
            
        }
    }
}
