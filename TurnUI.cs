using A_LEVEL_PROJECT.Delegate_Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //The primary in-game UI that the player will be interacting with
    internal class TurnUI
    {
        private TurnUIPanel[] _panels;
        private int _current_panel_pointer;

        //constructor
        public TurnUI(LevelData level_data)
        {
            _panels = new TurnUIPanel[8];

            //creates all panels required for turn UI
            MapPanel map_panel = new MapPanel(new Rectangle(24, 18, new Coord(1, 3)), "Map", level_data, 0);
            MapInfoPanel map_info_panel = new MapInfoPanel(new Rectangle(45, 15, new Coord(1, 22)), "Map-Info", level_data, 0);
            TurnStatusPanel turn_status_panel = new TurnStatusPanel(new Rectangle(45, 7, new Coord(1, 38)), "Status", level_data, "", 0);
            SelectionPanel command_panel = new SelectionPanel(new Rectangle(20, 18, new Coord(26, 3)), "Command-Centre", level_data, 0);
            SelectionPanel action_panel = new SelectionPanel(new Rectangle(24, 18, new Coord(47, 3)), "Action-Panel", level_data, 1);
            SelectionPanel summon_unit_panel = new SelectionPanel(new Rectangle(24, 18, new Coord(72, 3)), "Summon-Unit", level_data, 2);
            TechTreePanel tech_tree_panel = new TechTreePanel(new Rectangle(24, 15, new Coord(47, 22)), "Tech-Tree", level_data, 1);
            ResearchTechPanel research_tech_panel = new ResearchTechPanel(new Rectangle(24, 18, new Coord(47, 3)), "Research-Tech", level_data, 1);

            //assigns all panels to the _panels array
            _panels[0] = map_panel;
            _panels[1] = map_info_panel;
            _panels[2] = command_panel;
            _panels[3] = action_panel;
            _panels[4] = summon_unit_panel;
            _panels[5] = turn_status_panel;
            _panels[6] = tech_tree_panel;
            _panels[7] = research_tech_panel;

            //sets the map info panel, turn status panel and the tech tree panel as not interactive
            _panels[1].Interactive = false;
            _panels[5].Interactive = false;
            _panels[6].Interactive = false;

            //sets the map panel, map info panel, command panel and the turn status panel as initially visible
            _panels[0].Visible = true;
            _panels[1].Visible = true;
            _panels[2].Visible = true;
            _panels[5].Visible = true;

            //sets the action panel and the summon unit panel as initially invisble
            _panels[3].Visible = false;
            _panels[4].Visible = false;

            //sets the map panel as the initally selected panel
            _current_panel_pointer = 0;
            _panels[_current_panel_pointer].Active = true;

            //adds selections to command_panel
            command_panel.Add_Selection("Action Panel");
            command_panel.Add_Selection("Tech Tree");
            command_panel.Add_Selection("Save Game");
            command_panel.Add_Selection("End Turn");

            //adds selections to action_panel
            action_panel.Add_Selection("Summon Unit");
            action_panel.Add_Selection("Move Unit");

            //adds selections to summon unit panel
            summon_unit_panel.Add_Selection("Warrior");
            summon_unit_panel.Add_Selection("Rider");
            summon_unit_panel.Add_Selection("Swordsman");
            summon_unit_panel.Add_Selection("Defender");
            summon_unit_panel.Add_Selection("Knight");

        }

        //updates the panel selected if the user has pressed a specific key
        public PlayerAction Update(ConsoleKeyInfo press_info, LevelData level_data)
        {
            //tab key changes the current panel to the next one
            if (press_info.Key == ConsoleKey.Tab)
            {
                //current panel is set to no longer active
                _panels[_current_panel_pointer].Active = false;

                bool found = false;
                int new_panel_pointer = 0;

                //finds the next suitable panel which is interactive and visible
                for (int i = _current_panel_pointer + 1 ; i < _panels.Count(); i++)
                {
                    if (_panels[i].Interactive == true && _panels[i].Visible == true)
                    {
                        //once the panel is found assigns the position of the new panel to the new panel pointer variable
                        found = true;
                        new_panel_pointer = i;
                        break;
                    }
                }

                //changes current panel to new panel and makes it active
                _current_panel_pointer = new_panel_pointer;
                _panels[_current_panel_pointer].Active = true;
            }

            //retrieves action from panel key press
            PlayerAction action = _panels[_current_panel_pointer].Update(press_info);

            MapInfoPanel map_info_panel = (MapInfoPanel)_panels[1];
            MapPanel map_panel = (MapPanel)_panels[0];
            TechTreePanel tech_tree_panel = (TechTreePanel)_panels[6];
            ResearchTechPanel research_tech_panel = (ResearchTechPanel)_panels[7];

            //updates map info panel's level data
            map_info_panel.Update_Level_Data(level_data);

            //updates map panel's current player and map entities
            map_panel.Update_Current_Player(level_data.Current_Player);
            map_panel.Update_Map_Entities(level_data.Map_Entities);

            //updates tech tree panel's tech tree
            tech_tree_panel.Update_Tree(level_data.Current_Player.Tree);

            //updates research tech panel's tech tree
            research_tech_panel.Update_Tree(level_data.Current_Player.Tree);

            //obtains cursor from map panel and passes it to the map info panel
            Coord cursor = map_panel.Get_Cursor();
            map_info_panel.Set_Map_Cursor_Pos(cursor);

            //checks if the action is not null
            //any actions that can be dealt with within this class related to UI are dealt with and others are passed up call stack
            if (action != null)
            {

                //assigns latest map cursor to player action
                action.Map_Cursor = cursor;

                //displays action panel
                if (action.Type == "display-actionpanel")
                {
                    Collapse_Panels();

                    _panels[3].Visible = true;

                    _panels[_current_panel_pointer].Active = false;
                    _current_panel_pointer = 3;
                    _panels[3].Active = true;

                    return null;
                }

                //displays summon unit panel
                else if (action.Type == "display-summonunitpanel")
                {
                    _panels[4].Visible = true;

                    _panels[_current_panel_pointer].Active = false;
                    _current_panel_pointer = 4;
                    _panels[4].Active = true;

                    return null;
                }

                //displays tech tree
                else if (action.Type == "display-techtree")
                {
                    Collapse_Panels();

                    _panels[6].Visible = true;
                    _panels[7].Visible = true;

                    _panels[_current_panel_pointer].Active = false;
                    _current_panel_pointer = 7;
                    _panels[7].Active = true;

                    return null;
                }

                //return action if it is a summon
                else if (action.Type.Contains("summon"))
                {
                    return action;
                }

                //return action if it is research
                else if (action.Type.Contains("research"))
                {
                    return action;
                }

                //return action if it is to save the game
                else if (action.Type == "save")
                {
                    return action;
                }
            }

            //otherwise return action
            return action;
        }

        //collapses all panels in the turn UI that have a depth greater than 0
        public void Collapse_Panels()
        {
            foreach (TurnUIPanel panel in _panels)
            {
                if (panel.Depth > 0)
                {
                    panel.Visible = false;
                    panel.Active = false;
                }
            }

            _panels[_current_panel_pointer].Active = false;
            _current_panel_pointer = 0;
            _panels[_current_panel_pointer].Active = true;

            //clears right hand side of the screen where other panels were
            for (int i = 0; i < 49; i++)
            {
                Console.SetCursorPosition(46, i);
                Console.WriteLine("                                                   ");
            }
        }

        //displays all of the panels in the TurnUI
        public void Display()
        {

            Console.CursorVisible = false;

            foreach (TurnUIPanel panel in _panels)
            {
                if (panel.Visible == true)
                {
                    panel.Display();
                }
                
            }
        }

        //resets TurnUI for new turn
        public void Reset()
        {
            
            Console.CursorVisible = false;

            //loops through all panels and resets them individually
            foreach (TurnUIPanel panel in _panels)
            {
                panel.Reset();
            }

            //deactivates current panel
            _panels[_current_panel_pointer].Active = false;

            //changes the current panel to the first panel and activates it
            _current_panel_pointer = 0;
            _panels[_current_panel_pointer].Active = true;

            //makes panels of a depth greater than zero invisible
            foreach (TurnUIPanel panel in _panels)
            {
                if (panel.Depth > 0)
                { 
                    panel.Visible = false;
                }
            }

            //clears right hand side of the screen
            for (int  i = 0; i < 49; i++)
            {
                Console.SetCursorPosition(46, i);
                Console.WriteLine("                                                   ");
            }
        }

        //marks a tile on the map
        public void Mark_Cursor_Tile()
        {
            MapPanel map_panel = (MapPanel)_panels[0];
            map_panel.Mark_Cursor_Tile();
        }

        //unmarks a tile on the map
        public void Unmark_Cursor_Tile()
        {
            MapPanel map_panel = (MapPanel)_panels[0];
            map_panel.Unmark_Cursor_Tile();
        }

        //modifies turn UI to allow a player to select a unit's final position on the map
        public void Unit_Final_Position_Selection_State()
        {
            _panels[2].Interactive = false;
            _panels[3].Interactive = false;
            _panels[4].Interactive = false;
            _panels[0].Active = true;
            _panels[_current_panel_pointer].Active = false;
            _current_panel_pointer = 0;  
        }

        //undoes changes made by the Unit_Final_Position_Selection_State method
        public void Undo_Unit_Final_Position_Selection_State()
        {
            _panels[2].Interactive = true;
            _panels[3].Interactive = true;
            _panels[4].Interactive = true;
            _panels[0].Active = true;
            _current_panel_pointer = 0;
        }

        //gets the map cursor
        public Coord Get_Map_Cursor()
        {
            return ((MapPanel)_panels[0]).Get_Cursor();
        }

        //updates the status message
        public void Update_Status(string response)
        {
            TurnStatusPanel status_panel = (TurnStatusPanel)_panels[5];
            status_panel.Replace_Message(response);
        }
    }
}