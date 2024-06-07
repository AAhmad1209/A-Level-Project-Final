using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Represents a game map
    [Serializable]
    internal class Map
    {
        private string _name;
        private string _file_name;
        private Tile[,] _terrain;
        private int _width, _height;

        //constructor
        public Map(string name, string file_name)
        {
            _name = name;
            _file_name = file_name;

            //obtain dimensions of map from file
            using (StreamReader file = new StreamReader(file_name))
            {
                string row;
                _height = 0;

                while ((row = file.ReadLine()) != null)
                {
                    _width = row.Length;

                    _height++;
                }
            }

            //create terrain array
            _terrain = new Tile[_width, _height];

            //read from mapfile and populate terrain array
            using (StreamReader file = new StreamReader(file_name))
            {
                string row;
                int y = 0;

                while ((row = file.ReadLine()) != null)
                {
                    int x = 0;

                    foreach (char c in row)
                    {

                        string type = "";

                        //in the mapfile different numbers represent different terrain types
                        //assigns terrain type
                        switch (c)
                        {
                            case '1':
                                type = "grass";
                                break;
                            case '2':
                                type = "forest";
                                break;
                            case '3':
                                type = "mountain";
                                break;
                            case '6':
                                type = "sand";
                                break;
                            case 'V':
                                type = "village";
                                break;
                            default:
                                type = "grass";
                                break;
                        }

                        //creates new tile and populates array with the new Tile
                        _terrain[x, y] = new Tile(type, x, y);

                        x++;

                        //if the bounds have been reached end the loop
                        if (x == _width)
                        {
                            break;
                        }
                    }

                    y++;
                }
            }

            //this loop defines the connections of each tile to the tiles that surround it to be used by the BFS algorithm
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Tile tile = _terrain[x, y];

                    //tile to the left
                    if (x > 0)
                    {
                        tile.Neighbors.Add(_terrain[x - 1, y]);
                    }

                    //tile to the right
                    if (x < _width - 1)
                    {
                        tile.Neighbors.Add(_terrain[x + 1, y]);
                    }

                    //tile above
                    if (y > 0)
                    {
                        tile.Neighbors.Add(_terrain[x, y - 1]);
                    }

                    //tile underneath
                    if (y < _height - 1)
                    {
                        tile.Neighbors.Add(_terrain[x, y + 1]);
                    }
                }
            }
        }

        //resets the connections between each tile in terms of each tile's parent tile
        public void Reset_Connections()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Tile tile = _terrain[x, y];

                    tile.Neighbors = new List<Tile>();

                    //tile to the left
                    if (x > 0)
                    {
                        tile.Neighbors.Add(_terrain[x - 1, y]);
                    }

                    //tile to the right
                    if (x < _width - 1)
                    {
                        tile.Neighbors.Add(_terrain[x + 1, y]);
                    }

                    //tile above
                    if (y > 0)
                    {
                        tile.Neighbors.Add(_terrain[x, y - 1]);
                    }

                    //tile underneath
                    if (y < _height - 1)
                    {
                        tile.Neighbors.Add(_terrain[x, y + 1]);
                    }
                }
            }

            //resets the parent tile for all tiles to null
            foreach (Tile tile in _terrain)
            {
                tile.Parent = null;
            }
        }

        //resets all tiles to become accessible
        public void Reset_Tile_Access()
        {
            foreach (Tile tile in _terrain)
            {
                tile.Accessible = true;
            }
        }

        //removes tile access
        public void Remove_Tile_Access(Coord pos)
        {
            Tile target_tile = Get_Tile(pos.X, pos.Y);
            target_tile.Accessible = false;
        }

        //returns a tile on the map given a coordinate
        public Tile Get_Tile(int x, int y)
        {
                try
                {
                    return _terrain[x, y];
                }

                catch
                {
                    return null;
                }
        }

        //carries out a Breadth-first search between two tiles on the map to find the shortest optimum route
        public List<Tile> BFS(Coord start_pos, Coord end_pos)
        {
            Reset_Connections();
            Tile start_tile = Get_Tile(start_pos.X, start_pos.Y);
            Tile end_tile = Get_Tile(end_pos.X, end_pos.Y);

            //HashSet used as these are unique elements
            HashSet<Tile> visited = new HashSet<Tile>();
            Queue<Tile> queue = new Queue<Tile>();

            //visit first tile and add to queue
            visited.Add(start_tile);
            queue.Enqueue(start_tile);

            //run as long as queue is not empty
            while (queue.Count != 0)
            {
                Tile current_tile = queue.Dequeue();

                if (current_tile == end_tile)
                {
                    //construct path as it is back to the beginning
                    return Construct_Path(start_tile, end_tile);
                }
                    

                foreach (Tile neighbor in current_tile.Neighbors)
                {
                    //adds neighbor to the queue only if it has not been visited and is accessible
                    if (!visited.Contains(neighbor) && neighbor.Accessible == true)
                    {
                        visited.Add(neighbor);
                        neighbor.Parent = current_tile;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            //no path found
            return null;
        }

        //constructs a path between two map tiles
        private List<Tile> Construct_Path(Tile start_tile, Tile end_tile)
        {
            List<Tile> path = new List<Tile>();
            Tile current_tile = end_tile;

            //chains together a list of tiles using the parent attribute of each tile
            while (current_tile != null)
            {
                path.Insert(0, current_tile);
                current_tile = current_tile.Parent;
            }

            return path;
        }

        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }
    }

}
