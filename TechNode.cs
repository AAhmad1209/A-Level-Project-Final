using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Represents a node on a tech tree
    [Serializable]
    public class TechNode
    {
        private string _name;
        private List<TechNode> _child_nodes;
        private bool _unlocked;
        private int _cost;

        public string Name { get => _name; set => _name = value; }
        public bool Unlocked { get => _unlocked; set => _unlocked = value; }
        public List<TechNode> Child_Nodes { get => _child_nodes; set => _child_nodes = value; }
        public int Cost { get => _cost; set => _cost = value; }

        //constructor
        public TechNode(string name)
        {
            _name = name;
            _child_nodes = new List<TechNode>();
            _unlocked = false;
            _cost = 25;
        }

        //adds a child node
        public void Add_Child(TechNode child_node)
        {
            Child_Nodes.Add(child_node);
        }
    }
}
