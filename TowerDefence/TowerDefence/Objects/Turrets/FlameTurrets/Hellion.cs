using System;
using System.Collections.Generic;

namespace TowerDefence.Objects.Turrets.FlameTurrets
{
    class Hellion : FlameThrower
    {
        private SetUpVariables internal_Variables;
        private DateTime m_timeDeparture;
        public const float m_MoveRate = 0.5f;

        private Dictionary<Position, int> roadsCosts;
        private Dictionary<Position, bool> map;
        private Queue<Position> unvisitedNodes;
        private List<Position> moveCommands;

        public Hellion(int x, int y, TurretType turretType, TurretPlacement placement, 
            ref SetUpVariables variables) : base(x, y, turretType, placement, ref variables)
        {
            internal_Variables = variables;
            m_timeDeparture = DateTime.Now;

            roadsCosts = new Dictionary<Position, int>(new PositionEqualityComparer());
            map = new Dictionary<Position, bool>(new PositionEqualityComparer());
            unvisitedNodes = new Queue<Position>();
            moveCommands = new List<Position>();
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
                    ContainsEnemy(firstNode.Uniq_X, firstNode.Uniq_Y))
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

            var targetPosition = new Position(this.Target.Uniq_X, this.Target.Uniq_Y);
            var indexTargetPosition = GetBattlefieldIndex(targetPosition.Uniq_X, targetPosition.Uniq_Y);
            var hellionIndexTargetPosition = indexTargetPosition;
            var hellionTargetPosition = internal_Variables.Battleground[hellionIndexTargetPosition];

            var distanceFromTarget = roadsCosts[targetPosition]- 1;

            var timeToReachDistance = distanceFromTarget / m_MoveRate;
            int targetDistanceTraveled = (int)(timeToReachDistance * this.Target.MoveRate);
            
            while(targetDistanceTraveled + indexTargetPosition > hellionIndexTargetPosition)
            {
                hellionIndexTargetPosition++;
                hellionTargetPosition = internal_Variables.Battleground[hellionIndexTargetPosition];

                distanceFromTarget = roadsCosts[hellionTargetPosition] - 1;
                timeToReachDistance = distanceFromTarget / m_MoveRate;
                targetDistanceTraveled = (int)(timeToReachDistance * this.Target.MoveRate);
            }

            hellionTargetPosition = internal_Variables.Battleground[hellionIndexTargetPosition];
            ExhibitRoadTaken(hellionTargetPosition);
            hellionTargetPosition = moveCommands[1];

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

