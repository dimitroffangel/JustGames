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
            this.Variables = variables;
        }

        private void PlayGame()
        {
            SetTheMode();

            var startTime = DateTime.Now;

           while (!this.Variables.IsGameOver && 
                (this.Variables.EnemiesCurrentlyKilled + this.Variables.EnemiesCurrentlyMissed != this.Variables.EnemiesInitially))
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

            this.Variables.EnemiesKilled += this.Variables.EnemiesCurrentlyKilled;
            this.Variables.EnemiesMissed += this.Variables.EnemiesCurrentlyMissed;
            Console.Clear();

            if (this.Variables.EnemiesCount == 0)
            {
                Console.WriteLine("Game ended");

                if (++this.Variables.CurrentLevel > this.Variables.Highest_Level_Reached)
                    this.Variables.Highest_Level_Reached++;

                this.Variables.IsGameOver = true;
                this.Variables.EnemiesCurrentlyKilled = 0;
                this.Variables.EnemiesCurrentlyMissed = 0;

                // clear all data
                this.Variables.EnemyPositions.Clear();
                this.Variables.TurretsPosition.Clear();
                this.Variables.ObstaclePositions.Clear();
                this.Variables.Hellions.Clear();
                this.Variables.FireTrappers.Clear();
                this.Variables.SlowedEnemies.Clear();
                this.Variables.BleedingEnemies.Clear();
                this.Variables.Grenades.Clear();
                this.Variables.Battleground.Clear();
            }

            else // remake the end game statements 
            {
                Console.WriteLine("The City was overrun");
            }
        }

        private void ExtendTraps()
        {
            for (int i = 0; i <this.Variables.FireTrappers.Count; i++)
                this.Variables.FireTrappers[i].TryExtendingExplosion();
        }

        private void MoveHellions()
        {
            foreach (var hellion in this.Variables.Hellions)
                hellion.Move();
        }

        private void CheckSlowedEnemies()
        {
            for (int i = 0; i < this.Variables.SlowedEnemies.Count; i++)
            {
                var curEnemy = this.Variables.SlowedEnemies[i];

                // if the duration is lower than the time passed since the slow was incarnated
                if ((DateTime.Now - curEnemy.SlowedTime).Seconds >= curEnemy.SlowedDuration)
                {
                    // remove the enemy from the list
                    curEnemy.SlowedDuration = 0;
                    curEnemy.MoveRate = curEnemy.InitialMoveRate;
                    // so the slow time even in the future you check whether there is a slow time to not throw the code in to complete disarray
                    curEnemy.SlowedTime = default(DateTime); 
                    this.Variables.SlowedEnemies.Remove(curEnemy);
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

            int heightAddition = -1;

            foreach(string option in options)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2 + ++heightAddition);
                Console.WriteLine(option);
            }

            int input = int.Parse(Console.ReadLine());

            string configDirectory = "./PlayerConfig.txt";
            
            if(input == 1) // New Game
            {
                LevelSetup level = new LevelSetup(1, ref this.Variables, 100);
                this.Variables.CurrentLevel = 1;
                PlayGame();
            }

            else if(input == 2) // Continue
            { 
                LevelSetup level = new LevelSetup(2, ref this.Variables, 150);
                this.Variables.CurrentLevel = 2;
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

                this.Variables.Total_Earned_Money = int.Parse(stringishMoney.ToString());

                reader.Close();
                Console.WriteLine("\n Press any key to get back in the non-fantasy world");
            }

            else if(input == 5)
            {
                StreamWriter write = new StreamWriter(configDirectory, false);

                write.WriteLine("Current_Money: \" {0} \" ", this.Variables.PocketMoney);
                write.WriteLine("Total_Money: \" {0} \" ", this.Variables.PocketMoney + this.Variables.Total_Earned_Money);
                write.WriteLine("Total_Enemies_Killed: \" {0} \" ", this.Variables.EnemiesKilled);
                write.WriteLine("Total_Enemies_Missed: \" {0} \" ", this.Variables.EnemiesMissed);
                write.WriteLine("Total_Turrets_Lost: \" {0} \" ", this.Variables.TurretsLost);
                write.WriteLine("Total_Ammo_Used: \" {0} \" ", this.Variables.AmmoLost);
                write.WriteLine("Total_Rocks_Felt_On_Turret: \" {0} \" ", this.Variables.Rocks_Felt_On_Turret);
                write.WriteLine("Total_Rocks_Felt_On_You: \" {0} \" ", this.Variables.Rocks_Felt_On_You);
                write.WriteLine("Highest_Level_Reached: \" {0} \" ", this.Variables.Highest_Level_Reached);

                write.Close();
                Environment.Exit(0);
                return;
            }
            
            this.LoadStartingMenu();
        }
      
        public void SetTheMode()
        {
            Console.BufferWidth = Console.WindowWidth;
            Console.BufferHeight = Console.WindowHeight;

            Console.SetCursorPosition(this.Variables.PlayerCol, this.Variables.PlayerRow);
        }

        public void DrawUI()
        {
            Console.SetCursorPosition(Console.WindowWidth - 60, 0);
            Console.Write("Money: {0}, Enemies Killed: {1} Enemies Broken through: {2}"
                ,this.Variables.PocketMoney, this.Variables.EnemiesCurrentlyKilled, this.Variables.EnemiesCurrentlyMissed);
        }

        public void InitiateBleedingsEffect()
        {
            foreach (var bleeding in this.Variables.BleedingEnemies)
                bleeding.ActivateEffect();
        }

        public void InitiateGrenades()
        {
            foreach (var grenades in this.Variables.Grenades)
                grenades.TryExploding();
        }

        public void TurretLogic()
        {
            for (int i = 0; i < this.Variables.TurretsPosition.Count; i++)
            {
                var turret = this.Variables.TurretsPosition[i];
                turret.TryFiring();

                if (turret.Ammo <= 0)
                {
                    // draw the animation
                    Console.SetCursorPosition(turret.Uniq_X, turret.Uniq_Y);

                    if(turret.m_Type == TurretType.FireBunker_Hellion)
                    {
                        this.Variables.Hellions.Remove((Hellion)turret);
                    }

                    Console.Write(" ");

                    // remove the reference to the object so that he can be handled by the garbage collector
                    this.Variables.TurretsPosition.Remove(turret);
                    i--;
                }
            }
        }
        
    }
}
