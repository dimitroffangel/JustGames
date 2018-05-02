using System;
using System.Collections.Generic;

namespace TowerDefence
{
    class LevelSetup
    {
        public int level;
        private SetUpVariables Variables;

        public LevelSetup(int level, ref SetUpVariables variables, int enemies)
        {
            Console.Clear();

            this.Variables = variables;
            this.level = level;
            this.Variables.EnemiesCount = enemies;
            this.Variables.EnemiesInitially = enemies;

            this.LoadLevel();
        }

        #region Levels
        private void LoadLevel()
        {
            if (this.level == 1 || this.level == 2)
                this.Level_One();
        }

        private void Level_One()
        {
                this.Variables.IsGameOver = false;
                this.Variables.CurrentRow = SetUpVariables.InitialRow;
                this.Variables.CurrentCol = SetUpVariables.InitialCol;

                for (int i = 0; i < Console.WindowHeight-6; ++i)
                {
                    Console.SetCursorPosition(this.Variables.CurrentCol, this.Variables.CurrentRow);
                    Console.Write("|");
                    this.Variables.CurrentCol += 2;
                    Console.SetCursorPosition(this.Variables.CurrentCol--, this.Variables.CurrentRow);
                    Console.Write("|");

                
                    this.Variables.Battleground.Add(new Position(this.Variables.CurrentCol, this.Variables.CurrentRow));
                    Console.SetCursorPosition(this.Variables.CurrentCol--, this.Variables.CurrentRow++);
                    Console.Write("!");
                }

                Console.SetCursorPosition(this.Variables.CurrentCol++, this.Variables.CurrentRow);
                Console.Write("|");

                for (int i = 0; i < Console.WindowWidth-35; ++i)
                {
                    this.Variables.Battleground.Add(new Position(this.Variables.CurrentCol, this.Variables.CurrentRow));
                    Console.SetCursorPosition(this.Variables.CurrentCol++, this.Variables.CurrentRow);
                    Console.Write("!");

                    Console.SetCursorPosition(this.Variables.CurrentCol, --this.Variables.CurrentRow);
                    Console.Write("_");
                    Console.SetCursorPosition(this.Variables.CurrentCol, ++this.Variables.CurrentRow + 1);
                    Console.Write("-");
                }
                Console.SetCursorPosition(this.Variables.CurrentCol, this.Variables.CurrentRow);
                Console.Write("!");

                for (int i = 0; i < Console.WindowHeight - 5; ++i)
                {
                    Console.SetCursorPosition(this.Variables.CurrentCol, this.Variables.CurrentRow);
                    Console.Write("|");
                    this.Variables.CurrentCol += 2;
                    Console.SetCursorPosition(this.Variables.CurrentCol--, this.Variables.CurrentRow);
                    Console.Write("|");
                
                    this.Variables.Battleground.Add(new Position(this.Variables.CurrentCol, this.Variables.CurrentRow));
                    Console.SetCursorPosition(this.Variables.CurrentCol--, this.Variables.CurrentRow--);
                    Console.Write("!");
                }
        }
        #endregion
    }
}