            while(curPosition != hellionPosition)
            {
                surroundingCosts.Clear();
                surroundingPositions.Clear();

                surroundingPositions.Add(new Position(curPosition.Uniq_X, curPosition.Uniq_Y - 1)); // top
                surroundingPositions.Add(new Position(curPosition.Uniq_X+1, curPosition.Uniq_Y - 1)); // top-right
                surroundingPositions.Add(new Position(curPosition.Uniq_X+1, curPosition.Uniq_Y)); // right
                surroundingPositions.Add(new Position(curPosition.Uniq_X+1, curPosition.Uniq_Y + 1)); // bot-right
                surroundingPositions.Add(new Position(curPosition.Uniq_X, curPosition.Uniq_Y + 1)); // bot
                surroundingPositions.Add(new Position(curPosition.Uniq_X-1, curPosition.Uniq_Y + 1)); // bot-left
                surroundingPositions.Add(new Position(curPosition.Uniq_X-1, curPosition.Uniq_Y)); // left
                surroundingPositions.Add(new Position(curPosition.Uniq_X-1, curPosition.Uniq_Y -1)); // top-left

                surroundingCosts.Add(roadsCosts[surroundingPositions[0]]);
                surroundingCosts.Add(roadsCosts[surroundingPositions[1]]);
                surroundingCosts.Add(roadsCosts[surroundingPositions[2]]);
                surroundingCosts.Add(roadsCosts[surroundingPositions[3]]);
                surroundingCosts.Add(roadsCosts[surroundingPositions[4]]);
                surroundingCosts.Add(roadsCosts[surroundingPositions[5]]);
                surroundingCosts.Add(roadsCosts[surroundingPositions[6]]);
                surroundingCosts.Add(roadsCosts[surroundingPositions[7]]);

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

        private bool ContainsEnemy(int x, int y)
        {
            foreach (var enemy in internal_Variables.EnemyPositions)
            {
                if (enemy.Uniq_X == x && enemy.Uniq_Y == y)
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

        public void Move()
        {
            DateTime currentTime = DateTime.Now;

            if ((currentTime - m_timeDeparture).Seconds < m_MoveRate)
                return;

            m_timeDeparture = currentTime;

            if (this.Target == null)
            {
                InitialShortPath();
                FindShortestPath();
                this.FindTarget();
                SetHellionRoad();
            }

            int initialX = this.Uniq_X;
            int initialY = this.Uniq_Y;

            Console.SetCursorPosition(this.Uniq_X, this.Uniq_Y);
            Console.Write(" ");

            if (this.Uniq_X > this.Target.Uniq_X + 1)
                initialX--;
            else if (this.Uniq_X < this.Target.Uniq_X - 1)
                initialX++;

            if (this.Uniq_Y > this.Target.Uniq_Y + 1)
                initialY--;
            else if (this.Uniq_Y < this.Target.Uniq_Y - 1)
                initialY++;

            // check if the new position does not overlap with a turret
            // if it does TODOO
            if(ContainsTurret(initialX, initialY) || ContainsObstacle(initialX, initialY) || ContainsEnemy(initialX, initialY))
            {
                //  move x towards it
                initialY = this.Uniq_Y;
                
                if(ContainsTurret(initialX, initialY) || ContainsObstacle(initialX, initialY) || ContainsEnemy(initialX, initialY))
                {
                    // move y towards it
                    initialX = this.Uniq_X;
                    if (this.Uniq_Y > this.Target.Uniq_Y + 1)
                        initialY--;
                    else if (this.Uniq_Y < this.Target.Uniq_Y - 1)
                        initialY++;
                    
                    if(ContainsTurret(initialX, initialY) || ContainsObstacle(initialX, initialY) || ContainsEnemy(initialX, initialY))
                    {
                        // move x towards it, but y reversed towards it
                        initialX = this.Uniq_X;
                        initialY = this.Uniq_Y;

                        if (this.Uniq_X > this.Target.Uniq_X + 1)
                            initialX--;
                        else if (this.Uniq_X < this.Target.Uniq_X - 1)
                            initialX++;

                        if (this.Uniq_Y > this.Target.Uniq_Y + 1)
                            initialY++;
                        else if (this.Uniq_Y < this.Target.Uniq_Y - 1)
                            initialY--;

                        if(ContainsTurret(initialX, initialY) || ContainsObstacle(initialX, initialY) || ContainsEnemy(initialX, initialY))
                        {
                            // move y towards it, but x reversed towards it
                            initialX = this.Uniq_X;
                            initialY = this.Uniq_Y;

                            if (this.Uniq_X > this.Target.Uniq_X + 1)
                                initialX++;
                            else if (this.Uniq_X < this.Target.Uniq_X - 1)
                                initialX--;

                            if (this.Uniq_Y > this.Target.Uniq_Y + 1)
                                initialY--;
                            else if (this.Uniq_Y < this.Target.Uniq_Y - 1)
                                initialY++;

                            if (ContainsTurret(initialX, initialY) || ContainsObstacle(initialX, initialY) || ContainsEnemy(initialX, initialY))
                            {
                                // move both reversed towards it
                                initialX = this.Uniq_X;
                                initialY = this.Uniq_Y;

                                if (this.Uniq_X > this.Target.Uniq_X + 1)
                                    initialX++;
                                else if (this.Uniq_X < this.Target.Uniq_X - 1)
                                    initialX--;

                                if (this.Uniq_Y > this.Target.Uniq_Y + 1)
                                    initialY++;
                                else if (this.Uniq_Y < this.Target.Uniq_Y - 1)
                                    initialY--;

                            }
                        }
                    }
                }
            }

            this.Uniq_X = initialX;
            this.Uniq_Y = initialY;

            Console.SetCursorPosition(this.Uniq_X, this.Uniq_Y);
            Console.Write("&");
        }

        protected override void SetStats()
        {
            this.Ammo = 30;
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
