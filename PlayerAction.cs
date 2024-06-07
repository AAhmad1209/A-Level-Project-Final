using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Represents a player action
    internal class PlayerAction
    {
        private string _type;
        private Coord _map_cursor;
        private Unit _unit;

        //constructor
        public PlayerAction(string type)
        {
            _type = type;
            _map_cursor = null;
            
        }

        public string Type { get => _type; set => _type = value; }
        public Coord Map_Cursor { get => _map_cursor; set => _map_cursor = value; }
        public Unit Unit { get => _unit; set => _unit = value; }
    }
}
