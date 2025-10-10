using UnityEngine;

namespace Scripts.Dungeon
{
    public class DungeonTestingKeybinds : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
                DungeonLayoutManager.Instance.LoadDungeon(DungeonID.None);

            if (Input.GetKeyDown(KeyCode.Alpha1))
                DungeonLayoutManager.Instance.LoadDungeon(DungeonID.Dungeon1);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                DungeonLayoutManager.Instance.LoadDungeon(DungeonID.Dungeon2);

            if (Input.GetKeyDown(KeyCode.Alpha3))
                DungeonLayoutManager.Instance.LoadDungeon(DungeonID.Dungeon3);

            if (Input.GetKeyDown(KeyCode.Alpha4))
                DungeonLayoutManager.Instance.LoadDungeon(DungeonID.Dungeon4);
        }
    }
}
