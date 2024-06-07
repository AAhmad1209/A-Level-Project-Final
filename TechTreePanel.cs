using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_LEVEL_PROJECT
{
    using global::Delegate_Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Delegate_Model
    {
        //This panel displays the tech tree of a player
        internal class TechTreePanel : TurnUIPanel
        {
            private string _name;
            private int _internal_x;
            private int _internal_y;
            private int _x_count;
            private int _y_count;
            private TechTree _tree;

            //constructor
            public TechTreePanel(Rectangle rectangle, string name, LevelData level_data, int depth) : base(rectangle, name, level_data, depth)
            {
                _tree = level_data.Current_Player.Tree;
                _internal_x = _rectangle.Position.X + 1;
                _internal_y = _rectangle.Position.Y + 1;
                _name = name;
                _x_count = 0;
                _y_count = 0;
            }

            //displays the contents of the panel
            public override void Display_Contents()
            {
                Console.ResetColor();

                Display_Tree();

                Console.ResetColor();
            }

            //displays the tech tree
            public void Display_Tree()
            {
                Display_Node(_tree.Root, 0);

                //represents the number of lines down an item is displayed
                _y_count = 0;
            }

            //displays a node and it's sub nodes
            private void Display_Node(TechNode node, int level)
            {
                //sets level 1 nodes
                if (level == 1)
                {
                    Console.SetCursorPosition(_internal_x, _internal_y + _y_count);
                    Console.WriteLine("");

                    _y_count++;
                }

                //unclocked nodes are are represented in green and locked nodes are represented in red
                if (node.Unlocked)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                //displays the node
                Console.SetCursorPosition(_internal_x, _internal_y + _y_count);
                Console.WriteLine($"{Get_Indentation(level)}- {node.Name}");

                _y_count++;

                //displays all of the node's child nodes recursively
                foreach (TechNode child in node.Child_Nodes)
                {
                    Display_Node(child, level + 1);
                }
            }

            //returns the indentation string given a level of the tech tree
            private string Get_Indentation(int level)
            {
                const int Spaces_Per_Level = 4;
                return new string(' ', level * Spaces_Per_Level);
            }

            //adds a selection item to the selection items list
            public void Update_Tree(TechTree new_tree)
            {
                _tree = new_tree;
            }
        }
    }

}
