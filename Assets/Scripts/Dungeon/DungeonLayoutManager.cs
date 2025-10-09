using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Dungeon
{
    public class DungeonLayoutManager : MonoBehaviour
    {
        [SerializeField] private List<DungeonLayoutDefinition> standardRooms = new();
    }
}