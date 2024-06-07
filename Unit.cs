using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Represents a movable unit
    [Serializable]
    internal class Unit : MapEntity
    {
        private string _type;
        private int _hp;
        private int _defence;
        private int _attack;
        private int _movement;
        private int _cost;
        private int _capture_counter;
        private int _visibility;
        private string _marker;

        //constructor
        public Unit(Coord position, Player owner, string type) : base(position, owner, type)
        {
            //depending on the type of unit configure the attributes accordingly
            if (type == "warrior")
            {
                _hp = 20;
                _defence = 5;
                _attack = 10;
                _movement = 1;
                _cost = 10;
                _visibility = 2;
            }

            else if (type == "rider")
            {
                _hp = 30;
                _defence = 6;
                _attack = 10;
                _movement = 3;
                _cost = 20;
                _visibility = 4;
            }

            else if (type == "swordsman")
            {
                _hp = 30;
                _defence = 7;
                _attack = 15;
                _movement = 1;
                _cost = 30;
                _visibility = 2;
            }

            else if (type == "defender")
            {
                _hp = 40;
                _defence = 15;
                _attack = 5;
                _movement = 1;
                _cost = 30;
                _visibility = 5; 
            }

            else if (type == "knight")
            {
                _hp = 50;
                _defence = 10;
                _attack = 20;
                _movement = 5;
                _cost = 50;
                _visibility = 6;
            }

            _capture_counter = 0;
        }

        public int Cost { get => _cost; set => _cost = value; }
        public int HP { get => _hp; set => _hp = value; }
        public int Movement { get => _movement; set => _movement = value; }
        public int Defence { get => _defence; set => _defence = value; }
        public int Attack { get => _attack; set => _attack = value; }
        public int Capture_Counter { get => _capture_counter; set => _capture_counter = value; }
        public int Visibility { get => _visibility; set => _visibility = value; }
        public string Marker { get => _marker; set => _marker = value; }
    }
}
