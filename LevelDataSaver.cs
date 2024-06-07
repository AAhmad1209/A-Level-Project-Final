using Delegate_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace A_LEVEL_PROJECT
{
    //Saves level data to an external file
    internal class LevelDataSaver
    {
        //constructor
        public LevelDataSaver()
        {
        }

        //saves LevelData object to an external binary file
        public void Save(LevelData data)
        {
            //serialises the object to an external binary file
            string file_path = "leveldata.bin";
            Serialise(data, file_path); 
        }

        //serialises the LevelData object to an external binary file
        public void Serialise(LevelData data, string file_path)
        {
            try
            {
                //this ensures that it is overwritten every time
                using (FileStream fs = new FileStream(file_path, FileMode.Create))
                {
                    //uses BinaryFormatter to serialize data
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, data);
                }
            }

            catch (Exception ex)
            {
                
            }
        }

    }
}
