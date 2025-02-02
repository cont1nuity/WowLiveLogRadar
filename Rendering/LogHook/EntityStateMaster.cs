﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rendering.LogHook
{
    // will keep track of all entities to render
    public class EntityStateMaster
    {
        public bool IsInCombat { get; set; } = false;

        private Dictionary<string, Entity> PlayersToRender = new Dictionary<string, Entity>();
        private Dictionary<string, Entity> CreaturesToRender = new Dictionary<string, Entity>();
        private Dictionary<string, Entity> WorldMarkersToRender = new Dictionary<string, Entity>();
        private List<Entity> DebuffDropLocationsToRender = new List<Entity>();
        private Dictionary<string, BeamEntity> BeamsOriginatingFromCreatures = new Dictionary<string, BeamEntity>();
        private string MainCharacterName;
        private float maxX = float.MinValue;
        private float minX = float.MaxValue;
        private float maxY = float.MinValue;
        private float minY = float.MaxValue;

        public float MaxXPos { get { return maxX; } private set { if (maxX < value) { maxX = (float)Math.Round(value / 100, 2, MidpointRounding.AwayFromZero) * 100 + 10; } } }
        public float MaxYPos { get { return maxY; } private set { if (maxY < value) { maxY = (float)Math.Round(value / 100, 2, MidpointRounding.AwayFromZero) * 100 + 10; } } }
        public float MinXPos { get { return minX; } private set { if (minX > value) { minX = (float)Math.Round(value / 100, 2, MidpointRounding.AwayFromZero) * 100 - 10; } } }
        public float MinYPos { get { return minY; } private set { if (minY > value) { minY = (float)Math.Round(value / 100, 2, MidpointRounding.AwayFromZero) * 100 - 10; } } }

        public static EntityStateMaster Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested() {
            }

            internal static readonly EntityStateMaster instance = new EntityStateMaster();
        }

        private EntityStateMaster() { }

        public void SetMainCharacter(string mainCharacterName) {
            MainCharacterName = mainCharacterName;
        }

        public void ClearState() {
            if (IsInCombat) {
                maxX = float.MinValue;
                minX = float.MaxValue;
                maxY = float.MinValue;
                minY = float.MaxValue;
            }
            PlayersToRender.Clear();
            CreaturesToRender.Clear();
            DebuffDropLocationsToRender.Clear();
            BeamsOriginatingFromCreatures.Clear();
    }

        public void SetNameOnPlayer(string id, string name) {
            if (!PlayersToRender.ContainsKey(id)) {
                InitiatePlayer(id, "Unknown");
            }
            var player = PlayersToRender[id];
            player.name = name;
            if(player.name.Contains(MainCharacterName)) {
                player.RenderIdentifier = "Main";
            }

        }

        public void InitiatePlayer(string id, string playerClass, bool isOnField = true) {
            PlayersToRender.Add(id, new Entity() {
                Id = id,
                RenderIdentifier = playerClass,
                IsOnField = isOnField
            });
        }

        public void SetPlayerPosition(string id, float x, float y, bool isOnField = true) {
            if (!PlayersToRender.ContainsKey(id)) {
                InitiatePlayer(id, "Unknown");
            }
            var entity = PlayersToRender[id];
            entity.X = x;
            entity.Y = y;
            entity.IsOnField = isOnField;

            MaxXPos = x;
            MaxYPos = y;
            MinXPos = x;
            MinYPos = y;
        }

        public void SetCreaturePosition(string id, float x, float y, float rotation, bool isOnField = true) {
            if (!CreaturesToRender.ContainsKey(id)) {
                CreaturesToRender[id] = new Entity() { Id = id };
            }

            var entity = CreaturesToRender[id];
            entity.X = x;
            entity.Y = y;
            // 0 rad is north from the logs, rotate half a rad clockwise and it will match our grid
            entity.Rotation = rotation + (float)(0.5*Math.PI);
            entity.IsOnField = isOnField;

            MaxXPos = x;
            MaxYPos = y;
            MinXPos = x;
            MinYPos = y;
        }

        public void RemoveCreatureToRender(string id) {
            CreaturesToRender.Remove(id);
        }

        public void PlaceWorldMarker(string id, string markerName, float x, float y, bool isOnField = true) {
            WorldMarkersToRender[id] = new Entity() {
                Id = id,
                X = x,
                Y = y,
                RenderIdentifier=markerName,
                IsOnField=isOnField
            };
            /* exclude markers from this. just render them when visible.
            MaxXPos = x;
            MaxYPos = y;
            MinXPos = x;
            MinYPos = y;
            */
        }

        public void FlagPlayerAsHighlighted(string id, (int R, int G, int B) highlightColour) {
            var player = PlayersToRender[id];
            player.IsHighlighted = true;
            player.HighlightColour = highlightColour;
        }

        public void RemovePlayerHighlight(string id) {
            var player = PlayersToRender[id];
            player.IsHighlighted = false;
        }

        public void PlaceIndicatorOnPlayerPosition(string playerId, string idOfIndicator, (int R, int G, int B) highlightColour) {
            var player = PlayersToRender[playerId];
            DebuffDropLocationsToRender.Add(new Entity() {
                Id = idOfIndicator,
                X = player.X,
                Y = player.Y,
                IsOnField = true,
                IsHighlighted = true,
                HighlightColour = highlightColour
            });
        }

        public void AddBeamOriginatingFromCreature(string effectId, string creatureId, float width, float length, (int R, int G, int B) colour) {
            BeamsOriginatingFromCreatures.Add(effectId, new BeamEntity {
                EffectId = effectId,
                OriginatingFromEntityId = creatureId,
                Width = width,
                Length = length,
                Colour = colour
            });
        }

        public void RemoveBeamOriginatingFromCreature(string effectId) {
            BeamsOriginatingFromCreatures.Remove(effectId);
        }

        public string DebugEntityPositions() {
            var sb = new StringBuilder();
            foreach (var entity in PlayersToRender) {
                sb.Append($"entity: {entity.Key}, X: {entity.Value.X}, Y: {entity.Value.Y} , rotation: {entity.Value.Rotation}\n");
            }

            foreach (var entity in CreaturesToRender) {
                sb.Append($"entity: {entity.Key}, X: {entity.Value.X}, Y: {entity.Value.Y} , rotation: {entity.Value.Rotation}\n");
            }


            return sb.ToString();
        }

        public List<Entity> GetPlayersToRender() {
            return PlayersToRender.Values.ToList();
        }

        public List<Entity> GetWorldMarkersToRender() {
            return WorldMarkersToRender.Values.ToList();
        }

        public List<Entity> GetIndicatorsToRender() {
            return DebuffDropLocationsToRender.ToList();
        }

        public List<Entity> GetCreaturesToRender() {
            return CreaturesToRender.Values.ToList();
        }

        public List<(Entity entity, BeamEntity beam)> GetBeamsFromCreaturesToRender() {
            return BeamsOriginatingFromCreatures.Values
                .Select(b => (CreaturesToRender[b.OriginatingFromEntityId], b))
                .ToList();
        }


    }

    public class Entity {
        public string Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Rotation { get; set; }
        public string RenderIdentifier { get; set; }
        public string name { get; set; }
        public bool IsOnField { get; set; } = false;
        public bool IsHighlighted { get; set; } = false;
        public (int R, int G, int B) HighlightColour { get; set; }
    }

    public class BeamEntity
    {
        public string EffectId { get; set; }
        public string OriginatingFromEntityId { get; set; }
        public float Width { get; set; }
        public float Length { get; set; }
        public (int R, int G, int B) Colour { get; set; }
    }

    public class ListEntity
    {
        public string Id { get; set; }
        public List<string> Descriptions { get; set; }
    }
}
