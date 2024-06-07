using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //This is a selection panel which allows a player to research different technologies
    internal class ResearchTechPanel : TurnUIPanel
    {
        private string _name;
        private int _internal_x;
        private int _internal_y;
        private List<string> _selection_list;
        private int _selection_pointer;
        private TechTree _tree;

        //constructor
        public ResearchTechPanel(Rectangle rectangle, string name, LevelData level_data, int depth) : base(rectangle, name, level_data, depth)
        {
            _selection_list = new List<string>();
            _selection_pointer = 0;
            _tree = level_data.Current_Player.Tree;
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

            //if enter is pressed return a PlayerAction that includes the tech that has been selected for research
            else if (input.Key == ConsoleKey.Enter)
            {
                try
                {
                    //checks which technology was selected and returns the corresponding player action

                    if (_selection_list[_selection_pointer] == "Riding")
                    {
                        _selection_pointer = 0;
                        return new PlayerAction("research-riding");
                    }

                    else if (_selection_list[_selection_pointer] == "Smithery")
                    {
                        _selection_pointer = 0;
                        return new PlayerAction("research-smithery");
                    }

                    else if (_selection_list[_selection_pointer] == "Strategy")
                    {
                        _selection_pointer = 0;
                        return new PlayerAction("research-strategy");
                    }

                    else if (_selection_list[_selection_pointer] == "Chivalry")
                    {
                        _selection_pointer = 0;
                        return new PlayerAction("research-chivalry");
                    }

                    else if (_selection_list[_selection_pointer] == "Diplomacy")
                    {
                        _selection_pointer = 0;
                        return new PlayerAction("research-diplomacy");
                    }
                }

                catch
                {

                }
            }

            //for any other key return null
            return null;
        }

        //updates the tech tree
        public void Update_Tree(TechTree tree)
        {
            _tree = tree;
        }

        //adds all technologies that can be currently researched by the player to the selection list 
        public void Setup_List(TechNode node)
        {
            foreach (TechNode sub_node in node.Child_Nodes)
            {
                //checks if the node is unlocked, if not the end of a branch has been reached and the next unlockable node is added to the selection list
                if (sub_node.Unlocked == false)
                {
                    Add_Selection(sub_node.Name);
                }

                //recursively continue for all subbranches of the node
                else
                {
                    Setup_List(sub_node);
                }
            }
        }

        //allows the ResearchTechPanel to reset
        public override void Reset()
        {
            _selection_pointer = 0;
        }

        //clears the panel
        public void Clear()
        {
            for (int i = 0; i < 7; i++)
            {
                Console.SetCursorPosition(_internal_x, _internal_y + i);
                Console.WriteLine("              ");
            }
        }

        //displays the selection panel items
        public override void Display_Contents()
        {
            Reset_List();
            Clear();
            Setup_List(_tree.Root);

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

                //displays the selection item with numbering
                Console.SetCursorPosition(_internal_x, _internal_y + i);
                Console.WriteLine((i + 1).ToString() + ") " + _selection_list[i]);
            }
        }

        //empties the selection list
        public void Reset_List()
        {
            _selection_list = new List<string>();
            
        }

        //adds a selection item to the selection items list
        public void Add_Selection(string item)
        {
            _selection_list.Add(item);
        }
    }
}
