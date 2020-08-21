using PowerControl.UI;
using UnityEngine;

namespace PowerControl
{
    public class LoopHook : MonoBehaviour
    {
        private void Awake()
        {
            ModCore.Trace("Hook initialized");
        }

        private void OnGUI()
        {
            bool open = GUILayout.Button("Open UI");
            if (open)
                UI_PowerTerminal.Open(null);
        }
    }
}
