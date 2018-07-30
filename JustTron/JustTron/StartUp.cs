using System;
using System.Collections.Generic;
using System.Threading;

namespace JustTron
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

        static int UI_button_width = Console.WindowWidth / 2 - 10;
        static int UI_button_height = Console.WindowHeight / 2;
        static string CurrentMode = "none";

        static Position FirstPlayerPosition = new Position(Console.WindowWidth / 2 + 10, Console.WindowHeight / 2);
        static Stack<Position> FirstPlayerRoute = new Stack<Position>();
        const int Player_ONE_SnakeLength = 10;
 
        static Position SecondPlayerPosition = new Position(Console.WindowWidth / 2 - 20, Console.WindowHeight / 2);
        static Stack<Position> SecondPlayerRoute = new Stack<Position>();
        const int Player_TWO_SnakeLength = 10;

        static Position DecreaseTimeFigure = new Position();
        static Position IncreaseTimeFigure = new Position();

        static Position[] Directions = new Position[]
            {
                new Position(1, 0), // right
                new Position(-1, 0), //left
                new Position(0, -1), // up
                new Position(0, 1), //down
            };

        static int DirectionFirstPlayer = Left;
        static int DirectionSecondPlayer = Left;

        static Random RandomGenerator = new Random();
        static int FirstPlayerScore = 0;
        static int SecondPlayerScore = 0;
        static int CountSeconds = 1;
        static int Speed = 100;

        //directions
        const byte Right = 0;
        const byte Left = 1;
        const byte Up = 2; 
        const byte Down = 3;

        //countSeconds % one of these two == 0 increase/decrease time
        const int PrimeDecreaseTime = 43;
        const int PrimeIncreaseTime = 23;

        static void DrawFirstPlayer()
        {
            Console.SetCursorPosition(FirstPlayerPosition.X, FirstPlayerPosition.Y);

            for (int i = Player_ONE_SnakeLength; i >= 0; i--)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write("*");
                FirstPlayerRoute.Push(new Position(FirstPlayerPosition.X + i, FirstPlayerPosition.Y));
            }
        }

        static void DrawSecondPlayer()
        {
            Console.SetCursorPosition(SecondPlayerPosition.X, SecondPlayerPosition.Y);

            for (int i = Player_TWO_SnakeLength; i >= 0; i--)
            {
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Write("*");
                SecondPlayerRoute.Push(new Position(SecondPlayerPosition.X + i, FirstPlayerPosition.Y));
            }
        }

        static void ReadUserInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo userInput = Console.ReadKey();

                if (userInput.Key == ConsoleKey.RightArrow)
                {
                    if (DirectionFirstPlayer != Left)
                        DirectionFirstPlayer = Right;
                }

                else if (userInput.Key == ConsoleKey.LeftArrow)
                {
                    if (DirectionFirstPlayer != Right)
                        DirectionFirstPlayer = Left;
                }

                else if (userInput.Key == ConsoleKey.UpArrow)
                {
                    if (DirectionFirstPlayer != Down)
                        DirectionFirstPlayer = Up;
                }

                else if (userInput.Key == ConsoleKey.DownArrow)
                {
                    if (DirectionFirstPlayer != Up)
                        DirectionFirstPlayer = Down;
                }

                if (CurrentMode != "2) Player vs Player")
                    return;

                if (userInput.Key == ConsoleKey.D)
                {
                    if (DirectionSecondPlayer != Left)
                        DirectionSecondPlayer = Right;
                }

                else if (userInput.Key == ConsoleKey.A)
                {
                    if (DirectionSecondPlayer != Right)
                        DirectionSecondPlayer = Left;
                }

                else if (userInput.Key == ConsoleKey.W)
                {
                    if (DirectionSecondPlayer != Down)
                        DirectionSecondPlayer = Up;
                }

                else if (userInput.Key == ConsoleKey.S)
                {
                    if (DirectionSecondPlayer != Up)
                        DirectionSecondPlayer = Down;
                }
            }
        }

        static bool HasPathSecondPlayer(byte way)
        {
            // check if the new head is colliding with the surrounding

            DirectionSecondPlayer = way;
            Position currentHead = SecondPlayerRoute.Peek();
            Position nextDirection = Directions[DirectionSecondPlayer];
            Position newHead = 
                new Position(currentHead.X + nextDirection.X, currentHead.Y + nextDirection.Y);

            if (SecondPlayerRoute.Contains(newHead) || FirstPlayerRoute.Contains(newHead) ||
                newHead.X < 0 || newHead.X >= Console.WindowWidth ||
                 newHead.Y < 0 || newHead.Y >= Console.WindowHeight)
                return false;

            return true;
        }

        static void DirectSecondPlayer_PVE()
        {
            for (byte way = 0; way < 4; way++)
            {
                if (HasPathSecondPlayer(way))
                {
                    return; // has found a path
                }
            }
            return; // will crash
        }

        static bool FirstPlayerMoves()
        {
            /* 1) take the direction
             * create the new head, check for collision
             * push the head
             */
            Position nextDirection = Directions[DirectionFirstPlayer];
            Position currentHead = FirstPlayerRoute.Peek();
            Position newHead = new Position(currentHead.X + nextDirection.X, currentHead.Y + nextDirection.Y);

            if (newHead.X < 0 || newHead.X >= Console.WindowWidth ||
               newHead.Y < 0 || newHead.Y >= Console.WindowHeight ||
               FirstPlayerRoute.Contains(newHead) ||
               SecondPlayerRoute.Contains(newHead))
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.WriteLine("Second player wins");
                SecondPlayerScore++;
                return false;
            }

            Console.SetCursorPosition(newHead.X, newHead.Y);
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write("*");
            FirstPlayerRoute.Push(newHead);
            return true;
        }

        static bool SecondPlayerMoves()
        {
            Position currentHead = SecondPlayerRoute.Peek();
            Position nextDirection = Directions[DirectionSecondPlayer];
            Position newHead = new Position(currentHead.X + nextDirection.X, currentHead.Y + nextDirection.Y);

            if(newHead.X < 0 || newHead.X >= Console.WindowWidth||
               newHead.Y < 0 || newHead.Y >= Console.WindowHeight||
                SecondPlayerRoute.Contains(newHead) ||
                FirstPlayerRoute.Contains(newHead))
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.WriteLine("First Player Wins");
                FirstPlayerScore++;
                return false;
            }

            Console.SetCursorPosition(newHead.X, newHead.Y);
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
            if (CountSeconds % PrimeDecreaseTime == 0)
            {
                do
                {
                    DecreaseTimeFigure = new Position(RandomGenerator.Next(0, Console.WindowWidth - 1),
                                                RandomGenerator.Next(0, Console.WindowHeight));
                } while (FirstPlayerRoute.Contains(DecreaseTimeFigure) || 
                         SecondPlayerRoute.Contains(DecreaseTimeFigure));

                Console.SetCursorPosition(DecreaseTimeFigure.X, DecreaseTimeFigure.Y);
                Console.Write("%");
            }

            if (CountSeconds % PrimeIncreaseTime == 0)
            {
                do
                {
                    IncreaseTimeFigure = new Position(RandomGenerator.Next(0, Console.WindowWidth - 1),
                                                RandomGenerator.Next(0, Console.WindowHeight));
                } while (FirstPlayerRoute.Contains(IncreaseTimeFigure) || SecondPlayerRoute.Contains(IncreaseTimeFigure));

                Console.SetCursorPosition(IncreaseTimeFigure.X, IncreaseTimeFigure.Y);
                Console.Write("&");
            }

            CheckForCollisionsWithItems();
        }

        static void CheckForCollisionsWithItems()
        {
            if (FirstPlayerRoute.Contains(DecreaseTimeFigure) || SecondPlayerRoute.Contains(DecreaseTimeFigure))
            {
                Speed = (int)((float)Speed * 1.5);
                IncreaseTimeFigure = new Position();
            }

            if (FirstPlayerRoute.Contains(IncreaseTimeFigure) || SecondPlayerRoute.Contains(DecreaseTimeFigure))
            {
                Speed = (int)((float)Speed / 2.0);
                DecreaseTimeFigure = new Position();
            }
        }

        static void InitializeGame()
        {
            Console.Clear();
            Console.CursorVisible = false;
            Console.BufferWidth = Console.WindowWidth;
            Console.BufferHeight = Console.WindowHeight;

            DrawFirstPlayer();
            DrawSecondPlayer();
        }

        static void GameFunctions()
        {
           DrawScore();
           ReadUserInput();
           
           TryAddingItems();

           CountSeconds++;
           Thread.Sleep(Speed);
        }

        static void LoadMainMenu()
        {
            List<string> options = new List<string>()
            {
                "1) Player vs AI",
                "2) Player vs Player",
                "3) Exit"
            };

            int heightAddition = -1;

            foreach (string option in options)
            {
                Console.SetCursorPosition(UI_button_width,
                    UI_button_height + ++heightAddition);
                Console.WriteLine(option);
            }

            int input = 0;

            while (!int.TryParse(Console.ReadLine(), out input))
            {
                Console.SetCursorPosition(0, 0);
                Console.Write("Please enter a valid number");
                LoadMainMenu();
            }

            CurrentMode = options[input - 1];

            if (input == 1)
                LoadPVE();

            else if (input == 2)
                LoadPVP();

            else if (input == 3)
            {
                Environment.Exit(0);
            }
        }

        static void LoadPVE()
        {
            InitializeGame();

            while(true)
            {
                DirectSecondPlayer_PVE();
                GameFunctions();

                if (!FirstPlayerMoves() || !SecondPlayerMoves())
                {
                    if (FirstPlayerScore == 1)
                    {
                        Console.WriteLine("Meh yer' too naive'");
                        return;
                    }

                    if (SecondPlayerScore == 1)
                    {
                        Console.WriteLine("Yer' even trying ? shoulda reconsider yer' existence");
                        return;
                    }
                }
            }
        }

        static void LoadPVP()
        {
            InitializeGame();

            while(true)
            {
                GameFunctions();

                if (!FirstPlayerMoves() || !SecondPlayerMoves())
                {
                    if (FirstPlayerScore == 1)
                    {
                        Console.WriteLine("Player 1 wins");
                        return;
                    }

                    if (SecondPlayerScore == 1)
                    {
                        Console.WriteLine("Player 2 wins");
                        return;
                    }
                }
            }
        }

        static void Main()
        {
            LoadMainMenu();
        }
    }
}
