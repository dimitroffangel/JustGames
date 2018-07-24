using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using TowerDefence.Objects.Turrets.FlameTurrets;

namespace TowerDefence
{
    class GameActions : HumanInterface
    {
        public GameActions(ref SetUpVariables variables)
            : base(ref variables)
        {

        }

        private void PlayGame()
        {
            SetTheMode();

            var startTime = DateTime.Now;

           while (!internal_Variables.IsGameOver && 
                (internal_Variables.EnemiesCurrentlyKilled + internal_Variables.EnemiesCurrentlyMissed != internal_Variables.EnemiesInitially))
            {
                Thread.Sleep(100);

                DrawUI();
                ReadInput();

                // the time is augmented in order to ensure authority over when the enemies to start the invasion
                var endTime = DateTime.Now - startTime;

                if (endTime.TotalSeconds < SetUpVariables.warmUPTime)
                    continue;

                SpawnEnemies();
                ExtendTraps();
                MoveHellions();
                CheckSlowedEnemies();
                TurretLogic();
                InitiateBleedingsEffect();
                InitiateGrenades();
            }

            internal_Variables.EnemiesKilled += internal_Variables.EnemiesCurrentlyKilled;
            internal_Variables.EnemiesMissed += internal_Variables.EnemiesCurrentlyMissed;
            Console.Clear();

            if (internal_Variables.EnemiesCount == 0)
            {
                Console.WriteLine("Game ended");

                if (++internal_Variables.CurrentLevel > internal_Variables.Highest_Level_Reached)
                    internal_Variables.Highest_Level_Reached++;

                internal_Variables.IsGameOver = true;
                internal_Variables.EnemiesCurrentlyKilled = 0;
                internal_Variables.EnemiesCurrentlyMissed = 0;

                // clear all data
                internal_Variables.EnemyPositions.Clear();
                internal_Variables.TurretsPosition.Clear();
                internal_Variables.ObstaclePositions.Clear();
                internal_Variables.Hellions.Clear();
                internal_Variables.FireTrappers.Clear();
                internal_Variables.SlowedEnemies.Clear();
                internal_Variables.BleedingEnemies.Clear();
                internal_Variables.Grenades.Clear();
                internal_Variables.Battleground.Clear();
            }

            else // remake the end game statements 
            {
                Console.WriteLine("The City was overrun");
            }
        }

        private void ExtendTraps()
        {
            for (int i = 0; i <internal_Variables.FireTrappers.Count; i++)
                internal_Variables.FireTrappers[i].TryExtendingExplosion();
        }

        private void MoveHellions()
        {
            foreach (var hellion in internal_Variables.Hellions)
                hellion.Move();
        }

        private void CheckSlowedEnemies()
        {
            for (int i = 0; i < internal_Variables.SlowedEnemies.Count; i++)
            {
                var curEnemy = internal_Variables.SlowedEnemies[i];

                // if the duration is lower than the time passed since the slow was incarnated
                if ((DateTime.Now - curEnemy.SlowedTime).Seconds >= curEnemy.SlowedDuration)
                {
                    // remove the enemy from the list
                    curEnemy.SlowedDuration = 0;
                    curEnemy.MoveRate = curEnemy.InitialMoveRate;
                    // so the slow time even in the future you check whether there is a slow time to not throw the code in to complete disarray
                    curEnemy.SlowedTime = default(DateTime); 
                    internal_Variables.SlowedEnemies.Remove(curEnemy);
                    i--;
                }

            }
        }

        public void LoadStartingMenu()
        {

            List<string> options = new List<string>()
            {
                "1) New Game",
                "2) Continue",
                "3) Browse Levels",
                "4) Hall of Fame",
                "5) Exit"
            };

            int attempt;

            int heightAddition = -1;

            foreach(string option in options)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2 + ++heightAddition);
                Console.WriteLine(option);
            }

            int input = 0;

            while(!int.TryParse(Console.ReadLine(), out input))
            {
                Console.SetCursorPosition(0, 0);
                Console.Write("Please enter a number");
            }

            string configDirectory = "./PlayerConfig.txt";
            
            if(input == 1) // New Game
            {
                LevelSetup level = new LevelSetup(1, ref internal_Variables, 1000);
                internal_Variables.CurrentLevel = 1;
                PlayGame();
            }

