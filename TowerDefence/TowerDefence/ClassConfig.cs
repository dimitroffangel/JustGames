using System;
using System.IO;
using System.Text;

namespace TowerDefence
{
    class ClassConfig
    {
        private SetUpVariables internal_Variables;

        public ClassConfig(ref SetUpVariables variables)
        {
            internal_Variables = variables;
        }

        public void ReadImportantInformation()
        {
            StreamReader reader = new StreamReader("./PlayerConfig.txt");

            // the lines in the file are 9
            this.ReadLine(reader.ReadLine(), ref internal_Variables.PocketMoney);
            reader.ReadLine();
            this.ReadLine(reader.ReadLine(), ref internal_Variables.EnemiesKilled);
            this.ReadLine(reader.ReadLine(), ref internal_Variables.EnemiesMissed);
            this.ReadLine(reader.ReadLine(), ref internal_Variables.TurretsLost);
            this.ReadLine(reader.ReadLine(), ref internal_Variables.AmmoLost);
            this.ReadLine(reader.ReadLine(), ref internal_Variables.Rocks_Felt_On_Turret);
            this.ReadLine(reader.ReadLine(), ref internal_Variables.Rocks_Felt_On_You);
            this.ReadLine(reader.ReadLine(), ref internal_Variables.Highest_Level_Reached);

            reader.Close();
        }

        private void ReadLine(string line, ref int updateValue)
        {
            StringBuilder stringValue = new StringBuilder();

            foreach(char character in line)
            {
                if(Char.IsDigit(character))
                {
                    stringValue.Append(character);
                }
            }

            updateValue = int.Parse(stringValue.ToString());
        }

    }
}
