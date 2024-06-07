using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Represents an interactive panel on the turn UI
    internal class TurnUIPanel
    {
        protected Rectangle _rectangle;
        private bool _active;
        private int _internal_x;
        private int _internal_y;
        private string _name;
        private int _depth;
        private bool _interactive;
        private bool _visible;
      
        //constructor
        public TurnUIPanel(Rectangle rectangle, string name, LevelData level_data, int depth)
        {
            _depth = depth;
            _rectangle = rectangle;
            _rectangle.Width--;
            _rectangle.Height--;
            _interactive = true;
            _internal_x = _rectangle.Position.X + 1;
            _internal_y = _rectangle.Position.Y + 1;
            _name = name;  
        }

        public bool Active { get => _active; set => _active = value; }
        public string Name { get => _name; set => _name = value; }
        public bool Interactive { get => _interactive; set => _interactive = value; }
        public bool Visible { get => _visible; set => _visible = value; }
        public int Depth { get => _depth; set => _depth = value; }

        //allows all child classes of TurnUIPanel to add unique update functionality based on user key presses
        public virtual PlayerAction Update(ConsoleKeyInfo input)
        {
            return null;
        }

        //allows all child classes of TurnUIPanel to reset appropriately
        public virtual void Reset()
        {

        }

        //displays the complete TurnUIPanel (it's border and contents)
        public void Display()
        {
            Display_Border();
            Display_Contents();
        }

        //allows all child classes of TurnUIPanel to display unique content
        public virtual void Display_Contents()
        {

        }

        //displays the outer border of all TurnUIPanel objects according to their respective dimensions
        protected void Display_Border()
        {
            //active panels have a blue border
            if (_active == true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }

            //non-active panels have a white border
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.SetCursorPosition(_rectangle.Position.X, _rectangle.Position.Y);

            List<Coord> corners = new List<Coord>();

            //defines corners of border
            Coord top_left_corner = new Coord(_rectangle.Position.X, _rectangle.Position.Y);
            Coord top_right_corner = new Coord(_rectangle.Position.X + _rectangle.Width, _rectangle.Position.Y);
            Coord bottom_right_corner = new Coord(_rectangle.Position.X + _rectangle.Width, _rectangle.Position.Y + _rectangle.Height);
            Coord bottom_left_corner = new Coord(_rectangle.Position.X, _rectangle.Position.Y + _rectangle.Height);

            corners.Add(top_right_corner);
            corners.Add(top_left_corner);
            corners.Add(bottom_left_corner);
            corners.Add(bottom_right_corner);

            //ensures panel name does not make the total number of characters displayed on the top half exceed the width of the rectangle
            int width_count = 0;

            //displays top side of border
            for (int i = _rectangle.Position.X; i < _rectangle.Position.X + _rectangle.Width + 1; i++)
            {
                int clearance = _rectangle.Width - 3;

                //displays panel name if there is enough space given the dimensions of the panel
                if (width_count == 2 && clearance - Name.Length >= 0)
                {
                    Console.Write(_name);
                    i = i + _name.Length;
                }

                Coord current = new Coord(i, _rectangle.Position.Y);
                bool is_corner = false;

                //checks if the coordinate is the corner
                foreach (Coord corner in corners)
                {
                    if (corner.X == current.X && corner.Y == current.Y)
                    {
                        is_corner = true;
                    }
                }

                Console.SetCursorPosition(i, _rectangle.Position.Y);

                //displays the corner differently
                if (is_corner == true)
                {
                    Console.Write("+");
                }

                else
                {
                    Console.Write("-");
                }

                width_count++;
            }

            //displays bottom side of border
            for (int i = _rectangle.Position.X; i < _rectangle.Position.X + _rectangle.Width + 1; i++)
            {
                Coord current = new Coord(i, _rectangle.Position.Y + _rectangle.Height);
                bool is_corner = false;

                //checks if the coordinate is the corner
                foreach (Coord corner in corners)
                {
                    if (corner.X == current.X && corner.Y == current.Y)
                    {
                        is_corner = true;
                    }
                }

                Console.SetCursorPosition(i, _rectangle.Position.Y + _rectangle.Height);

                //displays the corner differently
                if (is_corner == true)
                {
                    Console.Write("+");
                }

                else
                {
                    Console.Write("-");
                }
            }

            //displays left side of border
            for (int i = _rectangle.Position.Y; i < _rectangle.Position.Y + _rectangle.Height + 1; i++)
            {
                Coord current = new Coord(_rectangle.Position.X, i);
                bool is_corner = false;

                //checks if the coordinate is the corner
                foreach (Coord corner in corners)
                {
                    if (corner.X == current.X && corner.Y == current.Y)
                    {
                        is_corner = true;
                    }
                }

                Console.SetCursorPosition(_rectangle.Position.X, i);

                //displays the corner differently
                if (is_corner == true)
                {
                    Console.Write("+");
                }

                else
                {
                    Console.Write("|");
                }
            }

            //displays right side of border
            for (int i = _rectangle.Position.Y; i < _rectangle.Position.Y + _rectangle.Height + 1; i++)
            {
                Coord current = new Coord(_rectangle.Position.X + _rectangle.Width, i);
                bool is_corner = false;

                //checks if the coordinate is the corner
                foreach (Coord corner in corners)
                {
                    if (corner.X == current.X && corner.Y == current.Y)
                    {
                        is_corner = true;
                    }
                }

                Console.SetCursorPosition(_rectangle.Position.X + _rectangle.Width, i);

                //displays the corner differently
                if (is_corner == true)
                {
                    Console.Write("+");
                }

                else
                {
                    Console.Write("|");
                }
            }
        }

    }
}
