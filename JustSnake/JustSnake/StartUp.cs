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

        static int foodsEaten = 0;
        static double travelSpeed = 100;
        
        const byte right = 0;
        const byte left = 1;
        const byte up = 2;
        const byte down = 3;
        const int foodDisappearTime = 8000;
        const int specialDissapearTime = 8000;
        const float increaseSteadySpeed = 0.1f;
        const float increaseSpeed = 0.4f;

        static void Main(String[] args)
        {
            Console.CursorVisible = false;

            int lastFoodTime = Environment.TickCount;
            int lastSpecialItem = Environment.TickCount;
            bool isAvailable = false;
            bool isFlexible = false;

            Queue<Position> snakeElements = new Queue<Position>();
            Position[] directions = new Position[]
            {
                new Position(1, 0), //right
                new Position(-1, 0), //left
                new Position(0, -1), //up
                new Position(0, 1) //down
            };

            int direction = right;
            Console.BufferHeight = Console.WindowHeight;
            Random nextRandomPosition = new Random();
            
            List<int> primes = new List<int>()
            {
                11,13,17,19, // current obstacle positions
            };

            for (int i = 0; i < 5; i++) // the snake is a queue
                snakeElements.Enqueue(new Position(i, 0));

            foreach (var particle in snakeElements)
            {
                Console.SetCursorPosition(particle.row, particle.col);
                Console.Write("*");
            }

            Position food = new Position();
            do
            {
                food = new Position(nextRandomPosition.Next(0, Console.WindowWidth), nextRandomPosition.Next(0, Console.WindowHeight));

            } while (snakeElements.Contains(food));

            Console.SetCursorPosition(food.row, food.col);
            Console.Write("@");

            Position specialFood = new Position();
            List<char> specialFoodsChars = new List<char>()
            {
                '#', // mashroom that takes 1/4 of your tail
                '$', // speed
                '&', // allows to move through walls
                '~', // your tail becomes your head
                'T' // slow time
            };

            char specialFoodChar = '#'; // each symbol will be randomly chosen

            do
            {
                specialFood = new Position(nextRandomPosition.Next(0, Console.WindowWidth), nextRandomPosition.Next(0, Console.WindowHeight));
            } while (snakeElements.Contains(specialFood));

            // pick a symbol
            specialFoodChar = specialFoodsChars[nextRandomPosition.Next(0, specialFoodsChars.Count - 1)];
            Console.SetCursorPosition(specialFood.row, specialFood.col);
            Console.Write(specialFoodChar);

            List<Position> obstacles = new List<Position>()
            {
                    new Position(nextRandomPosition.Next(0, Console.WindowWidth), nextRandomPosition.Next(0, Console.WindowHeight)),
                    new Position(nextRandomPosition.Next(0, Console.WindowWidth), nextRandomPosition.Next(0, Console.WindowHeight)),
                    new Position(nextRandomPosition.Next(0, Console.WindowWidth), nextRandomPosition.Next(0, Console.WindowHeight)),
                    new Position(nextRandomPosition.Next(0, Console.WindowWidth), nextRandomPosition.Next(0, Console.WindowHeight)),
                    new Position(nextRandomPosition.Next(0, Console.WindowWidth), nextRandomPosition.Next(0, Console.WindowHeight)),
            };

            foreach (var obstacle in obstacles)
            {
                Console.SetCursorPosition(obstacle.row, obstacle.col);
                Console.Write("!");
            }

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo userInput = Console.ReadKey();

                    if (userInput.Key == ConsoleKey.RightArrow)
                    {
                        if(direction != left)
                             direction = right;
                    }

                    else if (userInput.Key == ConsoleKey.LeftArrow)
                    {
                        if(direction != right)
                            direction = left;
                    }

                    else if (userInput.Key == ConsoleKey.UpArrow)
                    {
                        if(direction != down)
                            direction = up;
                    }

                    else if (userInput.Key == ConsoleKey.DownArrow)
                    {
                        if(direction != up)
                            direction = down;
                    }
                }
                
                Position snakeHead = snakeElements.Last();
                Position nextDirection = directions[direction];
                Position newSnakeHead = new Position(snakeHead.row + nextDirection.row, snakeHead.col + nextDirection.col);
                
                if(!isFlexible && (newSnakeHead.row < 0 || newSnakeHead.col < 0 ||
                  newSnakeHead.row >= Console.WindowWidth || newSnakeHead.col >= Console.WindowHeight ||
                  snakeElements.Contains(newSnakeHead) || obstacles.Contains(newSnakeHead)))
                {
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("Yer' done!");
                    Console.WriteLine("Yav' eaten {0} apples bah' yer' thicky wanka", foodsEaten);
                    return;
                }

                else if(isFlexible && (newSnakeHead.row < 0 || newSnakeHead.col < 0 ||
                  newSnakeHead.row >= Console.WindowWidth || newSnakeHead.col >= Console.WindowHeight ||
                  snakeElements.Contains(newSnakeHead) || obstacles.Contains(newSnakeHead)))
                {
                    // clear the current snake parts from the console
                    foreach(var snakeElement in snakeElements)
                    {
                        Console.SetCursorPosition(snakeElement.row, snakeElement.col);
                        Console.Write("    ");
                    }

                    // convert to list change the position and invert back to queue
                    List<Position> snakePositions = snakeElements.ToList();
                    
                    if(newSnakeHead.row < 0)
                    {
                        snakePositions.Insert(0, new Position(Console.WindowWidth - 1, snakePositions[1].col));

                        for (int i = 1; i < snakePositions.Count; i++)
                        {
                            snakePositions[i] = new Position(Console.WindowWidth - i - 1, snakePositions[i].col);
                            snakeElements.Dequeue();
                        }
                    }

                    else if(newSnakeHead.row == Console.WindowWidth)
                    {
                        snakePositions.Insert(0, new Position(0, snakePositions[1].col));

                        for (int i = 1; i < snakePositions.Count; i++)
                        {
                            snakePositions[i] = new Position(i, snakePositions[i].col);
                            snakeElements.Dequeue();
                        }
                    }

                    else if(newSnakeHead.col < 0)
                    {
                        snakePositions.Insert(0, new Position(snakePositions[1].row, Console.WindowHeight -1));

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


                Console.SetCursorPosition(snakeHead.row, snakeHead.col);
                Console.Write("*"); // replace the head with a body


                if (newSnakeHead.row >= 0 && newSnakeHead.row < Console.WindowWidth && 
                    newSnakeHead.col >= 0 && newSnakeHead.col < Console.WindowHeight)
                {
                    snakeElements.Enqueue(newSnakeHead); // and set the new head
                    Console.SetCursorPosition(newSnakeHead.row, newSnakeHead.col);
                }
                travelSpeed -= increaseSteadySpeed;

                if (direction == left)
                    Console.Write("<");
                if (direction == right)
                    Console.Write(">");
                if (direction == up)
                    Console.Write("^");
                if (direction == down)
                    Console.Write("v");

                if (newSnakeHead.row == food.row && newSnakeHead.col == food.col)
                {
                    do
                    {   
                        food = new Position(nextRandomPosition.Next(1, Console.WindowWidth-1), 
                                           nextRandomPosition.Next(1, Console.WindowHeight-1));
                    } while (snakeElements.Contains(food));
                    Console.SetCursorPosition(food.row, food.col);
                    Console.Write("@");
                    foodsEaten++;
                    lastFoodTime = Environment.TickCount;
                    travelSpeed -= increaseSpeed;
                }

                else if(newSnakeHead.row == specialFood.row && newSnakeHead.col == specialFood.col)
                {
                    // activate the special food effect
                    // then create a new one

                    if(specialFoodChar == '#')
                    {
                        int oneFourth = snakeElements.Count / 4;

                        for(int i = 0; i <= oneFourth; i++)
                        {
                            Position tail = snakeElements.Dequeue();
                            Console.SetCursorPosition(tail.row, tail.col);
                            Console.Write(" ");
                        }
                    }

                    else if(specialFoodChar == '$')
                        travelSpeed -= increaseSpeed;
                    else if(specialFoodChar == '&')
                        isFlexible = true;
                    else if(specialFoodChar == '~')
                    {
                        // make the queue a list clear all the elements swap the head and the tails and add the changes
                        var snakePositions = snakeElements.ToList();

                        snakeElements.Clear();

                        var tempPos = snakePositions.Last();
                        snakePositions[snakePositions.Count - 1] = snakePositions[0];
                        snakePositions[0] = tempPos;

                        foreach(var snakePos in snakePositions)
                        {
                            snakeElements.Enqueue(snakePos);
                        }
                    }
                    else if(specialFoodChar == 'T')
                        travelSpeed += increaseSpeed;

                    do
                    {
                        specialFood = new Position(nextRandomPosition.Next(0, Console.WindowWidth-1),
                                                  nextRandomPosition.Next(0, Console.WindowHeight-1));
                    } while (snakeElements.Contains(food));
                    
                    // pick a symbol
                    specialFoodChar = specialFoodsChars[nextRandomPosition.Next(0, specialFoodsChars.Count - 1)];
                    Console.SetCursorPosition(specialFood.row, specialFood.col);
                    Console.Write(specialFoodChar);
                }

                else
                {
                    Position tail = snakeElements.Dequeue();
                    Console.SetCursorPosition(tail.row, tail.col);
                    Console.Write(" "); 
                }
                
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

                Thread.Sleep((int)travelSpeed); 
            }
        }
    }
}
