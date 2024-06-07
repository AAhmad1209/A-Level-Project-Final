using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Represents information about the current level
    [Serializable]
    internal class LevelData
    {
        private List<Player> _players;
        private Map _map;
        private List<MapEntity> _map_entities;
        private Player _current_player;
        private int _current_player_pointer;
        private string _game_mode;

        //constructor
        public LevelData(List<Player> players, Map map, List<MapEntity> map_entities, Player current_player, int current_player_pointer, string game_mode)
        {
            _players = players;
            _map = map;
            _map_entities = map_entities;
            _current_player = current_player;
            _current_player_pointer = current_player_pointer;
            _game_mode = game_mode;
        }

        public string Game_Mode { get => _game_mode; set => _game_mode = value; }
        public int Current_Player_Pointer { get => _current_player_pointer; set => _current_player_pointer = value; }
        public List<Player> Players { get => _players; }
        public Map Map { get => _map; }
        public List<MapEntity> Map_Entities { get => _map_entities; }
        public Player Current_Player { get => _current_player; }

    }
}
