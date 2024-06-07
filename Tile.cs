using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Represents a tile on a map
    [Serializable]
    internal class Tile
    {
        private string _type;
        private bool _accessible;
        private List<Tile> _neighbors;
        private Tile _parent;
        private int _x;
        private int _y;

        //constructor
        public Tile(string type, int x, int y)
        {
            _type = type;
            _neighbors = new List<Tile>();
            _x = x;
            _y = y;
        }

        public string Type { get => _type; set => _type = value; }
        public bool Accessible { get => _accessible; set => _accessible = value; }
        public List<Tile> Neighbors { get => _neighbors; set => _neighbors = value; }
        public Tile Parent { get => _parent; set => _parent = value; }
        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }
    }
}
