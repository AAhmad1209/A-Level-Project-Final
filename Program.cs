using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Program class
    class Program
    {
        //entry point of program
        static void Main(string[] args)
        {
            //in the main UI a player can choose to resume a game or create a new game
            MainUI main_UI = new MainUI();

            //starts the main UI sequence
            main_UI.UI_Sequence();
         
        }
    }

}