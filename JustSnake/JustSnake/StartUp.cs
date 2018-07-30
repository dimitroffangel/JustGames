using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JustSnake
{
    class StartUp
    {
        struct Position
        {
           public int row;
           public int col;

           public Position(int row, int col)
           {
               this.row = row;
               this.col = col;
           }
        }

        static int FoodsEaten = 0;
        static double TravelSpeed = 100;
        static int LastFoodTime = Environment.TickCount;
        static int LastSpecialItem = Environment.TickCount;
        static bool IsFlexible = false;
        static Queue<Position> SnakeElements = new Queue<Position>();
        static Position SnakeHead;
        static Position NextDirection;
        static Position NewSnakeHead;

        static Position[] Directions = new Position[]
        {
                new Position(1, 0), //right
                new Position(-1, 0), //left
                new Position(0, -1), //up
                new Position(0, 1) //down
        };

        static int Direction = Right;
        static Random NextRandomPosition = new Random();

        static List<int> Primes = new List<int>()
            {
                11,13,17,19, // current obstacle positions
            };
        static List<Position> Obstacles = new List<Position>()
            {
                    new Position(NextRandomPosition.Next(0, Console.WindowWidth),
                                 NextRandomPosition.Next(0, Console.WindowHeight)),
                    new Position(NextRandomPosition.Next(0, Console.WindowWidth),
                                 NextRandomPosition.Next(0, Console.WindowHeight)),
                    new Position(NextRandomPosition.Next(0, Console.WindowWidth),
                                 NextRandomPosition.Next(0, Console.WindowHeight)),
                    new Position(NextRandomPosition.Next(0, Console.WindowWidth),
                                 NextRandomPosition.Next(0, Console.WindowHeight)),
                    new Position(NextRandomPosition.Next(0, Console.WindowWidth),
                                 NextRandomPosition.Next(0, Console.WindowHeight))
            };
        static char SpecialFoodChar = '#'; // each symbol will be randomly chosen
        static List<char> SpecialFoodsChars = new List<char>()
            {
                '#', // mashroom that takes 1/4 of your tail
                '$', // speed
                '&', // allows to move through walls
                '~', // your tail becomes your head
                'T' // slow time
            };
        static Position Food = new Position();
        static Position SpecialFood = new Position();

        #region constValues
        const byte Right = 0;
        const byte Left = 1;
        const byte Up = 2;
        const byte Down = 3;
        const int FoodDisappearTime = 8000;
        const int SpecialDissapearTime = 8000;
        const float IncreaseSteadySpeed = 0.1f;
        const float IncreaseSpeed = 0.4f;
        #endregion

        static void DrawFigure(int x, int y, char symbol)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(symbol);
        }

        static void CreateSnake()
        {
            for (int i = 0; i < 5; i++) // initialize the snake
                SnakeElements.Enqueue(new Position(i, 0));

            foreach (var particle in SnakeElements) // draw the snake
                DrawFigure(particle.row, particle.col, '*');

            TryUpdatingSnakeHead();
        }

        static void DrawTail()
        {
            DrawFigure(SnakeHead.row, SnakeHead.col, '*'); // replace the head with a body
        }

        static void DrawHead()
        {
            if (NewSnakeHead.row >= 0 && NewSnakeHead.row < Console.WindowWidth &&
                NewSnakeHead.col >= 0 && NewSnakeHead.col < Console.WindowHeight)
            {
                SnakeElements.Enqueue(NewSnakeHead); // and set the new head
                Console.SetCursorPosition(NewSnakeHead.row, NewSnakeHead.col);
            }

            TravelSpeed -= IncreaseSteadySpeed;

            if (Direction == Left)
                Console.Write("<");
            if (Direction == Right)
                Console.Write(">");
            if (Direction == Up)
                Console.Write("^");
            if (Direction == Down)
                Console.Write("v");
        }

        static void TryUpdatingSnakeHead()
        {
            SnakeHead = SnakeElements.Last();
            NextDirection = Directions[Direction];
            NewSnakeHead = new Position(SnakeHead.row + NextDirection.row,
                                        SnakeHead.col + NextDirection.col);
        }

        #region EnviromentCollision
        static bool HasSnakeCrashed()
        {
            if (!IsFlexible && (NewSnakeHead.row < 0 || NewSnakeHead.col < 0 ||
                  NewSnakeHead.row >= Console.WindowWidth || NewSnakeHead.col >= Console.WindowHeight ||
                  SnakeElements.Contains(NewSnakeHead) || Obstacles.Contains(NewSnakeHead)))
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Yer' done!");
                Console.WriteLine("Yav' eaten {0} apples bah' yer' thicky wanka", FoodsEaten);
                return true;
            }

            return false;
        }

        static void TryEvadingCrash()
        {
            if (IsFlexible && (NewSnakeHead.row < 0 || NewSnakeHead.col < 0 ||
                NewSnakeHead.row >= Console.WindowWidth || NewSnakeHead.col >= Console.WindowHeight ||
                SnakeElements.Contains(NewSnakeHead) || Obstacles.Contains(NewSnakeHead)))
            {
                // if the snake is flexible and has reached dumped its head somewhere 
                // remain calm just move on

                // clear the current snake parts from the console
                foreach (var snakeElement in SnakeElements)
                {
                    Console.SetCursorPosition(snakeElement.row, snakeElement.col);
                    Console.Write("    ");
                }

                // convert to list change the position and invert back to queue
                List<Position> snakePositions = SnakeElements.ToList();

                if (NewSnakeHead.row < 0)
                {
                    snakePositions.Insert(0, new Position(Console.WindowWidth - 1, snakePositions[1].col));

                    for (int i = 1; i < snakePositions.Count; i++)
                    {
                        snakePositions[i] = new Position(Console.WindowWidth - i - 1, snakePositions[i].col);
                        SnakeElements.Dequeue();
                    }
                }

                else if (NewSnakeHead.row == Console.WindowWidth)
                {
                    snakePositions.Insert(0, new Position(0, snakePositions[1].col));

                    for (int i = 1; i < snakePositions.Count; i++)
                    {
                        snakePositions[i] = new Position(i, snakePositions[i].col);
                        SnakeElements.Dequeue();
                    }
                }

                else if (NewSnakeHead.col < 0)
                {
                    snakePositions.Insert(0, new Position(snakePositions[1].row, Console.WindowHeight - 1));

                    for (int i = 1; i < snakePositions.Count; i++)
                    {
                        snakePositions[i] = new Position(snakePositions[i].row, Console.WindowHeight - i - 1);
                        SnakeElements.Dequeue();
                    }
                }

                else if (NewSnakeHead.col == Console.WindowHeight)
                {
                    snakePositions.Insert(0, new Position(snakePositions[1].row, 0));

                    for (int i = 1; i < snakePositions.Count; i++)
                    {
                        snakePositions[i] = new Position(snakePositions[i].row, i);
                        SnakeElements.Dequeue();
                    }
                }

                foreach (var snakePos in snakePositions)
                    SnakeElements.Enqueue(snakePos);
            }
        }

        static void TryEatingFood()
        {
            if (NewSnakeHead.row == Food.row && NewSnakeHead.col == Food.col)
            {
                do
                {
                    Food = new Position(NextRandomPosition.Next(1, Console.WindowWidth - 1),
                                       NextRandomPosition.Next(1, Console.WindowHeight - 1));
                } while (SnakeElements.Contains(Food));
                Console.SetCursorPosition(Food.row, Food.col);
                Console.Write("@");
                FoodsEaten++;
                LastFoodTime = Environment.TickCount;
                TravelSpeed -= IncreaseSpeed;
            }

            else if (NewSnakeHead.row == SpecialFood.row && NewSnakeHead.col == SpecialFood.col)
            {
                // activate the special food effect
                // then create a new one

                if (SpecialFoodChar == '#')
                {
                    int oneFourth = SnakeElements.Count / 4;

                    for (int i = 0; i <= oneFourth; i++)
                    {
                        Position tail = SnakeElements.Dequeue();
                        Console.SetCursorPosition(tail.row, tail.col);
                        Console.Write(" ");
                    }
                }

                else if (SpecialFoodChar == '$')
                    TravelSpeed -= IncreaseSpeed;
                else if (SpecialFoodChar == '&')
                    IsFlexible = true;
                else if (SpecialFoodChar == '~')
                {
                    /* make the queue a list. clear all elements
                     * reverse the postion, reverse the queue and translate the list to a queue
                    */

                    var snakePositions = SnakeElements.ToList();
                    SnakeElements.Clear();
                    snakePositions.Reverse();

                    if (Direction == Left)
                        Direction = Right;
                    if (Direction == Right)
                        Direction = Left;
                    if (Direction == Up)
                        Direction = Down;
                    if (Direction == Down)
                        Direction = Up;

                    foreach (var snakePos in snakePositions)
                        SnakeElements.Enqueue(snakePos);

                }
                else if (SpecialFoodChar == 'T')
                    TravelSpeed += IncreaseSpeed;

                do
                {
                    SpecialFood = new Position(NextRandomPosition.Next(0, Console.WindowWidth - 1),
                                              NextRandomPosition.Next(0, Console.WindowHeight - 1));
                } while (SnakeElements.Contains(Food));

                // pick a symbol
                SpecialFoodChar = SpecialFoodsChars[NextRandomPosition.Next(0, SpecialFoodsChars.Count - 1)];
                Console.SetCursorPosition(SpecialFood.row, SpecialFood.col);
                Console.Write(SpecialFoodChar);
            }

            else // if it has not eaten something the tail is cut
            {
                Position tail = SnakeElements.Dequeue();
                Console.SetCursorPosition(tail.row, tail.col);
                Console.Write(" ");
            }
        }
        #endregion

        #region Adding Objects
        static void AddOrdinaryFood()
        {
            do
            {
                Food = new Position(NextRandomPosition.Next(0, Console.WindowWidth),
                                    NextRandomPosition.Next(0, Console.WindowHeight));

            } while (SnakeElements.Contains(Food));

            DrawFigure(Food.row, Food.col, '@');
        }

        static void AddSpecialFood()
        {

            do
            {
                SpecialFood = new Position(NextRandomPosition.Next(0, Console.WindowWidth), NextRandomPosition.Next(0, Console.WindowHeight));
            } while (SnakeElements.Contains(SpecialFood));

            // pick a symbol 
            SpecialFoodChar = SpecialFoodsChars[NextRandomPosition.Next(0, SpecialFoodsChars.Count - 1)];
            DrawFigure(SpecialFood.row, SpecialFood.col, SpecialFoodChar);
        }

        static void AddObstacles()
        {
            foreach (var obstacle in Obstacles)
                DrawFigure(obstacle.row, obstacle.col, '!');
        }

        static void TrySpawningFood()
        {
            if (Environment.TickCount - LastFoodTime >= FoodDisappearTime)
            {
                Console.SetCursorPosition(Food.row, Food.col);
                Console.Write(" ");
                do
                {
                    Food = new Position(NextRandomPosition.Next(0, Console.WindowWidth),
                                       NextRandomPosition.Next(0, Console.WindowHeight));
                } while (SnakeElements.Contains(Food));
                Console.SetCursorPosition(Food.row, Food.col);
                Console.Write("@");
                LastFoodTime = Environment.TickCount;
            }
        }


        #endregion

        static void ReadInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo userInput = Console.ReadKey();

                if (userInput.Key == ConsoleKey.RightArrow)
                {
                    if (Direction != Left)
                        Direction = Right;
                }

                else if (userInput.Key == ConsoleKey.LeftArrow)
                {
                    if (Direction != Right)
                        Direction = Left;
                }

                else if (userInput.Key == ConsoleKey.UpArrow)
                {
                    if (Direction != Down)
                        Direction = Up;
                }

                else if (userInput.Key == ConsoleKey.DownArrow)
                {
                    if (Direction != Up)
                        Direction = Down;
                }
            }
        }

        static void InitializeGame()
        {
            Console.CursorVisible = false;
            Console.BufferHeight = Console.WindowHeight;

            CreateSnake();
            AddOrdinaryFood();
            AddSpecialFood();
            AddObstacles();
        }
        
        static void Main()
        {
            // initialize the game
            InitializeGame();
        
            while (true)
            {
                ReadInput();
                TryUpdatingSnakeHead();
                if (HasSnakeCrashed())
                    return;
                TryEvadingCrash();
                DrawTail();
                DrawHead();
                TryEatingFood();
                TrySpawningFood();

                Thread.Sleep((int)TravelSpeed); 
            }
        }
    }
}
