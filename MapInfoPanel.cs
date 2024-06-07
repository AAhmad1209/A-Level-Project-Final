using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //This is an information panel which displays map information to the player making up part of the turn UI
    class MapInfoPanel : TurnUIPanel
    {
        private string _name;
        private int _internal_x;
        private int _internal_y;
        private LevelData _level_data;

        private int _lines;

        private Coord _map_cursor_pos;
        private string _pos_terrain_type;

        //constructor
        public MapInfoPanel(Rectangle rectangle, string name, LevelData level_data, int depth) : base(rectangle, name, level_data, depth)
        {
            _level_data = level_data;
            _internal_x = _rectangle.Position.X + 1;
            _internal_y = _rectangle.Position.Y + 1;
            _lines = _rectangle.Height - 2;
            _name = name;
            _map_cursor_pos = new Coord(0, 0);
            _pos_terrain_type = "";
        }

        //allows the MapInfoPanel to reset
        public override void Reset()
        {
            _map_cursor_pos = new Coord(0, 0);
            _pos_terrain_type = "";
        }

        //displays information to the player inside the MapInfoPanel
        public override void Display_Contents()
        {
            //gets current tile and related information
            Tile current = _level_data.Map.Get_Tile(_map_cursor_pos.X, _map_cursor_pos.Y);

            //gets cloud cover bounds
            int width = _level_data.Current_Player.Cloud_Cover.GetLength(1);
            int height = _level_data.Current_Player.Cloud_Cover.GetLength(0);

            bool visible = true;

            //uses this to determine whether a tile is visible to the player
            //ensures loop bounds are inclusive
            for (int x = 0; x <= width - 1; x++)
            {
                for (int y = 0; y <= height - 1; y++)
                {
                    if (x == _map_cursor_pos.X && y == _map_cursor_pos.Y)
                    {
                        //here 1 represents visible and 0 represents not visible
                        if (_level_data.Current_Player.Cloud_Cover[y, x] == 1)
                        {
                            
                        }

                        else
                        {
                            visible = false;
                        }
                    }
                }
            }
            
            Clear();
            Console.ResetColor();
            Console.SetCursorPosition(_internal_x, _internal_y);

            //displays the coordinate of the selected tile and the terrain type

            Console.WriteLine("Selected Tile:");

            Console.SetCursorPosition(_internal_x, _internal_y+1);

            Console.WriteLine(@" ({0}, {1})", _map_cursor_pos.X, _map_cursor_pos.Y);

            Console.SetCursorPosition(_internal_x, _internal_y + 3);

            Console.WriteLine("Terrain Type:");

            Console.SetCursorPosition(_internal_x, _internal_y + 4);

            //if the coordinate is not visible displays Unknown, otherwise displays it's terrain type
            if (visible == true)
            {
                Console.WriteLine(" " + Capitalise(current.Type));
            }

            else
            {
                Console.WriteLine(" Unknown");
            }

            //obtains map entities at the cursor position
            List<MapEntity> coord_entities = Get_Coord_Entities(_map_cursor_pos);

            Console.SetCursorPosition(_internal_x, _internal_y + 5);

            Console.WriteLine(" ");

            Console.SetCursorPosition(_internal_x, _internal_y + 6);

            Console.WriteLine("Entities:");

            int line_displacement = 7;

            //if the tile is visible displays the specific information of map entities on it
            if (visible == true)
            {

                foreach (MapEntity entity in coord_entities)
                {
                    //displays if there is a village on the tile
                    if (entity.Type == "village")
                    {
                        if (entity.Owner == null)
                        {
                            Console.SetCursorPosition(_internal_x, _internal_y + line_displacement);
                            Console.WriteLine(@" {0} | Unclaimed", Capitalise(entity.Type));
                        }

                        else
                        {
                            Console.SetCursorPosition(_internal_x, _internal_y + line_displacement);
                            Console.WriteLine(@" {0} | Owner: {1}", Capitalise(entity.Type), entity.Owner.Name);
                        }

                    }

                    //displays if there is a unit on the tile
                    else
                    {
                        Unit temp_unit = (Unit)entity;
                        Console.SetCursorPosition(_internal_x, _internal_y + line_displacement);

                        //displays the unit's health, type and owner
                        Console.WriteLine(@" {0} | Owner: {1} | HP: {2}", Capitalise(entity.Type), entity.Owner.Name, temp_unit.HP);
                    }

                    line_displacement++;
                }

                //if there are no entities on the tile displays nothing here to the player
                if (coord_entities.Count == 0)
                {
                    Console.SetCursorPosition(_internal_x, _internal_y + line_displacement);
                    Console.WriteLine(" Nothing here!");
                }
            }

            //if the tile is not visible displays unknown to the player
            else
            {
                
                Console.SetCursorPosition(_internal_x, _internal_y + line_displacement);
                Console.WriteLine(" Unknown");
            }
        }

        //makes the first letter uppercase in a string and the rest lowercase
        public string Capitalise(string word)
        {
            return word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();
        }

        //sets the _map_cursor_pos attribute to be displayed as information to the player
        public void Set_Map_Cursor_Pos(Coord map_cursor_pos)
        {
            _map_cursor_pos = map_cursor_pos;
        
        }

        //clears the information panel
        public void Clear()
        {
            for (int i = 0; i < _lines + 1; i++)
            {
                Console.SetCursorPosition(_internal_x, _internal_y + i);
                Console.WriteLine("                                         ");
            }
        }

        //updates the level data
        public void Update_Level_Data(LevelData data)
        {
            _level_data = data;
        }

        //obtains map entities at specific coordinate
        public List<MapEntity> Get_Coord_Entities(Coord target_coord)
        {
            List <MapEntity> coord_entities = new List<MapEntity>();

            foreach (MapEntity entity in _level_data.Map_Entities)
            {
                //adds to the list of coord entities if the target_coord matches the entity position
                if (entity.Position.X == target_coord.X && entity.Position.Y == target_coord.Y)
                {
                    coord_entities.Add(entity);
                }
            }

            return coord_entities;
        }
    }
}
