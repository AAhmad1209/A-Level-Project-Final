using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Represents an in-game player
    [Serializable]
    internal class Player
    {
        private string _name;
        private int _money;
        private int[,] _cloud_cover;
        private int _total_output;
        private string _type;
        private TechTree _tree;
        private string _difficulty;

        //constructor
        public Player(string name, int start_money, string type, TechTree tree)
        {
            _name = name;
            _money = start_money;
            _total_output = 20;
            _type = type;
            _tree = tree;
            _difficulty = null;
        }

        //sets a player's cloud cover
        public void Set_Cloud_Cover(int[,] cloud_cover)
        {
            _cloud_cover = cloud_cover;
        }

        public string Name { get => _name; }
        public int Money { get => _money; set => _money = value; }
        public int[,] Cloud_Cover { get => _cloud_cover; set => _cloud_cover = value; }
        public int Total_Output { get => _total_output; set => _total_output = value; }
        public string Type { get => _type; set => _type = value; }
        public TechTree Tree { get => _tree; set => _tree = value; }
        public string Difficulty { get => _difficulty; set => _difficulty = value; }
    }
}
