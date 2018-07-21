using System;
using System.Collections.Generic;

namespace TowerDefence.Objects.Turrets.FlameTurrets
{
    class Hellion : FlameThrower
    {
        private SetUpVariables internal_Variables;
        private DateTime m_timeDeparture;
        public const float m_MoveRate = 0.4f;
        
        private List<Position> moveCommands;
        private Position initialTargetPosition;
        private int currentCommands;
        private List<int> availableBattlegrounds;

        private List<Position> bfsCommands;

        public Hellion(int x, int y, TurretType turretType, TurretPlacement placement, 
            ref SetUpVariables variables) : base(x, y, turretType, placement, ref variables)
        {
            internal_Variables = variables;
            m_timeDeparture = DateTime.Now;
            moveCommands = new List<Position>();
            currentCommands = 0;

            bfsCommands = new List<Position>();
        }

        private void FindTarget()
        {
            float minDistance = int.MaxValue;
            int minIndex = -1;
            int index = -1;
            bool hasRemovedHunter = false;

            foreach (var enemy in internal_Variables.EnemyPositions)
            {
                index++;
                float curDistance = this.DistanceBetween(enemy);

                if (minDistance > curDistance)
                {
                    if((this.Target != null && (this.Target.Uniq_X != enemy.Uniq_X && this.Target.Uniq_Y != enemy.Uniq_Y)) || 
                        this.Target == null)
                    {
                        if(enemy.TargetedBy.Count == 4)
                            continue;

                        minIndex = index;
                        minDistance = curDistance;
                    }
                }
            }

            if (internal_Variables.EnemyPositions.Count == 0 || minIndex < 0)
                return;

            this.Target = internal_Variables.EnemyPositions[minIndex];
            this.Target.TargetedBy.Add(this);
            this.initialTargetPosition = new Position(this.Target.Uniq_X, this.Target.Uniq_Y);

            // find the moving direction of the target
        }

        
        // check every position that surrounds the current position 
        // order them from closest to the target to farthest

        // FindShortestPath() to target
        // calculate the speed with which the unit will reach the target or consecutive positions near it
        // on each new fallen rock the FindShortestPath must be reevaluated 

         /* 1) Get a List with the map
          * 2) a recursive function to do the magic trick
          * 3) the functions goes from the current positon to the block which has the closest distance to the target
          * 4) when checking a position it should check if the movesCounter is bigger then the lowest amount of 
          *  needed moves to reach the destination
          */

        private void FindAllBattlePaths()
        {
            /*
             * foreach battle block find the shortest path to it -> FindBfsPath(block, this.Position, currentMoves)
            */
        }

