using System;
using System.Collections.Generic;
using System.Linq;

namespace yellowyears.uGoldSrc.Formats.BSP.Types
{
    [Serializable]
    public class Entity
    {
        /// <summary>
        /// The entity as it appears in the file, no parsing
        /// </summary>
        public string rawEntity;

        /// <summary>
        /// The classname points to the "type" of entity it is e.g monster_scientist or func_door
        /// </summary>
        public string className;

        /// <summary>
        /// The attributes contain a key and a value
        /// </summary>
        public List<Attribute> attributes = new List<Attribute>();

        private List<Attribute> ParseEntity(string rawEntity)
        {
            // Replace empty keys/attributes with "null" to stop errors
            rawEntity = rawEntity.Replace("\"\"", "NULL");

            var lines = rawEntity.Split('"').ToList();

            // Remove all null characters and whitespace entries in the array
            lines = Utilities.CleanStringList(lines);

            var attributes = new List<Attribute>();

            // Half the length as there are two lines for each attribute
            for (int i = 0; i < lines.Count / 2; i++)
            {
                // There are twice as many lines as attributes
                int j = i * 2;

                var key = lines[j];
                var value = lines[j + 1];

                // If the key is a classname, then we don't want to put this in the attributes
                if (key == "classname")
                {
                    className = value;
                    continue;
                }

                // The value is always one higher than the key
                var attribute = new Attribute(key, value);
                attributes.Add(attribute);
            }

            return attributes;
        }

        public Entity(string rawEntity)
        {
            this.rawEntity = rawEntity;
            this.attributes = ParseEntity(rawEntity);
        }

        [Serializable]
        public struct Attribute
        {
            public string key;
            public string value;

            public Attribute (string key, string value)
            {
                this.key = key;
                this.value = value;
            }
        }

    }
}