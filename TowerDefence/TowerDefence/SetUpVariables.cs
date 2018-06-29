using System;
using System.Collections.Generic;
using TowerDefence.Objects;
using TowerDefence.Objects.Turrets.FlameTurrets;

namespace TowerDefence
{
    class SetUpVariables
    {
        public const int InitialRow = 2;
        public const int InitialCol = 2;

        // Turrets level Damage
        public const float Level_1_Cannon_Damage = 60;
        public const float Level_2_Cannon_Damage = 27;
        public const float Level_3_Cannon_Damage = 40;
        public const float Level_1_FireBunker_Damage = 64;
        public const float Level_2_FireBunker_Damage = 52;
        public const float Level_3_FireBunker_Damage = 72;
        public const float Level_1_MissleBunker_Damage = 34;
        public const float Level_2_MissleBunker_Damage = 48;
        public const float Level_3_MissleBunker_Damage = 52;

        public const float Level_1_Tardus_Slow_Duration = 2.5f;

        // enemies time before invading the territory
        public const int warmUPTime = 2;

        public int CurrentRow = InitialRow;
        public int CurrentCol = InitialCol;
        
        public int PlayerRow = InitialRow + 1;
        public int PlayerCol = InitialCol + 3;
        public char PlayerView = 'v';

        public List<Position> Battleground = new List<Position>();
        public List<Enemy> EnemyPositions = new List<Enemy>();
        public List<Turret> TurretsPosition = new List<Turret>() ;
        public List<Position> ObstaclePositions = new List<Position>();
        public List<FireTrapper> FireTrappers = new List<FireTrapper>();
        public List<Enemy> SlowedEnemies = new List<Enemy>();
        public List<Hellion> Hellions = new List<Hellion>();
        public List<BleedingEffect> BleedingEnemies = new List<BleedingEffect>();
        public List<Grenade> Grenades = new List<Grenade>();

        public int CurrentLevel = 1;

        // Hall of fame

        /* Cost:
            Cannon -> 50
            FireBunker -> 340
            MIssleBunker -> 165
            LaserAnnihilator -> 500
        */

        public int Total_Earned_Money = 0;
        public int PocketMoney = 0;
        public int EnemiesKilled = 0;
        public int EnemiesCurrentlyKilled = 0;
        public int EnemiesCurrentlyMissed = 0;
        public int EnemiesMissed = 0;
        public int EnemiesCount = 0;
        public int EnemiesInitially = 0;
        public int TurretsLost = 0;
        public int AmmoLost = 0;
        public int Rocks_Felt_On_Turret = 0;
        public int Rocks_Felt_On_You = 0;
        public int Highest_Level_Reached = 0;
        
        public bool IsGameOver;

        internal bool EnemyHasPosition(int x, int y)
        {
            foreach (var enemy in this.EnemyPositions)
            {
                if (enemy.Uniq_X == x && enemy.Uniq_Y == y)
                    return true;
            }

            return false;
        }

        internal Enemy GetEnemy(int x, int y)
        {
            foreach (var enemy in this.EnemyPositions)
            {
                if (enemy.Uniq_X == x && enemy.Uniq_Y == y)
                    return enemy;
            }

            return null;
        }
    }
}
