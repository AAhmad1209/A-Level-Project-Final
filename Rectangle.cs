using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Represents a rectangle with a top left coordinate, width and height
    internal class Rectangle
    {
        private Coord _position;
        private int _width;
        private int _height;

        //constructor
        public Rectangle(int width, int height, Coord pos)
        {
            _width = width;
            _height = height;
            _position = pos;
        }

        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }
        internal Coord Position { get => _position; set => _position = value; }
    }
}
