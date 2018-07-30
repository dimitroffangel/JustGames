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
           public int X;
           public int Y;

           public Position(int x, int y)
           {
               this.X = x;
               this.Y = y;
           }
        }

        static int FoodsEaten = 0;
        static double TravelSpeed = 100;
        static int LastFoodTime = Environment.TickCount;
        static int LastSpecialItem = Environment.TickCount;
        static bool IsFlexible = false;
        static bool HasUsedFlexibility = false;
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
        const float IncreaseSteadilySpeed = 0.1f;
        const float IncreaseSpeed = 0.2f;
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
                DrawFigure(particle.X, particle.Y, '*');

            TryUpdatingSnakeHead();
        }

        static void DrawTail()
        {
            if(HasUsedFlexibility) // if the snake has used flexibilty, 
            // all of its parts must be transfered to the place where the snake is at that moment
                return;

            DrawFigure(SnakeHead.X, SnakeHead.Y, '*'); // replace the head with a body
        }

        static void DrawHead()
        {
            if(HasUsedFlexibility) // if the snake has used flexibilty, 
                // all of its parts must be transfered to the place where the snake is at that moment
            {
                HasUsedFlexibility = false;
                return;
            }

            if (NewSnakeHead.X >= 0 && NewSnakeHead.X < Console.WindowWidth &&
                NewSnakeHead.Y >= 0 && NewSnakeHead.Y < Console.WindowHeight)
            {
                SnakeElements.Enqueue(NewSnakeHead); // and set the new head
                Console.SetCursorPosition(NewSnakeHead.X, NewSnakeHead.Y);
            }

            TravelSpeed -= IncreaseSteadilySpeed;

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
            NewSnakeHead = new Position(SnakeHead.X + NextDirection.X,
                                        SnakeHead.Y + NextDirection.Y);
        }

        #region EnviromentCollision

        static bool HasSnakeDied()
        {
            if (SnakeElements.Count == 0)
                return true;

            return false;
        }

        static bool HasSnakeCrashed()
        {
            if (!IsFlexible && (NewSnakeHead.X < 0 || NewSnakeHead.Y < 0 ||
                  NewSnakeHead.X >= Console.WindowWidth || NewSnakeHead.Y >= Console.WindowHeight ||
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
            if (Obstacles.Contains(NewSnakeHead))
                return;

            if (IsFlexible && (NewSnakeHead.X < 0 || NewSnakeHead.Y < 0 ||
                NewSnakeHead.X >= Console.WindowWidth || NewSnakeHead.Y >= Console.WindowHeight ||
                SnakeElements.Contains(NewSnakeHead)))
            {
                // if the snake is flexible and has reached dumped its head somewhere 
                // remain calm just move on
                HasUsedFlexibility = true;

                // clear the current snake parts from the console
                foreach (var snakeElement in SnakeElements)
                {
                    Console.SetCursorPosition(snakeElement.X, snakeElement.Y);
                    Console.Write(" ");
                }

                // convert to list change the position and invert back to queue
                List<Position> snakePositions = SnakeElements.ToList();
                SnakeElements.Clear();

                if (NewSnakeHead.X < 0)
                {
                    for (int i = 0; i < snakePositions.Count; i++)
                        snakePositions[i] = new Position(Console.WindowWidth - i - 1, snakePositions[i].Y);

                    snakePositions.Insert(0, new Position(snakePositions[1].X - 1, snakePositions[1].Y));
                }

                else if (NewSnakeHead.X == Console.WindowWidth)
                {
                    for (int i = 0; i < snakePositions.Count; i++)
                        snakePositions[i] = new Position(i, snakePositions[i].Y);

                    snakePositions.Insert(0, new Position(snakePositions[1].X + 1, snakePositions[1].Y));
                }

                else if (NewSnakeHead.Y < 0)
                {
                    for (int i = 0; i < snakePositions.Count; i++)
                        snakePositions[i] = new Position(snakePositions[i].X, Console.WindowHeight - i - 1);

                    snakePositions.Insert(0, new Position(snakePositions[1].X, snakePositions[1].Y -1));
                }

                else if (NewSnakeHead.Y == Console.WindowHeight)
                {
                    for (int i = 0; i < snakePositions.Count; i++)
                    {
                        snakePositions[i] = new Position(snakePositions[i].X, i);
                    }

                    snakePositions.Insert(0, new Position(snakePositions[1].X, snakePositions[1].Y+1));
                }

                foreach (var snakePos in snakePositions)
                    SnakeElements.Enqueue(snakePos);
            }
        }

        static void TryEatingFood()
        {
            if (NewSnakeHead.X == Food.X && NewSnakeHead.Y == Food.Y)
            {
                do
                {
                    Food = new Position(NextRandomPosition.Next(1, Console.WindowWidth - 1),
                                       NextRandomPosition.Next(1, Console.WindowHeight - 1));
                } while (SnakeElements.Contains(Food));
                DrawFigure(Food.X, Food.Y, '@');
                FoodsEaten++;
                LastFoodTime = Environment.TickCount;
                TravelSpeed -= IncreaseSpeed;
            }

            else if (NewSnakeHead.X == SpecialFood.X && NewSnakeHead.Y == SpecialFood.Y)
            {
                // activate the special food effect
                // then create a new one

                if (SpecialFoodChar == '#')
                {
                    int oneFourth = SnakeElements.Count / 4;

                    for (int i = 0; i <= oneFourth; i++)
                    {
                        Position tail = SnakeElements.Dequeue();
                        DrawFigure(tail.X, tail.Y, ' ');
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
                DrawFigure(SpecialFood.X, SpecialFood.Y, SpecialFoodChar);
            }

            else // if it has not eaten something the tail is cut
            {
                Position tail = SnakeElements.Dequeue();
                DrawFigure(tail.X, tail.Y, ' ');
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

            DrawFigure(Food.X, Food.Y, '@');
        }

        static void AddSpecialFood()
        {

            do
            {
                SpecialFood = new Position(NextRandomPosition.Next(0, Console.WindowWidth), NextRandomPosition.Next(0, Console.WindowHeight));
            } while (SnakeElements.Contains(SpecialFood));

            // pick a symbol 
            SpecialFoodChar = SpecialFoodsChars[NextRandomPosition.Next(0, SpecialFoodsChars.Count - 1)];
            DrawFigure(SpecialFood.X, SpecialFood.Y, SpecialFoodChar);
        }

        static void AddObstacles()
        {
            foreach (var obstacle in Obstacles)
                DrawFigure(obstacle.X, obstacle.Y, '!');
        }

        static void TrySpawningFood()
        {
            if (Environment.TickCount - LastFoodTime >= FoodDisappearTime)
            {
                Console.SetCursorPosition(Food.X, Food.Y);
                Console.Write(" ");
                do
                {
                    Food = new Position(NextRandomPosition.Next(0, Console.WindowWidth),
                                       NextRandomPosition.Next(0, Console.WindowHeight));
                } while (SnakeElements.Contains(Food));
                Console.SetCursorPosition(Food.X, Food.Y);
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
                if (HasSnakeDied())
                    return;

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