        private void FindBfsPath(Position enemy, Position currentPosition)
        {
            if (this.Target == null || this.Target.GetHealthStatus() <= 0)
                return;

            // check if there is already such path found 
            // declare all the possible roads, check if there is an obstacle on 'em if there is not
            // move from there
            float currentDistance = 100000001;
            float minDistance = 100000001;
            Position nearestPoint = new Position(0, 0);
            Position currentDirection = new Position(0, 0);

            // top
            if ((currentPosition.Uniq_X >= 0 && currentPosition.Uniq_X < Console.WindowWidth &&
                currentPosition.Uniq_Y - 1 >= 0 && currentPosition.Uniq_Y - 1 < Console.WindowHeight &&
                !ContainsBattleground(currentPosition.Uniq_X, currentPosition.Uniq_Y - 1) &&
                !ContainsMove(currentPosition.Uniq_X, currentPosition.Uniq_Y - 1) &&
                !ContainsObstacle(currentPosition.Uniq_X, currentPosition.Uniq_Y - 1)) ||
                new Position(currentPosition.Uniq_X, currentPosition.Uniq_Y - 1) == enemy)
            {
                currentDirection = new Position(currentPosition.Uniq_X, currentPosition.Uniq_Y - 1);

                if (enemy == currentDirection)
                    return;

                currentDistance = enemy.DistanceBetween(currentDirection);

                if (currentDistance < minDistance)
                {
                    nearestPoint = currentDirection;
                    minDistance = currentDistance;
                }
            }

            // top-right
            if ((currentPosition.Uniq_X + 1 >= 0 && currentPosition.Uniq_X + 1 < Console.WindowWidth &&
                currentPosition.Uniq_Y - 1 >= 0 && currentPosition.Uniq_Y - 1 < Console.WindowHeight &&
                !ContainsBattleground(currentPosition.Uniq_X + 1, currentPosition.Uniq_Y - 1) &&
                !ContainsMove(currentPosition.Uniq_X + 1, currentPosition.Uniq_Y - 1) &&
                !ContainsObstacle(currentPosition.Uniq_X + 1, currentPosition.Uniq_Y - 1)) ||
                new Position(currentPosition.Uniq_X + 1, currentPosition.Uniq_Y - 1) == enemy)
            {
                currentDirection = new Position(currentPosition.Uniq_X + 1, currentPosition.Uniq_Y - 1);

                if (enemy == currentDirection)
                    return;

                currentDistance = enemy.DistanceBetween(currentDirection);

                if (currentDistance < minDistance)
                {
                    nearestPoint = currentDirection;
                    minDistance = currentDistance;
                }
            }
            // right
            if ((currentPosition.Uniq_X + 1 >= 0 && currentPosition.Uniq_X + 1 < Console.WindowWidth &&
                currentPosition.Uniq_Y >= 0 && currentPosition.Uniq_Y < Console.WindowHeight &&
                !ContainsBattleground(currentPosition.Uniq_X + 1, currentPosition.Uniq_Y) &&
                !ContainsMove(currentPosition.Uniq_X + 1, currentPosition.Uniq_Y) &&
                !ContainsObstacle(currentPosition.Uniq_X + 1, currentPosition.Uniq_Y)) ||
                new Position(currentPosition.Uniq_X + 1, currentPosition.Uniq_Y) == enemy)
            {
                currentDirection = new Position(currentPosition.Uniq_X + 1, currentPosition.Uniq_Y);

                if (enemy == currentDirection)
                    return;

                currentDistance = enemy.DistanceBetween(currentDirection);

                if (currentDistance < minDistance)
                {
                    nearestPoint = currentDirection;
                    minDistance = currentDistance;
                }
            }
            // bot-right

            if ((currentPosition.Uniq_X + 1 >= 0 && currentPosition.Uniq_X + 1 < Console.WindowWidth &&
                currentPosition.Uniq_Y + 1 >= 0 && currentPosition.Uniq_Y + 1 < Console.WindowHeight &&
                !ContainsBattleground(currentPosition.Uniq_X + 1, currentPosition.Uniq_Y + 1) &&
                !ContainsMove(currentPosition.Uniq_X + 1, currentPosition.Uniq_Y + 1) &&
                !ContainsObstacle(currentPosition.Uniq_X + 1, currentPosition.Uniq_Y + 1)) ||
                new Position(currentPosition.Uniq_X + 1, currentPosition.Uniq_Y + 1) == enemy)
            {
                currentDirection = new Position(currentPosition.Uniq_X + 1, currentPosition.Uniq_Y + 1);


                if (enemy == currentDirection)
                    return;

                currentDistance = enemy.DistanceBetween(currentDirection);

                if (currentDistance < minDistance)
                {
                    nearestPoint = currentDirection;
                    minDistance = currentDistance;
                }
            }
            // bot
            if ((currentPosition.Uniq_X >= 0 && currentPosition.Uniq_X < Console.WindowWidth &&
                currentPosition.Uniq_Y + 1 >= 0 && currentPosition.Uniq_Y + 1 < Console.WindowHeight &&
                !ContainsBattleground(currentPosition.Uniq_X, currentPosition.Uniq_Y + 1) &&
                !ContainsMove(currentPosition.Uniq_X, currentPosition.Uniq_Y + 1) &&
                !ContainsObstacle(currentPosition.Uniq_X, currentPosition.Uniq_Y + 1)) ||
                new Position(currentPosition.Uniq_X, currentPosition.Uniq_Y + 1) == enemy)
            {
                currentDirection = new Position(currentPosition.Uniq_X, currentPosition.Uniq_Y + 1);

                if (enemy == currentDirection)
                    return;

                currentDistance = enemy.DistanceBetween(currentDirection);

                if (currentDistance < minDistance)
                {
                    nearestPoint = currentDirection;
                    minDistance = currentDistance;
                }
            }
            // bot-left
            if ((currentPosition.Uniq_X - 1 >= 0 && currentPosition.Uniq_X - 1 < Console.WindowWidth &&
                currentPosition.Uniq_Y + 1 >= 0 && currentPosition.Uniq_Y + 1 < Console.WindowHeight &&
                !ContainsBattleground(currentPosition.Uniq_X - 1, currentPosition.Uniq_Y + 1) &&
                !ContainsMove(currentPosition.Uniq_X - 1, currentPosition.Uniq_Y + 1) &&
                !ContainsObstacle(currentPosition.Uniq_X - 1, currentPosition.Uniq_Y + 1)) ||
                new Position(currentPosition.Uniq_X - 1, currentPosition.Uniq_Y + 1) == enemy)
            {
                currentDirection =  new Position(currentPosition.Uniq_X - 1, currentPosition.Uniq_Y + 1);


                if (enemy == currentDirection)
                    return;

                currentDistance = enemy.DistanceBetween(currentDirection);

                if (currentDistance < minDistance)
                {
                    nearestPoint = currentDirection;
                    minDistance = currentDistance;
                }
            }
            // left
            if ((currentPosition.Uniq_X - 1 >= 0 && currentPosition.Uniq_X - 1 < Console.WindowWidth &&
                currentPosition.Uniq_Y >= 0 && currentPosition.Uniq_Y < Console.WindowHeight &&
                !ContainsBattleground(currentPosition.Uniq_X - 1, currentPosition.Uniq_Y) &&
                !ContainsObstacle(currentPosition.Uniq_X - 1, currentPosition.Uniq_Y) &&
                !ContainsMove(currentPosition.Uniq_X - 1, currentPosition.Uniq_Y)) ||
                new Position(currentPosition.Uniq_X - 1, currentPosition.Uniq_Y) == enemy)
            {
                currentDirection = new Position(currentPosition.Uniq_X - 1, currentPosition.Uniq_Y);


                if (enemy == currentDirection)
                    return;

                currentDistance = enemy.DistanceBetween(currentDirection);

                if (currentDistance < minDistance)
                {
                    nearestPoint = currentDirection;
                    minDistance = currentDistance;
                }
            }

            // top-left
            if ((currentPosition.Uniq_X - 1 >= 0 && currentPosition.Uniq_X - 1 < Console.WindowWidth &&
                currentPosition.Uniq_Y - 1 >= 0 && currentPosition.Uniq_Y - 1 < Console.WindowHeight &&
                !ContainsBattleground(currentPosition.Uniq_X - 1, currentPosition.Uniq_Y - 1) &&
                !ContainsObstacle(currentPosition.Uniq_X - 1, currentPosition.Uniq_Y - 1) &&
                !ContainsMove(currentPosition.Uniq_X - 1, currentPosition.Uniq_Y - 1)) ||
                new Position(currentPosition.Uniq_X - 1, currentPosition.Uniq_Y - 1) == enemy)
            {
                currentDirection = new Position(currentPosition.Uniq_X - 1, currentPosition.Uniq_Y - 1);

                if (enemy == currentDirection)
                    return;

                currentDistance = enemy.DistanceBetween(currentDirection);

                if (currentDistance < minDistance)
                {
                    nearestPoint = currentDirection;
                    minDistance = currentDistance;
                }
            }
            
            moveCommands.Add(nearestPoint);
            FindBfsPath(enemy, nearestPoint);
        }

