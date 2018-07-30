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
        static int lastFoodTime = Environment.TickCount;
        static int lastSpecialItem = Environment.TickCount;
        static bool isFlexible = false;
        static Queue<Position> snakeElements = new Queue<Position>();
        static Position snakeHead;
        static Position nextDirection;
        static Position newSnakeHead;

        static Position[] directions = new Position[]
        {
                new Position(1, 0), //right
                new Position(-1, 0), //left
                new Position(0, -1), //up
                new Position(0, 1) //down
        };

        static int direction = right;
        static Random nextRandomPosition = new Random();

        static List<int> primes = new List<int>()
            {
                11,13,17,19, // current obstacle positions
            };
        static List<Position> obstacles = new List<Position>()
            {
                    new Position(nextRandomPosition.Next(0, Console.WindowWidth),
                                 nextRandomPosition.Next(0, Console.WindowHeight)),
                    new Position(nextRandomPosition.Next(0, Console.WindowWidth),
                                 nextRandomPosition.Next(0, Console.WindowHeight)),
                    new Position(nextRandomPosition.Next(0, Console.WindowWidth),
                                 nextRandomPosition.Next(0, Console.WindowHeight)),
                    new Position(nextRandomPosition.Next(0, Console.WindowWidth),
                                 nextRandomPosition.Next(0, Console.WindowHeight)),
                    new Position(nextRandomPosition.Next(0, Console.WindowWidth),
                                 nextRandomPosition.Next(0, Console.WindowHeight))
            };
        static char specialFoodChar = '#'; // each symbol will be randomly chosen
        static List<char> specialFoodsChars = new List<char>()
            {
                '#', // mashroom that takes 1/4 of your tail
                '$', // speed
                '&', // allows to move through walls
                '~', // your tail becomes your head
                'T' // slow time
            };
        static Position food = new Position();
        static Position specialFood = new Position();

        #region constValues
        const byte right = 0;
        const byte left = 1;
        const byte up = 2;
        const byte down = 3;
        const int foodDisappearTime = 8000;
        const int specialDissapearTime = 8000;
        const float increaseSteadySpeed = 0.1f;
        const float increaseSpeed = 0.4f;
        #endregion

        static void DrawFigure(int x, int y, char symbol)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(symbol);
        }


        static void CreateSnake()
        {
            for (int i = 0; i < 5; i++) // initialize the snake
                snakeElements.Enqueue(new Position(i, 0));

            foreach (var particle in snakeElements) // draw the snake
                DrawFigure(particle.row, particle.col, '*');

            TryUpdatingSnakeHead();
        }

        static void DrawTail()
        {
            DrawFigure(snakeHead.row, snakeHead.col, '*'); // replace the head with a body
        }

        static void DrawHead()
        {
            if (newSnakeHead.row >= 0 && newSnakeHead.row < Console.WindowWidth &&
                newSnakeHead.col >= 0 && newSnakeHead.col < Console.WindowHeight)
            {
                snakeElements.Enqueue(newSnakeHead); // and set the new head
                Console.SetCursorPosition(newSnakeHead.row, newSnakeHead.col);
            }

            TravelSpeed -= increaseSteadySpeed;

            if (direction == left)
                Console.Write("<");
            if (direction == right)
                Console.Write(">");
            if (direction == up)
                Console.Write("^");
            if (direction == down)
                Console.Write("v");
        }

        static void TryUpdatingSnakeHead()
        {
            snakeHead = snakeElements.Last();
            nextDirection = directions[direction];
            newSnakeHead = new Position(snakeHead.row + nextDirection.row,
                                        snakeHead.col + nextDirection.col);
        }

        #region EnviromentCollision
        static bool HasSnakeCrashed()
        {
            if (!isFlexible && (newSnakeHead.row < 0 || newSnakeHead.col < 0 ||
                  newSnakeHead.row >= Console.WindowWidth || newSnakeHead.col >= Console.WindowHeight ||
                  snakeElements.Contains(newSnakeHead) || obstacles.Contains(newSnakeHead)))
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
            if (isFlexible && (newSnakeHead.row < 0 || newSnakeHead.col < 0 ||
                newSnakeHead.row >= Console.WindowWidth || newSnakeHead.col >= Console.WindowHeight ||
                snakeElements.Contains(newSnakeHead) || obstacles.Contains(newSnakeHead)))
            {
                // if the snake is flexible and has reached dumped its head somewhere 
                // remain calm just move on

                // clear the current snake parts from the console
                foreach (var snakeElement in snakeElements)
                {
                    Console.SetCursorPosition(snakeElement.row, snakeElement.col);
                    Console.Write("    ");
                }

                // convert to list change the position and invert back to queue
                List<Position> snakePositions = snakeElements.ToList();

                if (newSnakeHead.row < 0)
                {
                    snakePositions.Insert(0, new Position(Console.WindowWidth - 1, snakePositions[1].col));

                    for (int i = 1; i < snakePositions.Count; i++)
                    {
                        snakePositions[i] = new Position(Console.WindowWidth - i - 1, snakePositions[i].col);
                        snakeElements.Dequeue();
                    }
                }

                else if (newSnakeHead.row == Console.WindowWidth)
                {
                    snakePositions.Insert(0, new Position(0, snakePositions[1].col));

                    for (int i = 1; i < snakePositions.Count; i++)
                    {
                        snakePositions[i] = new Position(i, snakePositions[i].col);
                        snakeElements.Dequeue();
                    }
                }

                else if (newSnakeHead.col < 0)
                {
                    snakePositions.Insert(0, new Position(snakePositions[1].row, Console.WindowHeight - 1));

                    for (int i = 1; i < snakePositions.Count; i++)
                    {
                        snakePositions[i] = new Position(snakePositions[i].row, Console.WindowHeight - i - 1);
                        snakeElements.Dequeue();
                    }
                }

                else if (newSnakeHead.col == Console.WindowHeight)
                {
                    snakePositions.Insert(0, new Position(snakePositions[1].row, 0));

                    for (int i = 1; i < snakePositions.Count; i++)
                    {
                        snakePositions[i] = new Position(snakePositions[i].row, i);
                        snakeElements.Dequeue();
                    }
                }

                foreach (var snakePos in snakePositions)
                    snakeElements.Enqueue(snakePos);
            }
        }

        static void TryEatingFood()
        {
            if (newSnakeHead.row == food.row && newSnakeHead.col == food.col)
            {
                do
                {
                    food = new Position(nextRandomPosition.Next(1, Console.WindowWidth - 1),
                                       nextRandomPosition.Next(1, Console.WindowHeight - 1));
                } while (snakeElements.Contains(food));
                Console.SetCursorPosition(food.row, food.col);
                Console.Write("@");
                FoodsEaten++;
                lastFoodTime = Environment.TickCount;
                TravelSpeed -= increaseSpeed;
            }

            else if (newSnakeHead.row == specialFood.row && newSnakeHead.col == specialFood.col)
            {
                // activate the special food effect
                // then create a new one

                if (specialFoodChar == '#')
                {
                    int oneFourth = snakeElements.Count / 4;

                    for (int i = 0; i <= oneFourth; i++)
                    {
                        Position tail = snakeElements.Dequeue();
                        Console.SetCursorPosition(tail.row, tail.col);
                        Console.Write(" ");
                    }
                }

                else if (specialFoodChar == '$')
                    TravelSpeed -= increaseSpeed;
                else if (specialFoodChar == '&')
                    isFlexible = true;
                else if (specialFoodChar == '~')
                {
                    /* make the queue a list. clear all elements
                     * reverse the postion, reverse the queue and translate the list to a queue
                    */

                    var snakePositions = snakeElements.ToList();
                    snakeElements.Clear();
                    snakePositions.Reverse();

                    if (direction == left)
                        direction = right;
                    if (direction == right)
                        direction = left;
                    if (direction == up)
                        direction = down;
                    if (direction == down)
                        direction = up;

                    foreach (var snakePos in snakePositions)
                        snakeElements.Enqueue(snakePos);

                }
                else if (specialFoodChar == 'T')
                    TravelSpeed += increaseSpeed;

                do
                {
                    specialFood = new Position(nextRandomPosition.Next(0, Console.WindowWidth - 1),
                                              nextRandomPosition.Next(0, Console.WindowHeight - 1));
                } while (snakeElements.Contains(food));

                // pick a symbol
                specialFoodChar = specialFoodsChars[nextRandomPosition.Next(0, specialFoodsChars.Count - 1)];
                Console.SetCursorPosition(specialFood.row, specialFood.col);
                Console.Write(specialFoodChar);
            }

            else // if it has not eaten something the tail is cut
            {
                Position tail = snakeElements.Dequeue();
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
                food = new Position(nextRandomPosition.Next(0, Console.WindowWidth),
                                    nextRandomPosition.Next(0, Console.WindowHeight));

            } while (snakeElements.Contains(food));

            DrawFigure(food.row, food.col, '@');
        }

        static void AddSpecialFood()
        {

            do
            {
                specialFood = new Position(nextRandomPosition.Next(0, Console.WindowWidth), nextRandomPosition.Next(0, Console.WindowHeight));
            } while (snakeElements.Contains(specialFood));

            // pick a symbol 
            specialFoodChar = specialFoodsChars[nextRandomPosition.Next(0, specialFoodsChars.Count - 1)];
            DrawFigure(specialFood.row, specialFood.col, specialFoodChar);
        }

        static void AddObstacles()
        {
            foreach (var obstacle in obstacles)
                DrawFigure(obstacle.row, obstacle.col, '!');
        }

        static void TrySpawningFood()
        {
            if (Environment.TickCount - lastFoodTime >= foodDisappearTime)
            {
                Console.SetCursorPosition(food.row, food.col);
                Console.Write(" ");
                do
                {
                    food = new Position(nextRandomPosition.Next(0, Console.WindowWidth),
                                       nextRandomPosition.Next(0, Console.WindowHeight));
                } while (snakeElements.Contains(food));
                Console.SetCursorPosition(food.row, food.col);
                Console.Write("@");
                lastFoodTime = Environment.TickCount;
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
                    if (direction != left)
                        direction = right;
                }

                else if (userInput.Key == ConsoleKey.LeftArrow)
                {
                    if (direction != right)
                        direction = left;
                }

                else if (userInput.Key == ConsoleKey.UpArrow)
                {
                    if (direction != down)
                        direction = up;
                }

                else if (userInput.Key == ConsoleKey.DownArrow)
                {
                    if (direction != up)
                        direction = down;
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
