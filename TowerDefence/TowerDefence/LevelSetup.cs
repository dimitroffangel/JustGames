using System;
using System.Collections.Generic;

namespace TowerDefence
{
    class LevelSetup
    {
        public int m_Level;
        private SetUpVariables internal_Variables;

        public LevelSetup(int level, ref SetUpVariables variables, int enemies)
        {
            Console.Clear();
            m_Level = level;

            internal_Variables = variables;
            internal_Variables.EnemiesCount = enemies;
            internal_Variables.EnemiesInitially = enemies;

            this.LoadLevel();
        }

        #region Levels
        private void LoadLevel()
        {
            if (m_Level == 1 || m_Level == 2)
                this.Level_One();
        }

        private void Level_One()
        {
                internal_Variables.IsGameOver = false;
                internal_Variables.CurrentRow = SetUpVariables.InitialRow;
                internal_Variables.CurrentCol = SetUpVariables.InitialCol;

                for (int i = 0; i < Console.WindowHeight-6; ++i)
                {
                    Console
                    .SetCursorPosition(internal_Variables.CurrentCol, internal_Variables.CurrentRow);
                    Console.Write("|");
                    internal_Variables.CurrentCol += 2;
                    Console.SetCursorPosition(internal_Variables.CurrentCol--, 
                        internal_Variables.CurrentRow);
                    Console.Write("|");
                
                    internal_Variables.Battleground
                    .Add(new Position(internal_Variables.CurrentCol, internal_Variables.CurrentRow));
                    Console
                    .SetCursorPosition(internal_Variables.CurrentCol--, internal_Variables.CurrentRow++);
                    Console.Write("!");
                }

                Console
                .SetCursorPosition(internal_Variables.CurrentCol++, internal_Variables.CurrentRow);
                Console.Write("|");

                for (int i = 0; i < Console.WindowWidth-35; ++i)
                {
                    internal_Variables.Battleground.Add(new Position(internal_Variables.CurrentCol, internal_Variables.CurrentRow));
                    Console.SetCursorPosition(internal_Variables.CurrentCol++, internal_Variables.CurrentRow);
                    Console.Write("!");

                    Console.SetCursorPosition(internal_Variables.CurrentCol, --internal_Variables.CurrentRow);
                    Console.Write("_");
                    Console.SetCursorPosition(internal_Variables.CurrentCol, ++internal_Variables.CurrentRow + 1);
                    Console.Write("-");
                }
                Console.SetCursorPosition(internal_Variables.CurrentCol, internal_Variables.CurrentRow);
                Console.Write("!");

                for (int i = 0; i < Console.WindowHeight - 5; ++i)
                {
                    Console.SetCursorPosition(internal_Variables.CurrentCol, internal_Variables.CurrentRow);
                    Console.Write("|");
                    internal_Variables.CurrentCol += 2;
                    Console.SetCursorPosition(internal_Variables.CurrentCol--, internal_Variables.CurrentRow);
                    Console.Write("|");
                
                    internal_Variables.Battleground.Add(new Position(internal_Variables.CurrentCol, internal_Variables.CurrentRow));
                    Console.SetCursorPosition(internal_Variables.CurrentCol--, internal_Variables.CurrentRow--);
                    Console.Write("!");
                }
        }
        #endregion
    }
}