        private int GetBattlefieldIndex(int x, int y)
        {
            for (int i = 0; i < this.Variables.Battleground.Count; i++)
            {
                var currentBattlefield = this.Variables.Battleground[i];

                if (currentBattlefield.Uniq_X == x && currentBattlefield.Uniq_Y == y)
                    return i;
            }

            throw new ArgumentNullException("While searching for the battlefield index there was none found");
        }

        private void SetHellionRoad()
        {
            /* find the speed with which the enemy is moving -> done 
             * find the index of the target position -> done
             * find from where is the battleground  
             * find the road to the nearest position near -> done
             * move the hellion
             * 
             * TODO WHEN THE HELLION is not moving, but the target moves, the hellion must move immediately
            */

            if (this.Target == null || this.Target.GetHealthStatus() <= 0)
            {
                FindTarget();
                return;
            }

            Position hellionPosition = new Position(this.Uniq_X, this.Uniq_Y);
            Position targetPosition = new Position(this.Target.Uniq_X, this.Target.Uniq_Y);
            int indexTargetPosition = GetBattlefieldIndex(targetPosition.Uniq_X, targetPosition.Uniq_Y);
            int hellionIndexTargetPosition = indexTargetPosition;
            var hellionTargetPosition = targetPosition;
            int fromIndex = indexTargetPosition;
            int toIndex = internal_Variables.Battleground.Count - 1;
            
            /* for m_MoveRate the hellion travels from one block to another. Hence, the time it will take for it to travel the whole
             * distance will be roadCosts[targetPosition] (the sum of all blocks it will transit through) 
             * -1 because we want to go the the adjacent block not the block from which the target will move
             */

            // find the shortest path to the target with BFS

            float hellionTakenTime = m_MoveRate * (moveCommands.Count);
            // bfsCommands.Count = hellionTakenTime / m_MoveRate

            // this is the target calculated distance taken for the time the hellion moves
            int blocksCircled = (int)Math.Floor(hellionTakenTime / this.Target.MoveRate);
            var currentTime = DateTime.Now;

            List<double> timeTaken = new List<double>();

            while (true)
            {
                if (this.Target == null || this.Target.GetHealthStatus() <= 0)
                    return;

                hellionIndexTargetPosition = (int)Math.Ceiling((fromIndex + toIndex) / 2.0);
                if (hellionIndexTargetPosition == internal_Variables.Battleground.Count)
                    hellionIndexTargetPosition--;

                hellionTargetPosition = internal_Variables.Battleground[hellionIndexTargetPosition];
                var takenTime = (DateTime.Now - currentTime).TotalSeconds;
                timeTaken.Add(takenTime);

                ClearPathData();
                FindBfsPath(new Position(hellionTargetPosition.Uniq_X, hellionTargetPosition.Uniq_Y), 
                    new Position(this.Uniq_X, this.Uniq_Y));

                hellionTakenTime = (m_MoveRate + 0.04f) * (moveCommands.Count);
                blocksCircled = (int)Math.Floor(hellionTakenTime / (this.Target.MoveRate))+1;
                
                if (fromIndex == hellionIndexTargetPosition || toIndex == hellionIndexTargetPosition)
                    break;

                if ((
                    (blocksCircled + indexTargetPosition > hellionIndexTargetPosition - 2) ||
                    (blocksCircled + indexTargetPosition > hellionIndexTargetPosition - 1) ||
                    (blocksCircled + indexTargetPosition > hellionIndexTargetPosition)) &&
                    hellionIndexTargetPosition + 1 < internal_Variables.Battleground.Count)
                    fromIndex = hellionIndexTargetPosition;

                else if (hellionIndexTargetPosition + 1 < internal_Variables.Battleground.Count)
                    toIndex = hellionIndexTargetPosition;

                else
                    break;
            }

            /* if the hellionIndexTargetPosition == internal_Variables.BattleGround.Count 
             * then the target should find a new target, if the new target emerges with the same problem, then should traverse all targets
             * before this one if all give the equivalent situation, then the hellion is useless and it should be done something to it
             */
            if (hellionIndexTargetPosition == internal_Variables.Battleground.Count)
            {
                FindTarget();
                SetHellionRoad();
                return;
            }

            /* take the last position of the road
             * when the target moves, try to move with it, if there is a adjacent obstacle to the hellion, a new road must be planned
             */
        }

