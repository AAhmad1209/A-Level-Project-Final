using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Represents an x coordinate and a y coordinate
    [Serializable]
    internal class Coord
    {
        private int _x, _y;

        //constructor
        public Coord(int x, int y)
        {
            _x = x;
            _y = y;

        }

        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }
    }
}
