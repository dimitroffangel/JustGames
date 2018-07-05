using System;
using System.Collections.Generic;

namespace TowerDefence.Objects.Turrets.FlameTurrets
{
    class Hellion : FlameThrower
    {
        private SetUpVariables internal_Variables;
        private DateTime m_timeDeparture;
        public const float m_MoveRate = 0.4f;

        private Dictionary<Position, int> roadsCosts;
        private Dictionary<Position, bool> map;
        private Queue<Position> unvisitedNodes;
        private List<Position> moveCommands;
        private Position initialTargetPosition;
        private int currentCommand;
        

        public Hellion(int x, int y, TurretType turretType, TurretPlacement placement, 
            ref SetUpVariables variables) : base(x, y, turretType, placement, ref variables)
        {
            internal_Variables = variables;
            m_timeDeparture = DateTime.Now;

            roadsCosts = new Dictionary<Position, int>(new PositionEqualityComparer());
            map = new Dictionary<Position, bool>(new PositionEqualityComparer());
            unvisitedNodes = new Queue<Position>();
            moveCommands = new List<Position>();
            currentCommand = 0;
        }

        private void FindTarget()
        {
            float minDistance = int.MaxValue;
            int minIndex = -1;
            int index = -1;

            foreach (var enemy in internal_Variables.EnemyPositions)
            {
                index++;
                float curDistance = this.DistanceBetween(enemy);

                if (minDistance > curDistance)
                {
                    minIndex = index;
                    minDistance = curDistance;
                }
            }

            this.Target = internal_Variables.EnemyPositions[minIndex];
            this.initialTargetPosition = new Position(this.Target.Uniq_X, this.Target.Uniq_Y);

            // find the moving direction of the target
        }

        
        // check every position that surrounds the current position 
        // order them from closest to the target to farthest

        // FindShortestPath() to target
        // calculate the speed with which the unit will reach the target or consecutive positions near it
        // on each new fallen rock the FindShortestPath must be reevaluated 

        private void InitialShortPath()
        {
            /* set all passable nodes to unvisited
             * each distance to other nodes apart from the hellion position is inifinite
             */

            // add all positions to the list
            for(int x = 0; x < Console.WindowWidth; x++)
            {
                for(int y = 0; y < Console.WindowHeight; y++)
                {
                    map.Add(new Position(x,y), false);
                    roadsCosts.Add(new Position(x, y), int.MaxValue);
                }
            }

            roadsCosts[new Position(Uniq_X, Uniq_Y)] = 0;
            map[new Position(Uniq_X, Uniq_Y)] = true;

            // start from the hellion position
            roadsCosts[new Position(Uniq_X, Uniq_Y - 1)] = 1; // top
            roadsCosts[new Position(Uniq_X + 1, Uniq_Y - 1)] = 1; // top-right
            roadsCosts[new Position(Uniq_X + 1, Uniq_Y)] = 1; // right
            roadsCosts[new Position(Uniq_X + 1, Uniq_Y + 1)] = 1; // bot-right
            roadsCosts[new Position(Uniq_X, Uniq_Y + 1)] = 1; // bot
            roadsCosts[new Position(Uniq_X - 1, Uniq_Y + 1)] = 1; // bot-left
            roadsCosts[new Position(Uniq_X - 1, Uniq_Y)] = 1; // left
            roadsCosts[new Position(Uniq_X - 1, Uniq_Y - 1)] = 1; // top-left

            map[new Position(Uniq_X, Uniq_Y - 1)] = true; // top
            map[new Position(Uniq_X + 1, Uniq_Y - 1)] = true; // top-right
            map[new Position(Uniq_X + 1, Uniq_Y)] = true; // right
            map[new Position(Uniq_X + 1, Uniq_Y + 1)] = true; // bot-right
            map[new Position(Uniq_X, Uniq_Y + 1)] = true; // bot
            map[new Position(Uniq_X - 1, Uniq_Y + 1)] = true; // bot-left
            map[new Position(Uniq_X - 1, Uniq_Y)] = true; // left
            map[new Position(Uniq_X - 1, Uniq_Y - 1)] = true; // top-left

            unvisitedNodes.Enqueue(new Position(Uniq_X, Uniq_Y - 1)); // top
            unvisitedNodes.Enqueue(new Position(Uniq_X + 1, Uniq_Y - 1)); // top-right
            unvisitedNodes.Enqueue(new Position(Uniq_X + 1, Uniq_Y)); // right
            unvisitedNodes.Enqueue(new Position(Uniq_X + 1, Uniq_Y + 1)); // bot-right 
            unvisitedNodes.Enqueue(new Position(Uniq_X, Uniq_Y + 1)); // bot
            unvisitedNodes.Enqueue(new Position(Uniq_X - 1, Uniq_Y + 1)); // bot-left
            unvisitedNodes.Enqueue(new Position(Uniq_X - 1, Uniq_Y)); // left
            unvisitedNodes.Enqueue(new Position(Uniq_X - 1, Uniq_Y - 1)); // top-left
        }

