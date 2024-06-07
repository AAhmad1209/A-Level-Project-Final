using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Represents a game overall, which includes the initial game configuration
    internal class Game
    {
        string _game_mode;
        string _map_path;
        List<Player> _players;
        GameManager _manager;

        //constructor for a new game
        public Game(string game_mode, List<Player> players, string map_path)
        {
            _players = players;
            _map_path = map_path;
            _game_mode = game_mode;
            _manager = new GameManager(players, map_path, "domination");
        }

        //constructor for resuming a game
        public Game(LevelData level_data)
        {
            _manager = new GameManager(level_data);
        }

        //calls the play method in the manager object to begin the game
        public void Play()
        {
            _manager.Play();
        }

    }
}
