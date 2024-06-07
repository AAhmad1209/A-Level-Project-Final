using A_LEVEL_PROJECT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Represents a game level
    internal class GameLevel
    {
        private List<Player> _players;
        private Map _map;
        private List<MapEntity> _map_entities;
        private Player _current_player;
        private int _current_player_pointer;
        private string _game_mode;

        private Player _winner;

        private Coord _initial_unit_pos;

        //this delegate is used to pass a status_message to the Game_UI class so the UI can give an instant message to the player
        public delegate void StatusUpdateRequestEventHandler(string status_message);
        public event StatusUpdateRequestEventHandler OnStatusUpdateRequest;

        //enum for unit costs
        public enum Unit_Cost
        {
            Warrior = 10,
            Rider = 20,
            Swordsman = 30,
            Knight = 50,
            Defender = 30
        }

        //constructor for a new game
        public GameLevel(List<Player> players, string map_path, string game_mode)
        {
            _game_mode = game_mode;
            _players = players;
            _map_entities = new List<MapEntity>();
            _current_player = players[0];
            _current_player_pointer = 0;

            Setup_Level(players, map_path);
        }
        
        //constructor for resuming a game
        public GameLevel(LevelData level_data)
        {
            _game_mode = level_data.Game_Mode;
            _players = level_data.Players;
            _map_entities = level_data.Map_Entities;
            _current_player = level_data.Current_Player;
            _current_player_pointer = level_data.Current_Player_Pointer;
            _map = level_data.Map;
        }

        //loads the map, creates a capital village for each player and configures the tech tree of each player
        public void Setup_Level(List<Player> players, string map_path)
        {
            Load_Map(map_path);

            Assign_Capitals();

            Generate_Unclaimed_Villages();

            Create_Tech_Trees();
        }

        //assigns a Map object to the _map attribute using the map_path
        public void Load_Map(string map_path)
        {
            _map = new Map("map", map_path);
        }

        //assigns the capital villages to each player and sets up the initial cloud cover for each player accordingly
        public void Assign_Capitals()
        {
            List<Coord> capital_positions = new List<Coord>();

            capital_positions.Add(new Coord(1, 1));
            capital_positions.Add(new Coord(1, 6));
            capital_positions.Add(new Coord(15, 1));
            capital_positions.Add(new Coord(15, 6));


            for (int i = 0; i < _players.Count; i++)
            {
                Create_Capital(_players[i], capital_positions[i]);
                Setup_Cloud_Cover(_players[i], capital_positions[i]);

            }
            
      
        }

        //generates unclaimed villages in random positions on the map
        public void Generate_Unclaimed_Villages()
        {
            Random r = new Random();

            for (int i = 0; i < 4; i++)
            {
                Coord village_pos = new Coord(r.Next(0, 20), r.Next(0, 16));

                if (Village_Exists(village_pos))
                {

                }

                else
                {
                    Create_Unclaimed_Village(village_pos);
                }
            }
        }

        //creates a capital village given a player and a coordinate
        public void Create_Capital(Player player, Coord capital_pos)
        {
            _map_entities.Add(new Village(capital_pos, player, "village"));
        }

        //creates an unclaimed village
        public void Create_Unclaimed_Village(Coord unclaimed_village_pos)
        {
            _map_entities.Add(new Village(unclaimed_village_pos, null, "village"));
        }
        
        //sets up the cloud cover for a player given their capital position / initial position
        public void Setup_Cloud_Cover(Player player, Coord capital_pos)
        {
            //defines the visibility range around the capital
            int visibilityRange = 1;

            //creates a 2D array to store cloud cover values
            int[,] cloud_cover = new int[_map.Height, _map.Width];

            //marks the capital position itself as visible
            cloud_cover[capital_pos.Y, capital_pos.X] = 1;

            //loops through each coordinate on the map
            for (int y = 0; y < _map.Height; y++)
            {
                for (int x = 0; x < _map.Width; x++)
                {
                    //checks if the current coordinate is within the visibility range of the capital
                    if (Math.Abs(x - capital_pos.X) <= visibilityRange && Math.Abs(y - capital_pos.Y) <= visibilityRange)
                    {
                        //marks the coordinate as visible
                        cloud_cover[y, x] = 1;
                    }
                    else
                    {
                        //marks the coordinate as not visible
                        cloud_cover[y, x] = 0;
                    }
                }
            }

            //sets the cloud cover for the player
            player.Set_Cloud_Cover(cloud_cover);
        }

        //reveals a given amount of cloud cover for a player
        public void Reveal_Cloud_Cover(Player player, Coord reveal_coord, int visibility)
        {
            //creates a copy of the cloud cover array
            int[,] cloud_cover = player.Cloud_Cover;

            //calculates the boundaries of the cloud cover
            int start_x = Math.Max(0, reveal_coord.X - visibility / 2);
            int end_x = Math.Min(cloud_cover.GetLength(1) - 1, reveal_coord.X + visibility / 2);
            int start_y = Math.Max(0, reveal_coord.Y - visibility / 2);
            int end_y = Math.Min(cloud_cover.GetLength(0) - 1, reveal_coord.Y + visibility / 2);

            //loops through each coordinate within the boundaries
            for (int y = start_y; y <= end_y; y++)
            {
                for (int x = start_x; x <= end_x; x++)
                {
                    //marks the coordinate as visible
                    cloud_cover[y, x] = 1;
                }
            }

            //updates the cloud cover for the player
            player.Set_Cloud_Cover(cloud_cover);
        }

        //assigns each player the initial tech tree
        public void Create_Tech_Trees()
        {
            foreach (Player player in _players)
            {
                TechTree tech_tree = new TechTree(player.Name + "'s Tech");

                TechNode riding = tech_tree.Add_Node("Riding", tech_tree.Root);
                TechNode chivalry = tech_tree.Add_Node("Chivalry", riding);
                TechNode smithery = tech_tree.Add_Node("Smithery", tech_tree.Root);
                TechNode strategy = tech_tree.Add_Node("Strategy", tech_tree.Root);
                TechNode diplomacy = tech_tree.Add_Node("Diplomacy", strategy);

                player.Tree = tech_tree;
            }
        }

        //upgrades a player's tech tree by unlocking a node
        public void Unlock_Tech_Tree_Node(string node_name)
        {
            //unlocks the tech node and retrieves unlocked node
            TechNode unlocked_node = _current_player.Tree.Unlock_Node(node_name);

            //subtracts cost of tech node from the players money
            _current_player.Money = _current_player.Money - unlocked_node.Cost;
        }

        //ends the current turn
        public void End_Turn()
        {
            //adds the players total output/income to their total money
            _current_player.Money += _current_player.Total_Output;

            //updates the capture counter variable for each unit on an enemy village
            Update_Capture_Counter();

            /*reassigns the owner of any villages where an enemy unit has been on the village for more than one turn by analysing the capture counter
            of each respective unit*/
            Capture_Villages();

            //checks if the game is over
            if (Game_Over())
            {
                Console.Clear();
                Console.WriteLine("The winner is {0}!", _winner.Name);
                Console.ReadKey();
            }


            //changes the current player to the next player

            //if the _current_player_pointer is going to exceed the bounds of the _players array loop back by setting the counter to 0
            if (_current_player_pointer + 1 == _players.Count)
            {
                _current_player_pointer = 0;
                _current_player = _players[_current_player_pointer];
            }

            else
            {
                _current_player_pointer = _current_player_pointer + 1;
                _current_player = _players[_current_player_pointer];
            }
        
        }

        //summons a unit
        public Unit Summon_Unit(Player player, Coord summon_pos, string type)
        {
            Unit new_unit = new Unit(new Coord(summon_pos.X, summon_pos.Y), player, type);
            _map_entities.Add(new_unit);

            //subtracts the cost of the unit from the player's money
            _current_player.Money = _current_player.Money - new_unit.Cost;

            return new_unit;
        }

        //checks if a unit exits at a specific coordinate
        public bool Unit_Exists(Coord coord)
        {
            //loops through all map entities, if the entity is not a village and matches the coordinate then returns true
            foreach (MapEntity entity in _map_entities)
            {
                if (entity.Type != "village" && entity.Position.X == coord.X && entity.Position.Y == coord.Y)
                {
                    return true;
                }
            }

            //otherwise no unit exists at the coord position
            return false;
        }

        //gets unit at specific Coord
        public Unit Get_Unit(Coord coord)
        {
            //loops through all map entities, if the entity is not a village and matches the coordinate then returns the unit
            foreach (MapEntity entity in _map_entities)
            {
                if (entity.Type != "village" && entity.Position.X == coord.X && entity.Position.Y == coord.Y)
                {
                    return (Unit)entity;
                }
            }

            //otherwise returns null as there is no unit
            return null;
        }

        //moves a unit on the map
        public void Move_Unit(Unit unit, Coord final_pos)
        {
            //reassigns the unit's position coordinate attribute 
            unit.Position = new Coord(final_pos.X, final_pos.Y);

            //reveals the appropriate amount of cloud cover depending on the visibility of the unit
            Reveal_Cloud_Cover(_current_player, final_pos, unit.Visibility);
        }

        //checks if a coordinate on the map is visible to the current player
        public bool Visible_Coord(Coord coord)
        {
            int width = _current_player.Cloud_Cover.GetLength(1);
            int height = _current_player.Cloud_Cover.GetLength(0);

            //ensures loop bounds are inclusive
            for (int x = 0; x <= width - 1; x++)
            {
                for (int y = 0; y <= height - 1; y++)
                {
                    if (x == coord.X && y == coord.Y)
                    {
                        //1 denotes a visible tile on the map
                        //0 denotes a non-visible tile on the map
                        if (_current_player.Cloud_Cover[y, x] == 1)
                        {
                            return true;
                        }

                        else
                        {
                            return false;
                        }
                    }
                }
            }

            return false;
        }

        //returns latest information about level to be used mainly by the GameUI class and for saving the game
        public LevelData Get_Level_Data()
        {
            return new LevelData(_players, _map, _map_entities, _current_player, _current_player_pointer, _game_mode);
        }

        //handles player actions
        public string Handle_Player_Action(PlayerAction action)
        {
            //string to send back to GameUI object so an appropriate response can be made
            string response = "";

            //handles AI player turns
            if (action.Type == "AI-turn")
            {
                AI_Turn();
                return response;
            }

            //only human player actions underneath

            //handles saving the game in it's current state
            else if (action.Type == "save")
            {
                LevelDataSaver saver = new LevelDataSaver();
                saver.Save(Get_Level_Data());
            }

            //handles summoning a warrior unit type
            else if (action.Type == "summon-warrior")
            {
                //checks if the player has selected a valid village (belongs to them)
                if (Valid_Player_Village(action.Map_Cursor) == true)
                {
                    //checks if the player has sufficient funds to afford the unit
                    if (_current_player.Money - Unit_Cost.Warrior >= 0)
                    {
                        //checks if there is already a unit there on that village

                        //if not then the unit is summoned and the status panel is updated
                        if (Unit_Exists(action.Map_Cursor) == false)
                        {
                            Summon_Unit(_current_player, action.Map_Cursor, "warrior");

                            Update_Status("Warrior summoned!");
                        }

                        else
                        {
                            Update_Status("There is a unit here!");
                        }
                    }

                    //if not then status panel is updated to notify the player
                    else
                    {
                        Update_Status("Not enough money!");
                    }
                }

                //if not then status panel is updated to notify the player
                else
                {
                    Update_Status("Not a valid village!");
                }


            }

            //handles summoning a rider unit type
            else if (action.Type == "summon-rider")
            {
                //first checks if relevant technology is unlocked, if not updates status panel
                if (!Tech_Unlocked("Riding"))
                {
                    Update_Status("Unlock riding first");
                    return response;
                }

                //checks if the player has selected a valid village (belongs to them)
                if (Valid_Player_Village(action.Map_Cursor) == true)
                {

                    //checks if the player has sufficient funds to afford the unit
                    if (_current_player.Money - Unit_Cost.Rider >= 0)
                    {
                        //checks if there is already a unit there on that village

                        //if not then the unit is summoned and the status panel is updated
                        if (Unit_Exists(action.Map_Cursor) == false)
                        {
                            Summon_Unit(_current_player, action.Map_Cursor, "rider");

                            Update_Status("Rider summoned!");
                        }

                        else
                        {
                            Update_Status("There is a unit here!");
                        }
                    }

                    //if not then status panel is updated to notify the player
                    else
                    {
                        Update_Status("Not enough money!");
                    }
                }

                //if not then status panel is updated to notify the player
                else
                {
                    Update_Status("Not a valid village!");
                }


            }

            //handles summoning a swordsman unit type
            else if (action.Type == "summon-swordsman")
            {

                //first checks if relevant technology is unlocked, if not updates status panel
                if (!Tech_Unlocked("Smithery"))
                {
                    Update_Status("Unlock smithery first");
                    return response;
                }

                //checks if the player has selected a valid village (belongs to them)
                if (Valid_Player_Village(action.Map_Cursor) == true)
                {
                    //checks if the player has sufficient funds to afford the unit
                    if (_current_player.Money - Unit_Cost.Swordsman >= 0)
                    {
                        //checks if there is already a unit there on that village

                        //if not then the unit is summoned and the status panel is updated
                        if (Unit_Exists(action.Map_Cursor) == false)
                        {
                            Summon_Unit(_current_player, action.Map_Cursor, "swordsman");

                            Update_Status("Swordsman summoned!");
                        }

                        else
                        {
                            Update_Status("There is a unit here!");
                        }
                    }

                    //if not then status panel is updated to notify the player
                    else
                    {
                        Update_Status("Not enough money!");
                    }
                }

                //if not then status panel is updated to notify the player
                else
                {
                    Update_Status("Not a valid village!");
                }


            }

            //handles summoning a defender unit type
            else if (action.Type == "summon-defender")
            {

                //first checks if relevant technology is unlocked, if not updates status panel
                if (!Tech_Unlocked("Strategy"))
                {
                    Update_Status("Unlock strategy first");
                    return response;
                }

                //checks if the player has selected a valid village (belongs to them)
                if (Valid_Player_Village(action.Map_Cursor) == true)
                {

                    //checks if the player has sufficient funds to afford the unit
                    if (_current_player.Money - Unit_Cost.Defender >= 0)
                    {
                        //checks if there is already a unit there on that village

                        //if not then the unit is summoned and the status panel is updated
                        if (Unit_Exists(action.Map_Cursor) == false)
                        {
                            Summon_Unit(_current_player, action.Map_Cursor, "defender");

                            Update_Status("Defender summoned!");
                        }

                        else
                        {

                            Update_Status("There is a unit here!");
                        }
                    }

                    //if not then status panel is updated to notify the player
                    else
                    {
                        Update_Status("Not enough money!");
                    }
                }

                //if not then status panel is updated to notify the player
                else
                {
                    Update_Status("Not a valid village!");
                }


            }

            //handles summoning a knight unit type
            else if (action.Type == "summon-knight")
            {
                //first checks if relevant technology is unlocked, if not updates status panel
                if (!Tech_Unlocked("Chivalry"))
                {
                    Update_Status("Unlock chivalry first");
                    return response;
                }

                //checks if the player has selected a valid village (belongs to them)
                if (Valid_Player_Village(action.Map_Cursor) == true)
                {
                    //checks if the player has sufficient funds to afford the unit
                    if (_current_player.Money - Unit_Cost.Knight >= 0)
                    {
                        //checks if there is already a unit there on that village

                        //if not then the unit is summoned and the status panel is updated
                        if (Unit_Exists(action.Map_Cursor) == false)
                        {
                            Summon_Unit(_current_player, action.Map_Cursor, "knight");

                            Update_Status("Knight summoned!");
                        }

                        else
                        {
                            Update_Status("There is a unit here!");
                        }
                    }

                    //if not then status panel is updated to notify the player
                    else
                    {
                        Update_Status("Not enough money!");
                    }
                }

                //if not then status panel is updated to notify the player
                else
                {
                    Update_Status("Not a valid village!");
                }


            }

            //handles selecting a unit to move
            else if (action.Type == "move-unit")
            {
                if (Valid_Unit_Selection(action.Map_Cursor) == true)
                {
                    response = "highlight-unit";

                    _initial_unit_pos = new Coord(action.Map_Cursor.X, action.Map_Cursor.Y);
                }
            }

            //handles selecting the final position of a unit that is going to be moved
            else if (action.Type == "final-unit-move")
            {
                if (Valid_Unit_Movement(_initial_unit_pos, action.Map_Cursor) == true)
                {
                    Move_Unit(Get_Unit(_initial_unit_pos), action.Map_Cursor);
                }

            }

            //handles researching a technology
            else if (action.Type.Contains("research"))
            {
                //researches tech
                Research_Tech(action);
            }

            //used for handling UI responses
            return response;
        }

        //researches a technology
        public void Research_Tech(PlayerAction research_action)
        {
            //checks if player has sufficient funds
            if (_current_player.Money - 25 >= 0)
            {
                //by checking the action type the correct technology is unlocked on the tech tree of the current player
                if (research_action.Type == "research-riding")
                {
                    //unlocks node on player's tech tree
                    Unlock_Tech_Tree_Node("Riding");
                }

                else if (research_action.Type == "research-smithery")
                {
                    //unlocks node on player's tech tree
                    Unlock_Tech_Tree_Node("Smithery");
                    
                }
                else if (research_action.Type == "research-strategy")
                {
                    //unlocks node on player's tech tree
                    Unlock_Tech_Tree_Node("Strategy");
                }

                else if (research_action.Type == "research-chivalry")
                {
                    //unlocks node on player's tech tree
                    Unlock_Tech_Tree_Node("Chivalry");

                }
                else if (research_action.Type == "research-diplomacy")
                {
                    //unlocks node on player's tech tree
                    Unlock_Tech_Tree_Node("Diplomacy");

                }
            }
        }

        //checks if a technology is unlocked on the current players tech tree
        public bool Tech_Unlocked(string tech_name)
        {
            return _current_player.Tree.Unlocked(tech_name);
        }
        
        //checks whether a player can select a specific unit to move
        public bool Valid_Unit_Selection(Coord player_unit_pos)
        {
            foreach (MapEntity entity in _map_entities)
            {
                if ((entity.Type != "village") && entity.Position.X == player_unit_pos.X && entity.Position.Y == player_unit_pos.Y && entity.Owner.Name == _current_player.Name)
                {
                    return true;
                }
            }

            return false;
        }
  
        //checks whether a unit movement is valid
        public bool Valid_Unit_Movement(Coord intital_pos, Coord final_pos)
        {
            Unit unit = Get_Unit(intital_pos);
            Unit dest_unit = Get_Unit(final_pos);

            //checks if the final position of the unit is cloud covered / out of bounds
            if (Visible_Coord(final_pos) == false)
            {
                Update_Status("Can't travel into the unknown");
                return false;
            }

            //checks if the final position is within movement range of the player's unit if there is no unit at the final position
            if (dest_unit == null)
            {
                if ((intital_pos.X - final_pos.X <= unit.Movement) && (intital_pos.Y - final_pos.Y <= unit.Movement))
                {
                    if ((final_pos.X - intital_pos.X <= unit.Movement) && (final_pos.Y - intital_pos.Y <= unit.Movement))
                    {
                        return true;
                    }

                }

                return false;
            }

            //checks whether there is a friendly unit at the final position, if so don't allow the unit to move
            else if (dest_unit.Owner == _current_player)
            {
                return false;
            }

            //checks whether there is an enemy unit at the final position, if it can be killed then it is removed 
            else if (dest_unit.Owner != _current_player)
            {
                if ((intital_pos.X - final_pos.X <= unit.Movement) && (intital_pos.Y - final_pos.Y <= unit.Movement) && (Fight(unit, dest_unit) == true))
                {
                    Remove_Unit(dest_unit);

                    //checks whether the final position is within movement range of the player's unit
                    if ((final_pos.X - intital_pos.X <= unit.Movement) && (final_pos.Y - intital_pos.Y <= unit.Movement))
                    {
                        return true;
                    }

                }

                return false;
            }
          
            return false;
        }

        //checks whether a player owns a village at a specific coordinate
        public bool Valid_Player_Village(Coord coord)
        {
            foreach (MapEntity entity in _map_entities)
            {
                //for no owner
                if (entity.Owner == null)
                {
                    continue;
                }

                //for the player being the owner
                if (entity.Type == "village" && entity.Position.X == coord.X && entity.Position.Y == coord.Y && entity.Owner.Name == _current_player.Name)
                {
                    return true;
              
                }
            }

            return false;
        
        }

        //allows a unit to inflict damage on an enemy and vice versa
        //returns true if the defender was killed and false if the defender is still alive
        public bool Fight(Unit attacker, Unit defender)
        {
            //subtract attack damage from defender HP
            int damage = attacker.Attack;
            defender.HP = defender.HP - damage;

            //if defender still alive respond by attacking the attacker
            if (defender.HP > 0)
            {
                int response = defender.Defence;
                attacker.HP = attacker.HP - response;


                return false;
            }

            return true;
        }

        //removes a unit from the game
        public void Remove_Unit(Unit unit)
        {
            foreach (MapEntity entity in _map_entities)
            {
                if (entity == unit)
                {
                    _map_entities.Remove(entity);

                    break;
                }
            }
        }

        //updates each unit's capture counter
        public void Update_Capture_Counter()
        {
            //obtain all villages
            List<MapEntity> villages = new List<MapEntity>();

            foreach (MapEntity entity in _map_entities)
            {
                if (entity.Type == "village")
                {
                    villages.Add(entity);
                }
            }

            //foreach village if a unit exists on it and it is does not belong to the owner of the village then increment capture counter
            foreach (MapEntity village in villages)
            {
                if (Unit_Exists(village.Position))
                {
                    Unit village_unit = Get_Unit(village.Position);
                    if (village_unit.Owner != village.Owner)
                    {
                        village_unit.Capture_Counter += 1;
                    }
                }
            }
           
            //if a unit is not on a village reset it's capture counter to zero
            foreach (MapEntity entity in _map_entities)
            {
                if (entity.Type != "village" && Village_Exists(entity.Position) == false)
                {
                    Unit unit = Get_Unit(entity.Position);
                    unit.Capture_Counter = 0;
                }
            }
        }

        /*reassigns the owner of any villages where a unit that is not owned by the village owner (an enemy unit) has been on it for
        one complete turn cycle, one turn cycle is equivalent to the turn where the unit was moved onto an enemy village until that
        player's next turn (the one that moved the unit) after the other players have completed their turns */
        public void Capture_Villages()
        {

            //obtain all the villages
            List<MapEntity> villages = new List<MapEntity>();

            foreach (MapEntity entity in _map_entities)
            {
                if (entity.Type == "village")
                {
                    villages.Add(entity);
                }
            }

            //foreach village 
            foreach (MapEntity village in villages)
            {
                //checks if a unit exists on the village
                if (Unit_Exists(village.Position))
                {
                    //obtains the unit on the village
                    Unit village_unit = Get_Unit(village.Position);
                    Village village_object = (Village)village;

                    //checks if the unit is an enemy unit, and if the unit has reached the capture counter limit
                    if (village_unit.Owner != village.Owner && village_unit.Capture_Counter == _players.Count)
                    {
                        //checks if the owner of the village is not unclaimed
                        if (village_object.Owner != null)
                        {
                            //remove the village's output from the previous owner 
                            village.Owner.Total_Output = village.Owner.Total_Output - village_object.Output;

                            //subtract the output of the current turn from the previous player's money
                            village.Owner.Money = village.Owner.Money - village_object.Output;
                        }
                        
                        //reassign the village owner
                        village.Owner = village_unit.Owner;

                        //reset the capture counter
                        village_unit.Capture_Counter = 0;

                        //add the village's output to the new owner so they can recieve income from it every turn
                        village.Owner.Total_Output = village.Owner.Total_Output + village_object.Output;
                    }
                }
            }
        }

        //checks if a village exists at a given position
        public bool Village_Exists(Coord coord)
        {
            foreach (MapEntity entity in _map_entities)
            {
                if (entity.Type == "village" && entity.Position.X == coord.X && entity.Position.Y == coord.Y)
                {
                    return true;
                }
            }
            return false;
        }

        //checks if the game is over
        public bool Game_Over()
        {

            List<Player> remaining_players = new List<Player>();

            //in this game mode all players must be eradicated from the map to win
            if (_game_mode == "domination")
            {
                //loops through all map entities to get a list of the remaining players that own villages
                foreach (MapEntity entity in _map_entities)
                {
                    if (entity.Type == "village")
                    {
                        //for no owner don't add null to remaining players
                        if (entity.Owner == null)
                        {
                            continue;
                        }

                        //if the player has already been added to the remaining players list don't add them again
                        else if (remaining_players.Contains(entity.Owner))
                        {
                            continue;
                        }

                        //if the player has not been added, add them to the list
                        else
                        {
                            remaining_players.Add(entity.Owner);
                        }
                    }
                    
                        
                }

                //if there is only one player left they are the winner
                if (remaining_players.Count == 1)
                {
                    _winner = remaining_players[0];
                    return true;
                }

                //otherwise there is no winner
                else
                {
                    return false;
                }

            }

            return false;

        }

        //invokes the OnStatusUpdateRequest event to change the status panel message in the GameUI object
        public void Update_Status(string status_message)
        {
            
            OnStatusUpdateRequest.Invoke(status_message);
        }

        //allows a computer/AI player to play by summoning new units and moving them
        public void AI_Turn()
        {
            AI_Move();
            AI_Summon();
        }

        //allows an AI player to move their units on the map
        public void AI_Move()
        {
            //obtains all the units of the current AI player
            List<MapEntity> AI_units = new List<MapEntity>();

            foreach (MapEntity entity in _map_entities)
            {
                if (entity.Owner != null)
                {
                    if (entity.Owner.Name == _current_player.Name && entity.Type != "village")
                    {
                        AI_units.Add(entity);
                    }
                }

            }

            //loops through all of the units belonging to the AI and moves them
            foreach (MapEntity unit in AI_units)
            {
                Unit target_unit = (Unit)unit;
                AI_Unit_Move(target_unit);
            
            }
        }

        //moves an AI unit
        public void AI_Unit_Move(Unit unit)
        {
            //reset map tile access to allow the BFS algorithm to work correctly based on the current state of the map
            _map.Reset_Tile_Access();

            /*capturer units move towards enemy villages and unclaimed villages that are the closest to them
            they ignore enemy units and pathfind around them*/
            if (unit.Marker == "capturer")
            {
                //used to hold all villages that belong to enemies or are unclaimed
                List<MapEntity> target_villages = new List<MapEntity>();

                //obtains all villages that belong to enemies or are unclaimed
                foreach (MapEntity entity in _map_entities)
                {
                    if (entity.Owner != null)
                    {
                        if (entity.Owner.Name != _current_player.Name && entity.Type == "village")
                        {
                            //add to target villages (enemy village)
                            target_villages.Add(entity);
                        }
                    }

                    else
                    {
                        //add to target villages (unclaimed village)
                        target_villages.Add(entity);
                    }

                }

                //sets up the map so tile access is removed from coordinates where there are enemy units so the BFS can compute a path around them
                foreach (MapEntity entity in _map_entities)
                {
                    if (entity.Owner != null)
                    {
                        if (entity.Owner.Name != _current_player.Name && entity.Type != "village")
                        {
                            _map.Remove_Tile_Access(entity.Position);
                        }
                    }
                }

                //holds all possible routes a unit can take
                List<List<Tile>> possible_paths = new List<List<Tile>>();

                //for every target village use the BFS algorithm to compute the shortest path from the unit to it and then add it to the possible_paths array
                foreach (MapEntity target_village in target_villages)
                {
                    List<Tile> possible_path = _map.BFS(unit.Position, target_village.Position);

                    //ignore any null paths produced by the algorithm
                    if (possible_path == null)
                    {
                        continue;
                    }

                    //add any non-null paths to the possible_paths array
                    else
                    {
                        possible_paths.Add(_map.BFS(unit.Position, target_village.Position));
                    }
                }

                //used to hold the shortest path, default is null
                List<Tile> shortest_path = null;

                if (possible_paths.Count > 0)
                {
                    shortest_path = possible_paths[0];
                }

                //if there is no path do not move unit
                if (shortest_path == null)
                {

                }

                //move the unit according to the shortest path to the target village
                else
                {
                    //loops through each path and compares them to find the shortest
                    foreach (List<Tile> path in possible_paths)
                    {
                        //if there is a path shorter than the shortest_path replace the shortest_path
                        if (path.Count < shortest_path.Count)
                        {
                            shortest_path = path;
                        }
                    }

                    //obtain the unit's final position which is the first Tile in the shortest path
                    Coord final_pos = new Coord(shortest_path[1].X, shortest_path[1].Y);

                    //if it is a valid unit movement, move the unit
                    if (Valid_Unit_Movement(unit.Position, final_pos) == true)
                    {
                        Move_Unit(unit, final_pos);
                    }
                }
            }

            /*offensive-capturer units move towards enemy villages and unclaimed villages that are the closest to them,
            they do not deviate from their path, this results in them attacking enemy units in the way*/
            if (unit.Marker == "offensive-capturer")
            {
                //used to hold all villages that belong to enemies or are unclaimed
                List<MapEntity> target_villages = new List<MapEntity>();

                //obtains all villages that belong to enemies or are unclaimed
                foreach (MapEntity entity in _map_entities)
                {
                    if (entity.Owner != null)
                    {
                        if (entity.Owner.Name != _current_player.Name && entity.Type == "village")
                        {
                            //add to target villages (enemy village)
                            target_villages.Add(entity);
                        }
                    }

                    else
                    {
                        //add to target villages (unclaimed village)
                        target_villages.Add(entity);

                    }

                }

                //holds all possible routes a unit can take
                List<List<Tile>> possible_paths = new List<List<Tile>>();

                //for every target village use the BFS algorithm to compute the shortest path from the unit to it and then add it to the possible_paths array
                foreach (MapEntity target_village in target_villages)
                {
                    List<Tile> possible_path = _map.BFS(unit.Position, target_village.Position);

                    //ignore any null paths produced by the algorithm
                    if (possible_path == null)
                    {
                        continue;
                    }

                    //add any non-null paths to the possible_paths array
                    else
                    {
                        possible_paths.Add(_map.BFS(unit.Position, target_village.Position));
                    }
                }

                //used to hold the shortest path, default is null
                List<Tile> shortest_path = null;

                if (possible_paths.Count > 0)
                {
                    shortest_path = possible_paths[0];
                }

                //if there is no path do not move unit
                if (shortest_path == null)
                {

                }

                //move the unit according to the shortest path to the target village
                else
                {
                    //loops through each path and compares them to find the shortest
                    foreach (List<Tile> path in possible_paths)
                    {
                        //if there is a path shorter than the shortest_path replace the shortest_path
                        if (path.Count < shortest_path.Count)
                        {
                            shortest_path = path;
                        }
                    }

                    //obtain the unit's final position which is the first Tile in the shortest path
                    Coord final_pos = new Coord(shortest_path[1].X, shortest_path[1].Y);

                    //if it is a valid unit movement, move the unit
                    if (Valid_Unit_Movement(unit.Position, final_pos) == true)
                    {
                        Move_Unit(unit, final_pos);
                    }
                }

            }
        }

        //allows an AI player to summon units on all of their villages
        public void AI_Summon()
        {
            //obtains all villages that belong to the AI
            List<MapEntity> AI_villages = new List<MapEntity>();

            foreach (MapEntity entity in _map_entities)
            {
                if (entity.Owner != null)
                {
                    if (entity.Owner.Name == _current_player.Name && entity.Type == "village")
                    {
                        AI_villages.Add(entity);
                    }
                }

            }

            //by default the AI player will summon a warrior
            string selected_unit_type = "warrior";

            //randomly select a number between 1 and 2
            Random rand = new Random();
            int unit_type_selection_pointer = rand.Next(1, 3);

            //easy difficulty level AI players can only summon warriors
            if (_current_player.Difficulty == "easy")
            {
                selected_unit_type = "warrior";
            }

            //medium difficulty level AI players can randomly summon either riders or warriors
            else if (_current_player.Difficulty == "medium")
            {
                if (unit_type_selection_pointer == 1)
                {
                    selected_unit_type = "rider";
                }

                else
                {
                    selected_unit_type = "warrior";
                }
            }

            //hard difficulty level AI players can randomly summon either riders or swordsmen
            else if (_current_player.Difficulty == "hard")
            {
                if (unit_type_selection_pointer == 1)
                {
                    selected_unit_type = "rider";
                }

                else
                {
                    selected_unit_type = "swordsman";
                }
            }

            //extreme difficulty level AI players can randomly summon either defenders or knights
            else if (_current_player.Difficulty == "extreme")
            {
                if (unit_type_selection_pointer == 1)
                {
                    selected_unit_type = "defender";
                }

                else
                {
                    selected_unit_type = "knight";
                }
            }

            //loops through all of the AI player's villages and summons the selected unit type and assigns a marker to the unit that dictates it's behaviour
            for (int i = 0; i < AI_villages.Count; i++)
            {
                MapEntity village = AI_villages[i];

                //only summons if a unit does not already exist on the village
                if (!Unit_Exists(village.Position))
                {
                    //summons new unit
                    Unit new_unit = Summon_Unit(_current_player, village.Position, selected_unit_type);

                    //randomly marks the unit either as a capturer or offensive-capturer
                    Random rand2 = new Random();
                    int marker_selection = rand2.Next(1, 3);

                    if (marker_selection == 1)
                    {
                        new_unit.Marker = "offensive-capturer";
                    }

                    else
                    {
                        new_unit.Marker = "capturer";
                    }

                    
                }
            }
        }

      
    }
}