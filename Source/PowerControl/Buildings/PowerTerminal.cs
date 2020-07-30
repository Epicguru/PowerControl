using System.Collections.Generic;
using RimWorld;
using System.Text;
using Verse;
using PowerControl.UI;

namespace PowerControl.Buildings
{
    public class PowerTerminal : Building
    {
        public CompPowerTrader PowerTrader
        {
            get
            {
                if (_powerTrader == null)
                    _powerTrader = GetComp<CompPowerTrader>();
                return _powerTrader;
            }
        }
        private CompPowerTrader _powerTrader;

        private StringBuilder customStr = new StringBuilder();

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var thing in base.GetGizmos())
            {
                yield return thing;
            }

            var openUI = new Command_Action();
            openUI.defaultLabel = "PC.OpenTerminalUI".Translate();
            openUI.defaultDesc = "PC.OpenTerminalUIDesc".Translate();
            openUI.action = () =>
            {
                // Clicked! Open the UI. This will automatically open a new window or focus an existing one.
                UI_PowerTerminal.Open(this);
            };
        }

        public override string GetInspectString()
        {
            var baseString =  base.GetInspectString();
            customStr.Clear();

            var net = PowerTrader.PowerNet;
            if (net == null)
            {
                customStr.AppendLine("Not connected to a power net.");
            }
            else
            {
                int traderComps = net.powerComps.Count;
                int batteryComps = net.batteryComps.Count;
                int transmitters = net.transmitters.Count;
                int connectors = net.connectors.Count;

                customStr.Append("Traders: ").AppendLine(traderComps.ToString());
                customStr.Append("Batteries: ").AppendLine(batteryComps.ToString());
                customStr.Append("Transmitters: ").AppendLine(transmitters.ToString());
                customStr.Append("Connectors: ").AppendLine(connectors.ToString());
            }

            return (baseString + customStr).TrimEnd();
        }
    }
}
