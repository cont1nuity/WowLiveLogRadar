using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rendering.LogHook.EventHandling
{
    public class SpellCastSuccessHandler : EventHandler
    {
        public void Handle(string[] args) {
            // 0 - eventType
            // 1 - source id
            // 2 - source name
            // 9 - spell id
            // if we're considering x,y grid with 0,0 in the bottom left corner
            // 24 Y
            // 25 -X
            if (args[1].StartsWith("Player-")) {
                float x = float.Parse(args[25], CultureInfo.InvariantCulture);
                float y = float.Parse(args[24], CultureInfo.InvariantCulture);

                var instance = EntityStateMaster.Instance;
                instance.SetPlayerPosition(args[1], x, y);
                instance.SetNameOnPlayer(args[1], args[2].Split('-')[0]);
            } else {
                var spellId = int.Parse(args[9], CultureInfo.InvariantCulture);
                if (spellId == 181113)
                { // encounter spawn - some add spawned!
                    float x = float.Parse(args[25], CultureInfo.InvariantCulture);
                    float y = float.Parse(args[24], CultureInfo.InvariantCulture);
                    float rotation = float.Parse(args[27], CultureInfo.InvariantCulture);

                    var instance = EntityStateMaster.Instance;
                    EntityStateMaster.Instance.SetCreaturePosition(args[12], x, y, rotation); // 1 or 12?
                    //SPELL_CAST_SUCCESS,Creature-0-3109-2569-16854-203230-00001F3858,"Dragonfire Golem",0xa48,0x0,0000000000000000,nil,0x80000000,0x80000000,181113,"Encounter Spawn",0x1,Creature-0-3109-2569-16854-203230-00001F3858,Creature-0-3109-2569-16854-203331-00019F33F6,2257851,2257851,0,0,5043,0,3,0,100,0,2837.35,2505.75,2166,5.5621,70
                }
                else if (spellId == 400430)
                { // breath spell
                    EntityStateMaster.Instance.RemoveBeamOriginatingFromCreature(spellId.ToString());
                }
            }
        }
    }
}
