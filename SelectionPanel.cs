using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //This is a selection panel which allows a player to select an option making up part of the turn UI
    internal class SelectionPanel : TurnUIPanel
    {
        private string _name;
        private int _internal_x;
        private int _internal_y;
        private List<string> _selection_list;
        private int _selection_pointer;

        //constructor
        public SelectionPanel(Rectangle rectangle, string name, LevelData level_data, int depth) : base(rectangle, name, level_data, depth)
        {
            _selection_list = new List<string>();
            _selection_pointer = 0;
            _internal_x = _rectangle.Position.X + 1;
            _internal_y = _rectangle.Position.Y + 1;
            _name = name;

        }

        //updates the currently selected item based on the key presses of the user
        public override PlayerAction Update(ConsoleKeyInfo input)
        {
            //if the up or down arrow key is pressed change the selected option
            if ((input.Key == ConsoleKey.DownArrow || input.Key == ConsoleKey.S) && (_selection_pointer + 1 < _selection_list.Count))
            {
                _selection_pointer += 1;
            }

            else if ((input.Key == ConsoleKey.UpArrow || input.Key == ConsoleKey.W) && (_selection_pointer - 1 > -1))
            {
                _selection_pointer -= 1;
            }

            //if the up arrow is pressed at the top of the selection list change the selection pointer to point to the bottom selection
            else if (_selection_pointer == 0 && input.Key == ConsoleKey.UpArrow)
            {
                _selection_pointer = _selection_list.Count - 1;
            }

            //if the down arrow is pressed at the bottom of the selection list change the selection pointer to point to the top selection
            else if (_selection_pointer == _selection_list.Count - 1 && input.Key == ConsoleKey.DownArrow)
            {
                _selection_pointer = 0;
            }

            //if enter is pressed return a PlayerAction corresponding to the selected menu item
            else if (input.Key == ConsoleKey.Enter)
            {
                if (_selection_list[_selection_pointer] == "Action Panel")
                {
                    return new PlayerAction("display-actionpanel");
                }

                else if (_selection_list[_selection_pointer] == "Tech Tree")
                {
                    return new PlayerAction("display-techtree");
                }

                else if (_selection_list[_selection_pointer] == "Summon Unit")
                {
                    return new PlayerAction("display-summonunitpanel");
                }

                else if (_selection_list[_selection_pointer] == "Warrior")
                {
                    return new PlayerAction("summon-warrior");
                }

                else if (_selection_list[_selection_pointer] == "Rider")
                {
                    return new PlayerAction("summon-rider");
                }

                else if (_selection_list[_selection_pointer] == "Defender")
                {
                    return new PlayerAction("summon-defender");
                }

                else if (_selection_list[_selection_pointer] == "Swordsman")
                {
                    return new PlayerAction("summon-swordsman");
                }

                else if (_selection_list[_selection_pointer] == "Knight")
                {
                    return new PlayerAction("summon-knight");
                }

                else if (_selection_list[_selection_pointer] == "Move Unit")
                {
                    return new PlayerAction("move-unit");
                }

                else if (_selection_list[_selection_pointer] == "End Turn")
                {
                    return new PlayerAction("end-turn");
                }

                else if (_selection_list[_selection_pointer] == "Save Game")
                {
                    return new PlayerAction("save");
                }
            }

            //for any other key return null
            return null;
        }

        //allows the SelectionPanel to reset
        public override void Reset()
        {
            _selection_pointer = 0;
        }

        //displays the selection panel items
        public override void Display_Contents()
        {
            Console.ResetColor();

            for (int i = 0; i < _selection_list.Count; i++)
            {
                if (_selection_pointer == i)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.SetCursorPosition(_internal_x, _internal_y + i);

                //displays the selection item with numbering
                Console.WriteLine((i + 1).ToString() + ") " + _selection_list[i]);
            }
        }

        //adds a selection item to the selection items list
        public void Add_Selection(string item)
        {
            _selection_list.Add(item);
        }
    }
}
