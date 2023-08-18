using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rendering.LogHook.EventHandling
{
    public class UnitDiedHandler : EventHandler
    {
        public void Handle(string[] args) {
            // 0 - eventType
            // 5 - unit GUID
            // 6 - unit name
            if (args[5].StartsWith("Player-")) {
                // TODO: mark them as R.I.P. some time...
            } else if (args[5].StartsWith("Creature-")) {
                var instance = EntityStateMaster.Instance;
                instance.RemoveCreatureToRender(args[5]);
            }
        }
    }
}