            else if(input == 2) // Continue
            { 
                LevelSetup level = new LevelSetup(2, ref internal_Variables, 150);
                internal_Variables.CurrentLevel = 2;
                PlayGame();
            }
            
            else if(input == 3)
            {
                Console.Clear();

                string directory = "/TowerDefense_VS/TurretsConfig.txt";
                directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + directory;

                StreamReader reader = new StreamReader(directory);
                Console.WriteLine(reader.ReadToEnd());
            }

            else if(input == 4) // Enter Hall of Frame
            {
                Console.Clear();
                    
                StreamReader reader = new StreamReader(configDirectory);

                string fileContent = reader.ReadToEnd();
                fileContent = fileContent.Trim('"');
                Console.WriteLine(fileContent);

                //start reading from the beginning
                reader.DiscardBufferedData();
                reader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);

                reader.ReadLine();
                string secondLine = reader.ReadLine();
                StringBuilder stringishMoney = new StringBuilder();

                foreach(var character in secondLine)
                {
                    if (Char.IsDigit(character))
                        stringishMoney.Append(character);
                }

                internal_Variables.Total_Earned_Money = int.Parse(stringishMoney.ToString());

                reader.Close();
                Console.WriteLine("\n Press any key to get back in the non-fantasy world");
            }

            else if(input == 5)
            {
                StreamWriter write = new StreamWriter(configDirectory, false);

                write.WriteLine("Current_Money: \" {0} \" ", internal_Variables.PocketMoney);
                write.WriteLine("Total_Money: \" {0} \" ", internal_Variables.PocketMoney + internal_Variables.Total_Earned_Money);
                write.WriteLine("Total_Enemies_Killed: \" {0} \" ", internal_Variables.EnemiesKilled);
                write.WriteLine("Total_Enemies_Missed: \" {0} \" ", internal_Variables.EnemiesMissed);
                write.WriteLine("Total_Turrets_Lost: \" {0} \" ", internal_Variables.TurretsLost);
                write.WriteLine("Total_Ammo_Used: \" {0} \" ", internal_Variables.AmmoLost);
                write.WriteLine("Total_Rocks_Felt_On_Turret: \" {0} \" ", internal_Variables.Rocks_Felt_On_Turret);
                write.WriteLine("Total_Rocks_Felt_On_You: \" {0} \" ", internal_Variables.Rocks_Felt_On_You);
                write.WriteLine("Highest_Level_Reached: \" {0} \" ", internal_Variables.Highest_Level_Reached);

                write.Close();
                Environment.Exit(0);
                return;
            }
            
            LoadStartingMenu();
        }
      
        public void SetTheMode()
        {
            Console.BufferWidth = Console.WindowWidth;
            Console.BufferHeight = Console.WindowHeight;

            Console.SetCursorPosition(internal_Variables.PlayerCol, internal_Variables.PlayerRow);
        }

        public void DrawUI()
        {
            Console.SetCursorPosition(Console.WindowWidth - 60, 0);
            Console.Write("Money: {0}, Enemies Killed: {1} Enemies Broken through: {2}"
                ,internal_Variables.PocketMoney, internal_Variables.EnemiesCurrentlyKilled, internal_Variables.EnemiesCurrentlyMissed);
        }

        public void InitiateBleedingsEffect()
        {
            foreach (var bleeding in internal_Variables.BleedingEnemies)
                bleeding.ActivateEffect();
        }

        public void InitiateGrenades()
        {
            foreach (var grenades in internal_Variables.Grenades)
                grenades.TryExploding();
        }

        public void TurretLogic()
        {
            for (int i = 0; i < internal_Variables.TurretsPosition.Count; i++)
            {
                var turret = internal_Variables.TurretsPosition[i];
                turret.TryFiring();

                if (turret.Ammo <= 0)
                {
                    // draw the animation
                    Console.SetCursorPosition(turret.X, turret.Y);

                    if(turret.m_Type == TurretType.FireBunker_Hellion)
                    {
                        internal_Variables.Hellions.Remove((Hellion)turret);
                    }

                    Console.Write(" ");

                    // remove the reference to the object so that he can be handled by the garbage collector
                    internal_Variables.TurretsPosition.Remove(turret);
                    i--;
                }
            }
        }
        
    }
}
