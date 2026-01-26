using UnityEngine;

namespace Scripts.Player
{
    public class PlayerBindings : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
                UIManager.Instance.ToggleUIWindowActive(UIWindow.DungeonMap);
        }
    }
}
