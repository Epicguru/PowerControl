using PowerControl.Buildings;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PowerControl.UI
{
    public class UI_PowerTerminal : Window
    {
        private static List<UI_PowerTerminal> openTerminals = new List<UI_PowerTerminal>();

        public static UI_PowerTerminal Open(PowerTerminal terminal)
        {
            if (terminal == null)
            {
                ModCore.Warn("Called UI_PowerTerminal.Open() with null terminal.");
                return null;
            }

            var current = GetOpenUI(terminal);
            if (current != null)
            {
                // Focus existing.
                // Does this work?
                Find.WindowStack.Notify_ManuallySetFocus(current);
                return current;
            }
            else
            {
                // Open new.
                var created = new UI_PowerTerminal();
                created.Terminal = terminal;

                openTerminals.Add(created);

                return created;
            }
        }

        public static UI_PowerTerminal GetOpenUI(PowerTerminal terminal)
        {
            if (terminal == null || !terminal.Spawned)
                return null;

            foreach (var ui in openTerminals)
            {
                if (ui.Terminal == terminal)
                    return ui;
            }

            return null;
        }

        public PowerTerminal Terminal;

        private UI_PowerTerminal()
        {
            doCloseButton = true;
            onlyOneOfTypeAllowed = false;
            draggable = true;
            preventCameraMotion = false;
            resizeable = true;
            absorbInputAroundWindow = false;
            focusWhenOpened = true;
            closeOnClickedOutside = false;
            closeOnAccept = true;
            closeOnCancel = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Widgets.Label(new Rect(inRect.x, inRect.y, inRect.width, inRect.height), Terminal.def.LabelCap + " on powernet of size " + Terminal.PowerTrader.PowerNet.powerComps.Count);
        }
    }
}
