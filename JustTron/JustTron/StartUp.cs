using System;
using System.Collections.Generic;
using System.Threading;

namespace JustTron
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

        static Position FirstPlayerPosition = new Position(Console.WindowWidth / 2 + 10, Console.WindowHeight / 2);
        static Stack<Position> FirstPlayerRoute = new Stack<Position>();

        static Position SecondPlayerPosition = new Position(Console.WindowWidth / 2 - 20, Console.WindowHeight / 2);
        static Stack<Position> SecondPlayerRoute = new Stack<Position>();

        static Position DecreaseTimeFigure = new Position();
        static Position IncreaseTimeFigure = new Position();

        static Position[] Directions = new Position[]
            {
                new Position(1, 0), // right
                new Position(-1, 0), //left
                new Position(0, -1), // up
                new Position(0, 1), //down
            };

        static int DirectionFirstPlayer = left;
        static int DirectionSecondPlayer = right;

        static Random RandomGenerator = new Random();
        static int FirstPlayerScore = 0;
        static int SecondPlayerScore = 0;
        static int CountSeconds = 1;
        static int Speed = 100;

        //directions
        const byte right = 0;
        const byte left = 1;
        const byte up = 2; 
        const byte down = 3;

        //countSeconds % one of these two == 0 increase/decrease time
        const int primeDecreaseTime = 43;
        const int primeIncreaseTime = 23;

        static void DrawFirstPlayer()
        {
            Console.SetCursorPosition(FirstPlayerPosition.row, FirstPlayerPosition.col);

            for (int i = 10; i >= 0; i--)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write("*");
                FirstPlayerRoute.Push(new Position(FirstPlayerPosition.row + i, FirstPlayerPosition.col));
            }
        }

        static void DrawSecondPlayer()
        {
            Console.SetCursorPosition(SecondPlayerPosition.row, SecondPlayerPosition.col);

            for (int i = 10; i >= 0; i--)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Write("*");
                SecondPlayerRoute.Push(new Position(SecondPlayerPosition.row + i, FirstPlayerPosition.col));
            }
        }

        static void DirectFirstPlayer()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo userInput = Console.ReadKey();

                if (userInput.Key == ConsoleKey.RightArrow)
                {
                    if (DirectionFirstPlayer != left)
                        DirectionFirstPlayer = right;
                }

                if (userInput.Key == ConsoleKey.LeftArrow)
                {
                    if (DirectionFirstPlayer != right)
                        DirectionFirstPlayer = left;
                }

                if (userInput.Key == ConsoleKey.UpArrow)
                {
                    if (DirectionFirstPlayer != down)
                        DirectionFirstPlayer = up;
                }

                if (userInput.Key == ConsoleKey.DownArrow)
                {
                    if (DirectionFirstPlayer != up)
                        DirectionFirstPlayer = down;
                }
            }
        }

        static bool HasPath(byte way)
        {
            DirectionSecondPlayer = way;
            Position currentHead = SecondPlayerRoute.Peek();
            Position nextDirection = Directions[DirectionSecondPlayer];
            Position newHead = new Position(currentHead.row + nextDirection.row, currentHead.col + nextDirection.col);

            if (SecondPlayerRoute.Contains(newHead) ||
               FirstPlayerRoute.Contains(newHead) ||
                newHead.row < 0 || newHead.row >= Console.WindowWidth ||
                 newHead.col < 0 || newHead.col >= Console.WindowHeight)
                return false;

            return true;
        }

        static void DirectSecondPlayer()
        {
            //int randomNumber = randomGenerator.Next(0, 50); 

            //if (randomNumber % 2 != 0)
            //{
                for (byte way = 0; way < 4; way++)
                {
                    if (HasPath(way))
                    {
                        return; // has found a path
                    }
                }
                return; // will crash
            //}

            //else
            //{
            //    randomNumber = randomGenerator.Next(0, 50);

            //    if (randomNumber % 2 == 0) // move left or right
            //    {
            //        randomNumber = randomGenerator.Next(0, 50);

            //        if (randomNumber % 2 == 0) // move right
            //        {
            //            if (directionSecondPlayer != left)
            //                directionSecondPlayer = right;
            //        }

            //        else //move left
            //        {
            //            if (directionSecondPlayer != right)
            //                directionSecondPlayer = left;
            //        }
            //    }

            //    else //move up or down
            //    {
            //        randomNumber = randomGenerator.Next(0, 50);

            //        if (randomNumber % 2 == 0) // move up
            //        {
            //            if (directionSecondPlayer != down)
            //                directionSecondPlayer = up;
            //        }

            //        else // move down
            //        {
            //            if (directionSecondPlayer != up)
            //                directionSecondPlayer = down;
            //        }
            //    }
            //}

        }

        static bool FirstPlayerMoves()
        {
            Position currentHead = FirstPlayerRoute.Peek();
            Position nextDirection = Directions[DirectionFirstPlayer];
            Position newHead = new Position(currentHead.row + nextDirection.row, currentHead.col + nextDirection.col);

            if (newHead.row < 0 || newHead.row >= Console.WindowWidth ||
               newHead.col < 0 || newHead.col >= Console.WindowHeight ||
               FirstPlayerRoute.Contains(newHead) ||
               SecondPlayerRoute.Contains(newHead))
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.WriteLine("Second player wins");
                SecondPlayerScore++;
                return false;
            }

            Console.SetCursorPosition(newHead.row, newHead.col);
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write("*");
            FirstPlayerRoute.Push(newHead);
            return true;
        }

        static bool SecondPlayerMoves()
        {
            Position currentHead = SecondPlayerRoute.Peek();
            Position nextDirection = Directions[DirectionSecondPlayer];
            Position newHead = new Position(currentHead.row + nextDirection.row, currentHead.col + nextDirection.col);

            if(newHead.row < 0 || newHead.row >= Console.WindowWidth||
               newHead.col < 0 || newHead.col >= Console.WindowHeight||
                SecondPlayerRoute.Contains(newHead) ||
                FirstPlayerRoute.Contains(newHead))
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.WriteLine("First Player Wins");
                FirstPlayerScore++;
                return false;
            }

            Console.SetCursorPosition(newHead.row, newHead.col);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Write("*");
            SecondPlayerRoute.Push(newHead);

            return true;
        }

        static void DrawScore()
        {
            Console.SetCursorPosition(Console.WindowWidth / 2, 0);
            Console.Write("{0} - {1}", SecondPlayerScore, FirstPlayerScore);
        }

        static void TryAddingItems()
        {
            if (CountSeconds % primeDecreaseTime == 0)
            {
                do
                {
                    DecreaseTimeFigure = new Position(RandomGenerator.Next(0, Console.WindowWidth - 1),
                                                RandomGenerator.Next(0, Console.WindowHeight));
                } while (FirstPlayerRoute.Contains(DecreaseTimeFigure) || SecondPlayerRoute.Contains(DecreaseTimeFigure));

                Console.SetCursorPosition(DecreaseTimeFigure.row, DecreaseTimeFigure.col);
                Console.Write("%");
            }

            if (CountSeconds % primeIncreaseTime == 0)
            {
                do
                {
                    IncreaseTimeFigure = new Position(RandomGenerator.Next(0, Console.WindowWidth - 1),
                                                RandomGenerator.Next(0, Console.WindowHeight));
                } while (FirstPlayerRoute.Contains(IncreaseTimeFigure) || SecondPlayerRoute.Contains(IncreaseTimeFigure));

                Console.SetCursorPosition(IncreaseTimeFigure.row, IncreaseTimeFigure.col);
                Console.Write("&");
            }

            CheckForCollisionsWithItems();
        }

        static void CheckForCollisionsWithItems()
        {
            if (FirstPlayerRoute.Contains(DecreaseTimeFigure) || SecondPlayerRoute.Contains(DecreaseTimeFigure))
            {
                Speed = (int)((double)Speed * 2.5);
                IncreaseTimeFigure = new Position();
            }

            if (FirstPlayerRoute.Contains(IncreaseTimeFigure) || SecondPlayerRoute.Contains(DecreaseTimeFigure))
            {
                Speed = (int)((double)Speed / 1.5);
                DecreaseTimeFigure = new Position();
            }
        }

        static void Main(string[] args)
        {
            Console.BufferWidth = Console.WindowWidth;
            Console.BufferHeight = Console.WindowHeight;

            DrawFirstPlayer();
            DrawSecondPlayer();
            
            while (true)
            {
              DrawScore();
              DirectFirstPlayer();
              DirectSecondPlayer();

              if (!FirstPlayerMoves() || !SecondPlayerMoves())
              {
                  if (FirstPlayerScore == 1)
                  {
                      Console.WriteLine("Meh yer' too naive'");
                      return;
                  }

                  if (SecondPlayerScore == 1)
                  {
                      Console.WriteLine("Yer' even trying shoulda reconsider yer' existence");
                      return;
                  }
              }

              TryAddingItems();

              CountSeconds++;
              Thread.Sleep(Speed);
            }
        }
    }
}