        private int FindMinElementIndex<T>(List<T> list) where T :System.IComparable<T>
        {
            T curMin = list[0];
            int minIndex = 0;

            for(int i = 0; i < list.Count; i++)
            {
                T curElement = list[i];

                if (curMin.CompareTo(curElement) > 0)
                {
                    curMin = curElement;
                    minIndex = i;
                }
            }

            return minIndex;
        }

        private bool ContainsMove(int x, int y)
        {
            foreach(var move in this.moveCommands)
            {
                if (move.Uniq_X == x && move.Uniq_Y == y)
                    return true;
            }

            return false;
        }

        private bool ContainsTurret(int x, int y)
        {
            foreach(var turret in internal_Variables.TurretsPosition)
            {
                if(turret.Uniq_X == x && turret.Uniq_Y == y)
                    return true;
            }

            return false;
        }

        private bool ContainsBattleground(int x, int y)
        {
            foreach (var field in internal_Variables.Battleground)
            {
                if (field.Uniq_X == x && field.Uniq_Y == y)
                    return true;
            }
            return false;
        }

        private bool ContainsObstacle(int x, int y)
        {
            foreach (var obstacles in internal_Variables.ObstaclePositions)
            {
                if (obstacles.Uniq_X == x && obstacles.Uniq_Y == y)
                    return true;
            }
            return false;
        }

