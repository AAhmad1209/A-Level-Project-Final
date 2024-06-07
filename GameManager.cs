using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Manages the delegate subscriptions between the GameLevel and GameUI classes
    internal class GameManager
    {
        private GameLevel _game_level;
        private GameUI _game_UI;

        //constructor for a new game
        public GameManager(List<Player> players, string map_path, string game_mode)
        {
            _game_level = new GameLevel(players, map_path, game_mode);
            _game_UI = new GameUI();

            Setup_Subscriptions();
        }

        //constructor for resuming a game
        public GameManager(LevelData level_data)
        {
            _game_level = new GameLevel(level_data);
            _game_UI = new GameUI();

            Setup_Subscriptions();
        }

        //sets up the subscriptions between the GameLevel and GameUI classes
        public void Setup_Subscriptions()
        {
            //used to pass level data from the GameLevel class to the GameUI class
            _game_UI.OnLevelDataRequest += _game_level.Get_Level_Data;

            //used to pass player actions from the GameUI class to the GameLevel class
            _game_UI.OnPlayerActionRequest += _game_level.Handle_Player_Action;

            //used to send an end turn request from the GameUI class to the GameLevel class
            _game_UI.OnEndTurnRequest += _game_level.End_Turn;

            //used to pass a status message from the GameLevel class to the GameUI class
            _game_level.OnStatusUpdateRequest += _game_UI.Update_Status_Panel;

        }

        //starts the game
        public void Play()
        {
            //initiates UI loop
            _game_UI.UI_Loop();
        }
    }
}
