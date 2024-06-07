using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Represents a village
    [Serializable]
    internal class Village : MapEntity
    {
        private int _xp;
        private int _level;
        private int _output;

        //constructor
        public Village(Coord position, Player owner, string type) : base(position, owner, type)
        {
            //all villages give an output of 20 per turn
            _output = 20;
        }

        public int Output { get => _output; set => _output = value; }
    }
}
