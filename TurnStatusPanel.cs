using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //This is an information panel which displays turn status information to the player
    class TurnStatusPanel : TurnUIPanel
    {
        private string _name;
        private int _internal_x;
        private int _internal_y;
        private LevelData _level_data;
        private string _message;

        //constructor
        public TurnStatusPanel(Rectangle rectangle, string name, LevelData level_data, string message, int depth) : base(rectangle, name, level_data, depth)
        {
            _level_data = level_data;
            _message = message;
            _internal_x = _rectangle.Position.X + 1;
            _internal_y = _rectangle.Position.Y + 1;
            _name = name;
        }

        //allows the turn status panel to reset
        public override void Reset()
        {
            _message = "";
        }

        //displays information to the player inside the turn status panel
        public override void Display_Contents()
        {
            Clear();
            Console.ResetColor();

            Console.SetCursorPosition(_internal_x, _internal_y);

            Console.WriteLine(_message);
        }

        //clears the message inside the turn status panel
        public void Clear()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.SetCursorPosition(_internal_x, _internal_y + i);
                Console.WriteLine("                                    ");
            }
        }

        //changes message inside the turn status panel
        public void Replace_Message(string new_message)
        {
            _message = new_message;
        }
    }
}
