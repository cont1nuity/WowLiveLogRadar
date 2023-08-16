﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rendering.LogHook.EventHandling
{
    public class CombatLogEventHandler
    {
        private readonly Dictionary<string, EventHandler> Handlers = new Dictionary<string, EventHandler>() {
            {
                "WORLD_MARKER_PLACED", new WorldMarkerPlacedHandler()
            },
            {
                "COMBATANT_INFO", new CombatantInfoHandler()
            },
            {
                "SPELL_CAST_SUCCESS", new SpellCastSuccessHandler()
            },
            {
                "SPELL_AURA_APPLIED", new SpellAuraAppliedHandler()
            },
            {
                "SPELL_AURA_REMOVED", new SpellAuraRemovedHandler()
            }
            
                        // todo: features I wanna make for fun
                        // more events for positions
                        // current player highlight
                        // attach line between player and a marker during debuff
                        // laser from boss when he does breath
                        // arrow showing to go to a marker with debuff

                        // wont care for now:
                        // deaths
                        // wipes? should just start over on next attempt
                        // making stuff generic. It's just a POC to get my point across. Hardcode IDs for days babyyyy
        };

        public void Handle(string eventType, string[] args) {
            if(!Handlers.ContainsKey(eventType)) {
                return;
            }

            Handlers[eventType].Handle(args);
        }
    }
}
