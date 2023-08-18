using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rendering.LogHook.EventHandling
{
    public class SpellHealHandler : EventHandler
    {
        public void Handle(string[] args) {
            // 0 - eventType
            // 1 - source id
            // 5 - target id
            // 6 - target name
            // if we're considering x,y grid with 0,0 in the bottom left corner
            // 24 Y target
            // 25 -X target
            // 27 - rotation

            if (args[5].StartsWith("Creature-") && !args[13].StartsWith("Player-")) { //ensure to filter summoned creatures from players...) {
                float x = float.Parse(args[25], CultureInfo.InvariantCulture);
                float y = float.Parse(args[24], CultureInfo.InvariantCulture);
                float rotation = float.Parse(args[27], CultureInfo.InvariantCulture);
                EntityStateMaster.Instance.SetCreaturePosition(args[5], x, y, rotation);
            }
            else if (args[5].StartsWith("Player-")) {
                // this or spell cast success?
                float x = float.Parse(args[25], CultureInfo.InvariantCulture);
                float y = float.Parse(args[24], CultureInfo.InvariantCulture);

                var instance = EntityStateMaster.Instance;
                instance.SetPlayerPosition(args[5], x, y);
                instance.SetNameOnPlayer(args[5], args[6].Split('-')[0]);
            }
        }
    }
}
