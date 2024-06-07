using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Manages the in-game UI
    internal class GameUI
    {
        private LevelData _level_data;
        private TurnUI _turn_UI;

        //used to obtain LevelData objects from the GameLevel class
        public delegate LevelData LevelDataRequestEventHandler();
        public event LevelDataRequestEventHandler OnLevelDataRequest;

        //used to send PlayerAction objects to the GameLevel class
        public delegate string PlayerActionRequestEventHandler(PlayerAction action);
        public event PlayerActionRequestEventHandler OnPlayerActionRequest;

        //used to notify the GameLevel class to end the turn and subsequently change the player
        public delegate void EndTurnRequestEventHandler();
        public event EndTurnRequestEventHandler OnEndTurnRequest;


        //constructor
        public GameUI()
        {
          
        }

        //gets the current level data by invoking the OnLevelDataRequest event
        public LevelData Get_level_Data()
        {
            return OnLevelDataRequest.Invoke();
        }

        //ends the current turn by invoking the OnEndTurnRequest event
        public void End_Turn()
        {
            OnEndTurnRequest.Invoke();
        }

        //requests player actions by invoking the OnPlayerActionRequest event
        public string Request_Player_Action(PlayerAction action)
        {
            return OnPlayerActionRequest.Invoke(action);
        }

        //updates the level data attribute
        public void Update_Level_Data()
        {
            _level_data = Get_level_Data();
        }

        //displays top UI bar
        public void Display_UI_Bar()
        {
            Console.SetCursorPosition(1, 1);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(1, 1);
            Console.Write("Current Player: " + _level_data.Current_Player.Name + " ||| Money: " + _level_data.Current_Player.Money + " ||| Output: " + _level_data.Current_Player.Total_Output);
        }

        //main UI loop
        public void UI_Loop()
        {
            //updates the level data initially to match the copy the GameLevel object has
            Update_Level_Data();

            //creates a new TurnUI object
            _turn_UI = new TurnUI(_level_data);

            Console.WriteLine("");
            
            //UI loop
            while (true)
            {
                //updates the level data
                Update_Level_Data();

                //checks if the player is an AI player, if so then does not display the turn UI
                //AI players do not need UI their behavior is automated by algorithms in the GameLevel class
                if (_level_data.Current_Player.Type == "AI")
                {
                    //creates a new PlayerAction with a type called AI-turn
                    //this is sent to the GameLevel class so the AI turn can be completed by the appropriate methods
                    PlayerAction AI_action = new PlayerAction("AI-turn");
                    Request_Player_Action(AI_action);

                    //ends the turn
                    End_Turn();

                    //updates the turn UI automatically
                    _turn_UI.Update(new ConsoleKeyInfo('Q', ConsoleKey.Q, false, false, false), Get_level_Data());

                    continue;
                }

                //displays top UI bar
                Display_UI_Bar();

                //displays main turn UI
                _turn_UI.Display();

                //obtains key presses from the player, updates the turn UI and returns the corresponding player action
                ConsoleKeyInfo press = Console.ReadKey();
                PlayerAction action = _turn_UI.Update(press, Get_level_Data());

                //checks if the action has not already been completed by the TurnUI class / more needs to be done
                if (action != null)
                {
                    //if the player wants to end the turn
                    if (action.Type == "end-turn")
                    {
                        End_Turn();
                        Update_Level_Data();
                        _turn_UI.Reset();

                        //updates the turn UI automatically
                        _turn_UI.Update(new ConsoleKeyInfo('Q', ConsoleKey.Q, false, false, false), Get_level_Data());
                    }

                    //if the player wants to move a unit
                    else if (action.Type == "move-unit")
                    {
                        /*the player requests to move a given unit on the map and the GameLevel class sends back whether the UI can proceed
                         to the next step depending on the current map cursor position*/
                        string action_response = Request_Player_Action(action);

                        //if the unit selection is successful then the selected unit will be marked
                        if (action_response == "highlight-unit")
                        {
                            _turn_UI.Mark_Cursor_Tile();

                            /*in this state the turn UI locks to the map window and the player has to move the final position cursor to select
                             the initially selected unit's final position*/
                            _turn_UI.Unit_Final_Position_Selection_State();

                            bool move_finished = false;

                            //updates turn UI every time a player moves the final position cursor
                            while (move_finished == false)
                            {
                                _turn_UI.Display();
                                ConsoleKeyInfo position_selection_press = Console.ReadKey();

                                //if the player hits enter the loop is broken
                                if (position_selection_press.Key == ConsoleKey.Enter)
                                {
                                    //sets the move to finished
                                    move_finished = true;

                                    //creates a new player action to be sent to the GameLevel class
                                    PlayerAction position_selection_press_action = new PlayerAction("final-unit-move");

                                    //adds map cursor data to the player action
                                    position_selection_press_action.Map_Cursor = _turn_UI.Get_Map_Cursor();

                                    //requests action
                                    string selection_response = Request_Player_Action(position_selection_press_action);

                                    break;
                                }

                                //updates the turn UI
                                _turn_UI.Update(position_selection_press, Get_level_Data());

                            }

                            //the turn UI is reset to it's default state and the cursor marked on the map is unmarked
                            _turn_UI.Undo_Unit_Final_Position_Selection_State();
                            _turn_UI.Unmark_Cursor_Tile();
                        }

                        //updates the turn UI automatically
                        _turn_UI.Update(new ConsoleKeyInfo('Q', ConsoleKey.Q, false, false, false), Get_level_Data());

                    }

                    //any other type of player action is requested
                    else
                    {
                        string action_repsonse = Request_Player_Action(action);
                        
                        //level data is updated
                        Update_Level_Data();
                    } 
                }

                //level data is updated
                Update_Level_Data();
              
            }
        }

        //updates the status panel with a given status message
        public void Update_Status_Panel(string status_message)
        {
            _turn_UI.Update_Status(status_message);

            //updates the turn UI automatically
            _turn_UI.Update(new ConsoleKeyInfo('Q', ConsoleKey.Q, false, false, false), Get_level_Data());
        }
    }
}
