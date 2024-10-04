using System;
using System.Collections.Generic;
using System.Linq;
using Assets.HeroEditor4D.Common.Scripts.Common;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.HeroEditor4D.Common.Scripts.Data
{
	/// <summary>
	/// Represents sprite entry in SpriteCollection.
	/// </summary>
	[Serializable]
	public class ItemSprite
	{
        public string Name;
        public string Id;
        public string Edition;
        public string Collection;
        public string Path;
		public Sprite Sprite;
		public List<Sprite> Sprites;
		public List<string> Tags = new List<string>();
        public string Meta;

        public Dictionary<string, string> MetaDict
        {
            get => Meta == "" ? new Dictionary<string, string>() : JsonConvert.DeserializeObject<Dictionary<string, string>>(Meta);
            set => Meta = JsonConvert.SerializeObject(value);
        }

        public ItemSprite(string edition, string collection, string type, string name, string path, Sprite sprite, List<Sprite> sprites)
        {
            Id = ItemIdGenerator.GetNextId(type); // Generate sequential ID based on type

            //Id = $"{edition}.{collection}.{type}.{name}";

            if (sprites == null || sprites.Count == 0)
            {
                throw new Exception($"Please set [Texture Type = Sprite] for [{Id}] from Import Settings!");
            }

            Name = name;
            Collection = collection;
            Edition = edition;
            Path = path;
            Sprite = sprite;
            Sprites = sprites.OrderBy(i => i.name).ToList();
        }

		public Sprite GetSprite(string name)
		{
			return Sprites.Single(j => j.name == name);
		}

        public static class ItemIdGenerator
        {
            private static int armorIdCounter = 0;
            private static int helmetIdCounter = 0;

            public static string GetNextId(string itemType)
            {
                switch (itemType)
                {
                    case "Armor":
                        return $"AID{++armorIdCounter:00}";
                    case "Helmet":
                        return $"HID{++helmetIdCounter:00}";
                    default:
                        throw new ArgumentException("Unknown item type");
                }
            }
        }


    }
}