        private void ClearPathData()
        {
            moveCommands.Clear();
            currentCommands = 0;
        }

        public void Move()
        {
            DateTime currentTime = DateTime.Now;

            if (moveCommands.Count == currentCommands || moveCommands.Count == 0)
            {
                // clear the path
                ClearPathData();

                if (this.Target == null || (this.Target != null && this.Target.GetHealthStatus() <= 0))
                {
                    this.Target = null;
                    FindTarget();
                }
                
                SetHellionRoad();
                return;
            }

            if ((currentTime - m_timeDeparture).TotalSeconds < m_MoveRate || moveCommands.Count == currentCommands
                || moveCommands.Count == 0)
                return;

            m_timeDeparture = currentTime;

            if (this.Target == null || this.Target.GetHealthStatus() <= 0)
            {
                if(initialTargetPosition != null) // this is not the first time to search for a new target
                    this.ClearPathData();

                if (this.Target != null && this.Target.GetHealthStatus() <= 0)
                    this.Target = null;
                
                this.FindTarget();
                SetHellionRoad();
                return;
            }
            Console.SetCursorPosition(this.Uniq_X, this.Uniq_Y);
            Console.Write(" ");
            
            Uniq_X = moveCommands[currentCommands].Uniq_X;
            Uniq_Y = moveCommands[currentCommands].Uniq_Y;

            currentCommands++;

            // check if the new position does not overlap with a turret

            Console.SetCursorPosition(this.Uniq_X, this.Uniq_Y);
            Console.Write("&");
        }

        protected override void SetStats()
        {
            this.Ammo = 1000000;
            this.FireRate = 0.1f;

            if (this.LevelDamage == 1)
            {
                this.Damage = 20;
            }

            else if (this.LevelDamage == 2)
            {
                this.Damage = SetUpVariables.Level_2_FireBunker_Damage;
                this.FireEffect = 5;
            }

            else if (this.LevelDamage == 3)
            {
                this.Damage = SetUpVariables.Level_3_FireBunker_Damage;
                this.FireEffect = 5;
            }
        }
    }
}
