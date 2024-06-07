using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Represents an entity on a game map
    [Serializable]
    internal class MapEntity
    {
        private Coord _position;
        private Player _owner;
        private string _type;

        //constructor
        public MapEntity(Coord position, Player owner, string type)
        {
            _type = type;
            _position = position;
            _owner = owner;
        }

        public Coord Position { get => _position; set => _position = value; }
        public string Type { get => _type; }
        public Player Owner { get => _owner; set => _owner = value; }
    }
        
}