        private void FindShortestPath()
        {
            while (unvisitedNodes.Count > 0)
            {
                var firstNode = unvisitedNodes.Dequeue();
                int firstNodeCost = roadsCosts[new Position(firstNode.Uniq_X, firstNode.Uniq_Y)] + 1;

                if(ContainsTurret(firstNode.Uniq_X, firstNode.Uniq_Y) || ContainsObstacle(firstNode.Uniq_X, firstNode.Uniq_Y) || 
                    ContainsBattleground(firstNode.Uniq_X, firstNode.Uniq_Y))
                {
                    roadsCosts[firstNode] = int.MaxValue;
                    continue;
                }

                // start from the hellion position
                if (map.ContainsKey(new Position(firstNode.Uniq_X, firstNode.Uniq_Y - 1)))
                {
                    if (!map[new Position(firstNode.Uniq_X, firstNode.Uniq_Y - 1)])
                    {
                        roadsCosts[new Position(firstNode.Uniq_X, firstNode.Uniq_Y - 1)] = firstNodeCost; // top
                        unvisitedNodes.Enqueue(new Position(firstNode.Uniq_X, firstNode.Uniq_Y - 1)); // top
                        map[new Position(firstNode.Uniq_X, firstNode.Uniq_Y - 1)] = true;
                    }
                    else if (roadsCosts[new Position(firstNode.Uniq_X, firstNode.Uniq_Y - 1)] > firstNodeCost)
                        roadsCosts[new Position(firstNode.Uniq_X, firstNode.Uniq_Y - 1)] = firstNodeCost; // top
                }
                if (map.ContainsKey(new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y - 1)))
                {
                    if (!map[new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y - 1)])    // top-right
                    {
                        roadsCosts[new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y - 1)] = firstNodeCost;
                        unvisitedNodes.Enqueue(new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y - 1)); // top-right
                        map[new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y - 1)] = true;
                    }
                    else if (roadsCosts[new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y - 1)] > firstNodeCost)
                        roadsCosts[new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y - 1)] = firstNodeCost;
                }

                if (map.ContainsKey(new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y)))
                {
                    if (!map[new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y)]) // right
                    {
                        roadsCosts[new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y)] = firstNodeCost; // right
                        unvisitedNodes.Enqueue(new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y)); // right
                        map[new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y)] = true;
                    }
                    else if (roadsCosts[new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y)] > firstNodeCost)
                        roadsCosts[new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y)] = firstNodeCost;
                }

                if (map.ContainsKey(new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y + 1)))
                {
                    if (!map[new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y + 1)])
                    {
                        roadsCosts[new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y + 1)] = firstNodeCost; // bot-right
                        unvisitedNodes.Enqueue(new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y + 1)); // bot-right 
                        map[new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y + 1)] = true;
                    }
                    else if (roadsCosts[new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y + 1)] > firstNodeCost)
                        roadsCosts[new Position(firstNode.Uniq_X + 1, firstNode.Uniq_Y + 1)] = firstNodeCost;
                }
                if (map.ContainsKey(new Position(firstNode.Uniq_X, firstNode.Uniq_Y + 1)))
                {
                    if (!map[new Position(firstNode.Uniq_X, firstNode.Uniq_Y + 1)])
                    {
                        roadsCosts[new Position(firstNode.Uniq_X, firstNode.Uniq_Y + 1)] = firstNodeCost; // bot
                        unvisitedNodes.Enqueue(new Position(firstNode.Uniq_X, firstNode.Uniq_Y + 1)); // bot
                        map[new Position(firstNode.Uniq_X, firstNode.Uniq_Y + 1)] = true;
                    }
                    else if (roadsCosts[new Position(firstNode.Uniq_X, firstNode.Uniq_Y + 1)] > firstNodeCost)
                        roadsCosts[new Position(firstNode.Uniq_X, firstNode.Uniq_Y + 1)] = firstNodeCost;
                }

                if (map.ContainsKey(new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y + 1)))
                {
                    if (!map[new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y + 1)])
                    {
                        roadsCosts[new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y + 1)] = firstNodeCost; // bot-left
                        unvisitedNodes.Enqueue(new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y + 1)); // bot-left
                        map[new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y + 1)] = true;
                    }
                    else if (roadsCosts[new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y + 1)] > firstNodeCost)
                        roadsCosts[new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y + 1)] = firstNodeCost;
                }

                if (map.ContainsKey(new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y)))
                {
                    if (!map[new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y)])
                    {
                        roadsCosts[new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y)] = firstNodeCost; // left
                        unvisitedNodes.Enqueue(new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y)); // left
                        map[new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y)] = true;
                    }
                    else if (roadsCosts[new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y)] > firstNodeCost)
                        roadsCosts[new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y)] = firstNodeCost;
                }

                if (map.ContainsKey(new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y - 1)))
                {
                    if (!map[new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y - 1)])
                    {
                        roadsCosts[new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y - 1)] = firstNodeCost; // top-left
                        unvisitedNodes.Enqueue(new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y - 1)); // top-left
                        map[new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y - 1)] = true;
                    }
                    else if (roadsCosts[new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y - 1)] > firstNodeCost)
                        roadsCosts[new Position(firstNode.Uniq_X - 1, firstNode.Uniq_Y - 1)] = firstNodeCost;
                }
            }
        }

        private int GetBattlefieldIndex(int x, int y)
        {
            for (int i = 0; i < this.Variables.Battleground.Count; i++)
            {
                var currentBattlefield = this.Variables.Battleground[i];

                if (currentBattlefield.Uniq_X == x && currentBattlefield.Uniq_Y == y)
                    return i + 1;
            }

            throw new ArgumentNullException("Searching for the battlefield index there was none found");
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
            if (this.Target == null)
                return;

            Position hellionPosition = new Position(this.Uniq_X, this.Uniq_Y);
            Position targetPosition = new Position(this.Target.Uniq_X, this.Target.Uniq_Y);
            int indexTargetPosition = GetBattlefieldIndex(targetPosition.Uniq_X, targetPosition.Uniq_Y);
            int hellionIndexTargetPosition = indexTargetPosition;
            var hellionTargetPosition = targetPosition;
            
            /* for m_MoveRate the hellion travels from one block to another. Hence, the time it will take for it to travel the whole
             * distance will be roadCosts[targetPosition] (the sum of blocks it will transit through) 
             * -1 because we want to go the the adjacent block not the block from which the target will move
             */

            float hellionTakenTime = m_MoveRate * (roadsCosts[hellionTargetPosition]);
            // roadCosts[hellionTargetPosition] - 1 = hellionTakenTime / m_MoveRate

            // this is the target calculated distance taken for the time the hellion moves
            int blocksCircled = (int)Math.Floor(hellionTakenTime / this.Target.MoveRate);
            
            while(((blocksCircled + indexTargetPosition > hellionIndexTargetPosition - 3)|| (blocksCircled + indexTargetPosition > hellionIndexTargetPosition - 2)||
               (blocksCircled + indexTargetPosition > hellionIndexTargetPosition - 1)  || (blocksCircled + indexTargetPosition > hellionIndexTargetPosition)) &&
                hellionIndexTargetPosition+ 1 < internal_Variables.Battleground.Count)
            {
                hellionIndexTargetPosition++;
                hellionTargetPosition = internal_Variables.Battleground[hellionIndexTargetPosition];

                hellionTakenTime = m_MoveRate * (roadsCosts[hellionTargetPosition]);

                blocksCircled = (int)Math.Floor(hellionTakenTime / this.Target.MoveRate);
            }

            hellionTargetPosition = internal_Variables.Battleground[hellionIndexTargetPosition];
            ExhibitRoadTaken(hellionTargetPosition);
            hellionTargetPosition = moveCommands[0];

            /* take the last position of the road
             * when the target moves, try to move with it, if there is a adjacent obstacle to the hellion, a new road must be planned
             */
        }

        private int FindMinElement(List<int> list)
        {
            int curMin = list[0];
            int minIndex = 0;

            for(int i = 0; i < list.Count; i++)
            {
                int curElement = list[i];

                if (curMin > curElement)
                {
                    curMin = curElement;
                    minIndex = i;
                }
            }

            return minIndex;
        }

        private void ExhibitRoadTaken(Position endPosition)
        {
            Position curPosition = endPosition;
            Position hellionPosition = new Position(Uniq_X, Uniq_Y);
            List<int> surroundingCosts = new List<int>();
            List<Position> surroundingPositions = new List<Position>();
            int surroundingPositionsCount = 0;

            while(curPosition != hellionPosition)
            {
                surroundingCosts.Clear();
                surroundingPositions.Clear();
                surroundingPositionsCount = 0;

                if (map.ContainsKey(new Position(curPosition.Uniq_X, curPosition.Uniq_Y - 1)))
                {
                    surroundingPositions.Add(new Position(curPosition.Uniq_X, curPosition.Uniq_Y - 1)); // top
                    surroundingPositionsCount++;
                    surroundingCosts.Add(roadsCosts[surroundingPositions[surroundingPositionsCount-1]]);

                }
                if (map.ContainsKey(new Position(curPosition.Uniq_X + 1, curPosition.Uniq_Y - 1)))
                {
                    surroundingPositions.Add(new Position(curPosition.Uniq_X + 1, curPosition.Uniq_Y - 1)); // top-right
                    surroundingPositionsCount++;
                    surroundingCosts.Add(roadsCosts[surroundingPositions[surroundingPositionsCount - 1]]);
                }
                if (map.ContainsKey(new Position(curPosition.Uniq_X + 1, curPosition.Uniq_Y)))
                {
                    surroundingPositions.Add(new Position(curPosition.Uniq_X + 1, curPosition.Uniq_Y)); // right
                    surroundingPositionsCount++;
                    surroundingCosts.Add(roadsCosts[surroundingPositions[surroundingPositionsCount - 1]]);
                }
                if (map.ContainsKey(new Position(curPosition.Uniq_X + 1, curPosition.Uniq_Y + 1)))
                {
                    surroundingPositions.Add(new Position(curPosition.Uniq_X + 1, curPosition.Uniq_Y + 1)); // bot-right
                    surroundingPositionsCount++;
                    surroundingCosts.Add(roadsCosts[surroundingPositions[surroundingPositionsCount - 1]]);
                }
                if (map.ContainsKey(new Position(curPosition.Uniq_X, curPosition.Uniq_Y + 1)))
                {
                    surroundingPositions.Add(new Position(curPosition.Uniq_X, curPosition.Uniq_Y + 1)); // bot
                    surroundingPositionsCount++;
                    surroundingCosts.Add(roadsCosts[surroundingPositions[surroundingPositionsCount - 1]]);
                }
                if (map.ContainsKey(new Position(curPosition.Uniq_X - 1, curPosition.Uniq_Y + 1)))
                {
                    surroundingPositions.Add(new Position(curPosition.Uniq_X - 1, curPosition.Uniq_Y + 1)); // bot-left
                    surroundingPositionsCount++;
                    surroundingCosts.Add(roadsCosts[surroundingPositions[surroundingPositionsCount - 1]]);
                }
                if (map.ContainsKey(new Position(curPosition.Uniq_X - 1, curPosition.Uniq_Y)))
                {
                    surroundingPositions.Add(new Position(curPosition.Uniq_X - 1, curPosition.Uniq_Y)); // left
                    surroundingPositionsCount++;
                    surroundingCosts.Add(roadsCosts[surroundingPositions[surroundingPositionsCount - 1]]);
                }
                if (map.ContainsKey(new Position(curPosition.Uniq_X - 1, curPosition.Uniq_Y - 1)))
                {
                    surroundingPositions.Add(new Position(curPosition.Uniq_X - 1, curPosition.Uniq_Y - 1)); // top-left
                    surroundingPositionsCount++;
                    surroundingCosts.Add(roadsCosts[surroundingPositions[surroundingPositionsCount - 1]]);
                }

                curPosition = surroundingPositions[FindMinElement(surroundingCosts)];
                moveCommands.Add(curPosition);
            }
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
            roadsCosts.Clear();
            map.Clear();
            unvisitedNodes.Clear();
            moveCommands.Clear();
            currentCommand = 0;
        }

        public void Move()
        {
            DateTime currentTime = DateTime.Now;

            if (moveCommands.Count - currentCommand - 1 == 0)
            {
                // clear the path
                ClearPathData();
                InitialShortPath();
                FindShortestPath();
                SetHellionRoad();
                return;
            }

            if ((currentTime - m_timeDeparture).Seconds < m_MoveRate || moveCommands.Count -currentCommand-1 == 0)
                return;

            m_timeDeparture = currentTime;

            if (this.Target == null)
            {
                if(initialTargetPosition != null) // this is not the first time to search for a new target
                    this.ClearPathData();

                InitialShortPath();
                FindShortestPath();
                this.FindTarget();
                SetHellionRoad();
                return;
            }
            Console.SetCursorPosition(this.Uniq_X, this.Uniq_Y);
            Console.Write(" ");
            currentCommand++;

            Uniq_X = moveCommands[moveCommands.Count - 1 - currentCommand].Uniq_X;
            Uniq_Y = moveCommands[moveCommands.Count - 1 - currentCommand].Uniq_Y;

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
