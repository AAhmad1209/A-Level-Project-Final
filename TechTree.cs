using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Represents a tech tree
    [Serializable]
    public class TechTree
    {
        private Dictionary<string, TechNode> _node_reference;
        private TechNode _root;

        public TechNode Root { get => _root; set => _root = value; }

        //constructor
        public TechTree(string root_name)
        {
            _root = new TechNode(root_name);
            _root.Unlocked = true;

            _node_reference = new Dictionary<string, TechNode>();
        }

        //adds a node to the tree given it's parent
        public TechNode Add_Node(string name, TechNode parent_node)
        {
            TechNode node = new TechNode(name);

            //node reference used to find nodes quickly
            _node_reference.Add(name, node);

            parent_node.Add_Child(node);

            return node;
        }

        //unlocks a node on the tech tree
        public TechNode Unlock_Node(string name)
        {
            _node_reference.TryGetValue(name, out TechNode node);

            node.Unlocked = true;

            return node;
        }

        //checks if a node is unlockced on the tech tree
        public bool Unlocked(string node_name)
        {
            _node_reference.TryGetValue(node_name, out TechNode node);

            if (node.Unlocked == true)
            {
                return true;
            }

            return false;
        }       
    }
}
