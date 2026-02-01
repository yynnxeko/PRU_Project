using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
    public class Leveling
    {
        public string name { get; set; }
        public float level { get; set; }
        public List<string> words = new List<string>();

        public Leveling(float level)
        {
            this.level = level;
        }

        public void DetermineLevelName(float level)
        {
            switch (level)
            {
                case 1:
                    string[] levelOneWords = { "jungle", "laser", "vision", "castle", "minion" };
                    name = "The Wanderer";
                    words = new List<string>(levelOneWords);
                    break;
                case 2:
                    string[] levelTwoWords = { "illusion", "rainbow", "fantasy", "vanguard", "cauldron" };
                    name = "The Alchemist";
                    words = new List<string>(levelTwoWords);
                    break;
                case 3:
                    string[] levelThreeWords = { "fabrication", "imagination", "speculation", "magnification", "preference" };
                    name = "The Black Witch";
                    words = new List<string>(levelThreeWords);
                    break;
            }
        }
    }
}
