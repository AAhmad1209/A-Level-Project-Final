using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Model
{
    //Loads level data from an external file
    internal class LevelDataLoader
    {
        //constructor
        public LevelDataLoader()
        {
        }

        //loads level data from an external binary file and returns it
        public LevelData Load()
        {
            LevelData deserialised_data = Deserialise("leveldata.bin");

            return deserialised_data;
        }

        //deserialises LevelData object from a binary file
        public LevelData Deserialise(string file_path)
        {
            LevelData data = null;

            try
            {
                using (FileStream fs = new FileStream(file_path, FileMode.Open))
                {
                    //uses BinaryFormatter to deserialize data
                    BinaryFormatter formatter = new BinaryFormatter();
                    data = (LevelData)formatter.Deserialize(fs);
                }
            }

            catch (Exception ex)
            {
                
            }

            return data;
        }

    }
}
