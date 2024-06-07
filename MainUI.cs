using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //This represents the main menu and all of the game setup screens
    internal class MainUI
    {
        //constructor
        public MainUI() { }

        //allows the user to setup a game or resume an existing one from the main menu and configuration screens
        public void UI_Sequence()
        {

            //font size 18
            Console.SetWindowSize(150, 55);

            //allow user to select between the 4 initial menu options

            int selection_pointer = 0;

            List<string> options = new List<string>();
            options.Add("New Game");
            options.Add("Resume Game");
            options.Add("About");
            options.Add("Quit");

            string selection = "";

            Console.WriteLine("Battle of Consoletopia");

            //loops until the player has selected 1 of the 4 options by pressing enter
            while (true)
            {
                //sets default display properties
                Console.ResetColor();
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 0);

                Console.WriteLine();

                //used to set line that option is displayed on using the SetCursorPosition method
                int line_displacement = 1;

                //displays all menu options
                foreach (string option in options)
                {
                    //selected option in green
                    if (options[selection_pointer] == option)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;

                        //changes the selected option based on the selection pointer 
                        selection = option;
                    }

                    //non-selected option in white
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    //displays the option
                    Console.SetCursorPosition(0, line_displacement + 1);
                    Console.WriteLine(@"{0}) {1}", line_displacement, option);

                    line_displacement++;

                }



                ConsoleKeyInfo press = Console.ReadKey();

                //changes selection pointer based on user key press

                //up arrow reduces selection pointer
                if (press.Key == ConsoleKey.UpArrow)
                {
                    //if it is 0 change to 3 which is the final index
                    //this means when up is pressed at the top the bottom option will become selected
                    if (selection_pointer == 0)
                    {
                        selection_pointer = 3;
                    }

                    else
                    {
                        selection_pointer--;
                    }

                }

                //down arrow increases selection pointer
                else if (press.Key == ConsoleKey.DownArrow)
                {
                    //if it is 3 change to 0 which is the first index
                    //this means when down is pressed at the bottom the top option will become selected
                    if (selection_pointer == 3)
                    {
                        selection_pointer = 0;
                    }

                    else
                    {
                        selection_pointer++;
                    }
                }

                //if an option is selected by pressing enter break the loop
                else if (press.Key == ConsoleKey.Enter)
                {
                    break;
                }
            }

            Game game;

            //branches depending on selection

            //for resuming a game
            if (selection == "Resume Game")
            {
                
                Console.Clear();

                //loads the game from the external file
                LevelDataLoader loader = new LevelDataLoader();
                LevelData level_data = loader.Load();

                //creates a new game using the original game information
                game = new Game(level_data);

                //initiates the game
                game.Play();
            }

            //for creating a new game
            else if (selection == "New Game")
            {
                Console.Clear();

                //gets the number of players for the new game
                int number_of_players = Get_Number_Of_Players();

                Console.Clear();

                List<Player> players = new List<Player>();

                //loops for the number of players specified, creates the players and allows them to be configured
                for (int i = 0; i < number_of_players; i++)
                {
                    Console.Clear();
                    Console.WriteLine(@"Player {0} Setup", i + 1);

                    //gets the player type
                    string player_type = Get_Player_Type();
                    
                    //allows the difficulty level to be set for AI players
                    if (player_type == "AI")
                    {
                        string difficulty = Get_AI_Difficulty();

                        string name = "AI " + (i+1).ToString();

                        Player new_AI_player = new Player(name, 100, player_type, new TechTree("Tech Tree"));
                        new_AI_player.Difficulty = difficulty.ToLower();

                        players.Add(new_AI_player);
                    }

                    //allows human player names to be chosen by the user
                    else if (player_type == "Human")
                    {
                        Console.Clear();
                        Console.WriteLine();
                        Console.WriteLine(@"Enter player {0}'s name: ", i+1);
                        Console.WriteLine();

                        string name = Console.ReadLine();

                        Player new_human_player = new Player(name, 100, player_type, new TechTree("Tech Tree"));

                        players.Add(new_human_player);
                    }
                }

                string map_path = "Maps\\mapfile1.txt";

                //creates the new game
                game = new Game("domination", players, map_path);

                Console.Clear();

                Console.WriteLine("Press enter to begin...");

                Console.ReadKey();

                Console.Clear();

                //initiates the new game
                game.Play();
            }

            //for the about screen
            else if (selection == "About")
            {
                Console.Clear();
                Console.WriteLine("Just a project..");
            }

            //for quitting
            else if (selection == "Quit")
            {
                System.Environment.Exit(1);
            }
            
        }

        //allows the user to choose a player as being AI or human
        public string Get_Player_Type()
        {
            Console.WriteLine();

            //allow user to select between the menu  options

            int selection_pointer = 0;

            List<string> options = new List<string>();

            options.Add("Human");
            options.Add("AI");
            

            string selection = "";

            Console.WriteLine("Select Type Of Player");

            //loops until the player has selected 1 of the 2 options by pressing enter
            while (true)
            {

                //sets default display properties

                Console.ResetColor();
                Console.CursorVisible = false;

                Console.WriteLine();


                //used to set line that option is displayed on using the SetCursorPosition method
                int line_displacement = 3;

                //displays all menu options
                foreach (string option in options)
                {
                    //selected option in green
                    if (options[selection_pointer] == option)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        selection = option;
                    }

                    //non-selected option in white
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    //displays the option
                    Console.SetCursorPosition(0, line_displacement + 1);
                    Console.WriteLine(@"- {0}", option);

                    line_displacement++;

                }


                ConsoleKeyInfo press = Console.ReadKey();

                //changes selection pointer based on user key press

                //up arrow reduces selection pointer
                if (press.Key == ConsoleKey.UpArrow)
                {
                    //if it is 0 change to 1 which is the final index
                    //this means when up is pressed at the top the bottom option will become selected
                    if (selection_pointer == 0)
                    {
                        selection_pointer = 1;
                    }

                    else
                    {
                        selection_pointer--;
                    }

                }

                //down arrow increases selection pointer
                else if (press.Key == ConsoleKey.DownArrow)
                {
                    //if it is 1 change to 0 which is the first index
                    //this means when down is pressed at the bottom the top option will become selected
                    if (selection_pointer == 1)
                    {
                        selection_pointer = 0;
                    }

                    else
                    {
                        selection_pointer++;
                    }
                }

                //if an option is selected by pressing enter break the loop
                else if (press.Key == ConsoleKey.Enter)
                {
                    break;
                }
            }

            Console.ResetColor();

            return selection;
        }

        //allows the user to set an AI player's difficulty
        public string Get_AI_Difficulty()
        {

            Console.Clear();

            //allow user to select between the 4 menu options

            int selection_pointer = 0;

            List<string> options = new List<string>();

            options.Add("Easy");
            options.Add("Medium");
            options.Add("Hard");
            options.Add("Extreme");


            string selection = "";

            Console.WriteLine("Select AI difficulty");

            //loops until the player has selected 1 of the 4 options by pressing enter
            while (true)
            {

                //sets default display properties
                Console.ResetColor();
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 0);



                Console.WriteLine();


                //used to set line that option is displayed on using the SetCursorPosition method
                int line_displacement = 1;

                //displays all menu options
                foreach (string option in options)
                {
                    //selected option in green
                    if (options[selection_pointer] == option)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;

                        //changes the selected option based on the selection pointer 
                        selection = option;
                    }

                    //non-selected option in white
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    //displays the option
                    Console.SetCursorPosition(0, line_displacement + 1);
                    Console.WriteLine(@"- {0}", option);

                    line_displacement++;

                }


                ConsoleKeyInfo press = Console.ReadKey();

                //changes selection pointer based on user key press

                //up arrow reduces selection pointer
                if (press.Key == ConsoleKey.UpArrow)
                {
                    //if it is 0 change to 3 which is the final index
                    //this means when up is pressed at the top the bottom option will become selected
                    if (selection_pointer == 0)
                    {
                        selection_pointer = 3;
                    }

                    else
                    {
                        selection_pointer--;
                    }

                }

                //down arrow increases selection pointer
                else if (press.Key == ConsoleKey.DownArrow)
                {
                    //if it is 3 change to 0 which is the first index
                    //this means when down is pressed at the bottom the top option will become selected
                    if (selection_pointer == 3)
                    {
                        selection_pointer = 0;
                    }

                    else
                    {
                        selection_pointer++;
                    }
                }

                //if an option is selected by pressing enter break the loop
                else if (press.Key == ConsoleKey.Enter)
                {
                    break;
                }
            }
            Console.ResetColor();

            return selection;
        }

        //allows the user to select the number of players
        public int Get_Number_Of_Players()
        {
            Console.Clear();

            //allow user to select between the 4 menu options

            int selection_pointer = 0;

            List<string> options = new List<string>();

            options.Add("1");
            options.Add("2");
            options.Add("3");
            options.Add("4");

            string selection = "";

            Console.WriteLine("Select Number Of Players");

            //loops until the player has selected 1 of the 4 options by pressing enter
            while (true)
            {
                //sets default display properties
                Console.ResetColor();
                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 0);



                Console.WriteLine();

                //used to set line that option is displayed on using the SetCursorPosition method
                int line_displacement = 1;

                //displays all menu options
                foreach (string option in options)
                {
                    //selected option in green
                    if (options[selection_pointer] == option)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;

                        //changes the selected option based on the selection pointer
                        selection = option;
                    }

                    //non-selected option in white
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    //displays the option
                    Console.SetCursorPosition(0, line_displacement + 1);
                    Console.WriteLine(@"- {0}", option);

                    line_displacement++;

                }

                ConsoleKeyInfo press = Console.ReadKey();

                //changes selection pointer based on user key press

                //up arrow reduces selection pointer
                if (press.Key == ConsoleKey.UpArrow)
                {
                    //if it is 0 change to 3 which is the final index
                    //this means when up is pressed at the top the bottom option will become selected
                    if (selection_pointer == 0)
                    {
                        selection_pointer = 3;
                    }

                    else
                    {
                        selection_pointer--;
                    }

                }

                //down arrow increases selection pointer
                else if (press.Key == ConsoleKey.DownArrow)
                {
                    //if it is 3 change to 0 which is the first index
                    //this means when down is pressed at the bottom the top option will become selected
                    if (selection_pointer == 3)
                    {
                        selection_pointer = 0;
                    }

                    else
                    {
                        selection_pointer++;
                    }
                }

                //if an option is selected by pressing enter break the loop
                else if (press.Key == ConsoleKey.Enter)
                {
                    break;
                }
            }
            Console.ResetColor();

            //return as integer value
            return Convert.ToInt32(selection);
        }
    }
}
