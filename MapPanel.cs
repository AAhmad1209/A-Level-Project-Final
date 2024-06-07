using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //This is a map panel which displays the map to the player making up part of the turn UI
    class MapPanel : TurnUIPanel
    {
        private Map _map;
        private List<MapEntity> _map_entities;
        private Coord _cursor;
        private int _internal_x, _internal_y;

        private Player _current_player;

        private Coord _mark_coord;

        //constructor
        public MapPanel(Rectangle rectangle, string name, LevelData level_data, int depth) : base(rectangle, name, level_data, depth)
        {
            _map = level_data.Map;
            _current_player = level_data.Current_Player;

            _map_entities = level_data.Map_Entities;
            _internal_x = _rectangle.Position.X + 1;
            _internal_y = _rectangle.Position.Y + 1;

            _cursor = new Coord(0, 0);

            _mark_coord = null;

        }

        //updates the map cursor position based on the key the user has pressed
        public override PlayerAction Update(ConsoleKeyInfo input)
        {
            //the down key changes the cursor position to point to the tile underneath the currently selected tile
            if (input.Key == ConsoleKey.DownArrow)
            {
                //does not allow the map boundaries to be crossed
                if (_map.Get_Tile(0, _cursor.Y + 1) == null)
                {
                    return null;
                }

                else
                {
                    _cursor.Y++;
                }
            }

            //the up key changes the cursor position to point to the tile above the currently selected tile
            else if (input.Key == ConsoleKey.UpArrow)
            {
                //does not allow the map boundaries to be crossed
                if (_map.Get_Tile(0, _cursor.Y - 1) == null)
                {
                    return null;
                }

                else
                {
                    _cursor.Y--;
                }
            }

            //the left key changes the cursor position to point to the tile to the left of the currently selected tile
            else if (input.Key == ConsoleKey.LeftArrow)
            {
                //does not allow the map boundaries to be crossed
                if (_map.Get_Tile(_cursor.X - 1, 0) == null)
                {
                    return null;
                }

                else
                {
                    _cursor.X--;
                }
            }

            //the right key changes the cursor position to point to the tile to the right of the currently selected tile
            else if (input.Key == ConsoleKey.RightArrow)
            {
                //does not allow the map boundaries to be crossed
                if (_map.Get_Tile(_cursor.X + 1, 0) == null)
                {
                    return null;
                }

                else
                {
                    _cursor.X++;
                }
            }

            return null;
        }

        //displays the map inside the MapPanel and highlights the location of the map cursor
        public override void Display_Contents()
        {

            Console.ResetColor();

            Console.SetCursorPosition(_internal_x, _internal_y);

            //loops through the entire map and displays each tile according to a variety of factors
            //including: terrain type, cloud cover, units, villages and entity relation to current player
            for (int y = 0; y < _map.Height; y++)
            {
                for (int x = 0; x < _map.Width; x++)
                {
                    //gets current tile
                    Tile tile = _map.Get_Tile(x, y);

                    bool cloudy = false;

                    //checks if the current tile is cloudy
                    if (_current_player.Cloud_Cover[y, x] == 0)
                    {
                        cloudy = true;
                    }

                    //the symbol is the character that will be displayed to represent the tile
                    string symbol = " ";

                    bool contains_village = false;

                    //friendly, unclaimed or enemy
                    string village_type = "";

                    //checks if there is a village on the tile, if so it defines the village's relation to the player
                    foreach (MapEntity entity in _map_entities)
                    { 
                        if (entity.Position.X == x && entity.Position.Y == y && entity.Type == "village")
                        {
                            contains_village = true;
                           

                            if (entity.Owner == null)
                            {
                                village_type = "unclaimed";
                            }

                            else if (entity.Owner.Name == _current_player.Name)
                            {
                                village_type = "friendly";
                            }

                            else
                            {
                                village_type = "enemy";
                            }
                        }
                    
                    }

                    bool contains_unit = false;

                    //friendly or enemy
                    string unit_relation = "";

                    //warrior, rider, knight... to name a few
                    string unit_type = "";

                    //checks if there is a unit on the tile, if so it defines the unit's relation to the player
                    foreach (MapEntity entity in _map_entities)
                    {
                        if (entity.Position.X == x && entity.Position.Y == y && entity.Type != "village")
                        {
                            contains_unit = true;
                            if (entity.Owner.Name == _current_player.Name)
                            {
                                unit_relation = "friendly";
                            }

                            else
                            {
                                unit_relation = "enemy";
                            }

                            unit_type = entity.Type;
                        }
                    }

                    //checks if the tile contains a village
                    if (contains_village == true)
                    { 
                        //enemy villages are displayed in a red foreground color
                        if (village_type == "enemy")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }

                        //unclaimed villages are displayed in a black foreground color
                        else if (village_type == "unclaimed")
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                        }

                        //friendly villages are displayed in a blue foreground color
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                        }

                        //villages are represented by the letter V on the map
                        symbol = "V";
                    }

                    //checks if the tile contains a unit
                    if (contains_unit == true)
                    {
                        //enemy units are displayed in a red foreground color
                        if (unit_relation == "enemy")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }

                        //friendly units are displayed in a blue foreground color
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                        }

                        //the different unit types have unique symbols to represent them

                        if (unit_type == "warrior")
                        {
                            symbol = "W";
                        }

                        else if (unit_type == "rider")
                        {
                            symbol = "R";
                        }

                        else if (unit_type == "swordsman")
                        {
                            symbol = "S";
                        }

                        else if (unit_type == "defender")
                        {
                            symbol = "D";
                        }

                        else if (unit_type == "knight")
                        {
                            symbol = "K";
                        }
                    }


                    //the tile's background colour is determined primarly by the tile terrain

                    if (tile.Type == "grass")
                    {
                        Console.BackgroundColor = ConsoleColor.Green;

                    }

                    else if (tile.Type == "forest")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;

                    }

                    else if (tile.Type == "mountain")
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;

                    }

                    else if (tile.Type == "sand")
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;

                    }

                    //checks if the tile is marked
                    bool marked_tile = false;
                    if (_mark_coord != null)
                    {
                        if (x == _mark_coord.X && y == _mark_coord.Y)
                        {
                            marked_tile = true;
                        }
                    }

                    //marked tiles have a purple background color
                    if (marked_tile == true)
                    {
                        Console.BackgroundColor = ConsoleColor.Magenta;
                    }

                    //if the tile is cloudy the background is white and the symbol becomes a ~ character to hide what is on the tile
                    if (cloudy)
                    {
                        Random r = new Random();

                        int choice = r.Next(1, 3);

                        if (choice == 0)
                        {
                            symbol = "~";
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        }

                        else if (choice == 1 || choice == 2 || choice == 3)
                        {
                            symbol = "~";
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                            Console.ForegroundColor = ConsoleColor.DarkGray;

                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        
                    }

                    //checks if the tile is the map cursor position, if so the background of the tile is magenta
                    if (x == _cursor.X && y == _cursor.Y)
                    {

                        Console.BackgroundColor = ConsoleColor.Magenta;

                        Console.Write(symbol);
                    }

                    //if it is not the map cursor position write the symbol on the screen
                    else
                    {
                        Console.Write(symbol);
                    }

                    Console.ResetColor();
                }

                //sets cursor position 
                (int x, int y) current_pos = Console.GetCursorPosition();
                Console.SetCursorPosition(_internal_x, current_pos.y + 1);
  
            }
 
            Console.ResetColor();
        }

        //updates the _map_entities array
        public void Update_Map_Entities(List<MapEntity> entities)
        {
           _map_entities = entities;
            Display_Contents();
        }

        //updates the current player
        public void Update_Current_Player(Player current_player)
        {
            _current_player = current_player;

            //Display_Contents();
        }

        //resets the cursor position to default
        public override void Reset()
        {
            _cursor = new Coord(0, 0);
            Display_Contents();

        }

        //returns the cursor position as a Coord
        public Coord Get_Cursor()
        {
            return _cursor;
        }

        //marks the currently selected tile
        public void Mark_Cursor_Tile()
        {
            _mark_coord = new Coord(_cursor.X, _cursor.Y);
        }

        //unmarks the marked tile
        public void Unmark_Cursor_Tile()
        {
            _cursor = new Coord(_mark_coord.X, _mark_coord.Y);
            _mark_coord = null;
        }

    }
}
