using System;
using System.Collections.Generic;
using System.Threading;

namespace JustPingPong
{
    class StartUp
    {
        const int firstPlayerPadSize = 4;
        const int secondPlayerPadSize = 12;
        static int firstPlayerPosition = Console.WindowHeight/ 2 - firstPlayerPadSize / 2;
        static int secondPlayerPosition = Console.WindowHeight / 2- secondPlayerPadSize / 2;
        static int ballPositionX = Console.WindowWidth / 2;
        static int ballPositionY = Console.WindowHeight / 2;
        static bool ballDirectionUp = true;
        static bool ballDirectionRight = true;
        static int firstPlayerResult = 0;
        static int secondPlayerResult = 0;
        static Random randomGenerator = new Random();

        static void RemoveScroll()
        {
            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.WindowWidth;
        }

        static void PrintResult()
        {
            Console.SetCursorPosition(Console.WindowWidth / 2, 0);
            Console.Write("{0} - {1}", firstPlayerResult, secondPlayerResult);
        }

        static void DrawFirstPlayer()
        {
            for (int y = firstPlayerPosition; y < firstPlayerPosition + firstPlayerPadSize; y++)
            {
                PrintAtPosition(0, y, '|');
                PrintAtPosition(1, y, '|');
            }
        }

        static void PrintAtPosition(int x, int y, char symbol)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(symbol);
        }

        static void DrawSecondPlayer()
        {
            for (int y = secondPlayerPosition; y < secondPlayerPosition + secondPlayerPadSize; y++)
            {
                PrintAtPosition(Console.WindowWidth - 1, y, '|');
                PrintAtPosition(Console.WindowWidth - 2, y, '|');
            }
        }

        static void DrawBall()
        {
            PrintAtPosition(ballPositionX, ballPositionY, '@');
        }

        static void MoveFirstPlayerUp()
        {
            if (firstPlayerPosition > 0)
                firstPlayerPosition--;
        }

        static void MoveFirstPlayerDown()
        {
            if (firstPlayerPosition < Console.WindowHeight- firstPlayerPadSize)
                firstPlayerPosition++;
        }

        static void MoveSecondPlayerUp()
        {
            if (secondPlayerPosition > 0)
                secondPlayerPosition--;
        }

        static void MoveSecondPlayerDown()
        {
            if (secondPlayerPosition < Console.WindowHeight - secondPlayerPosition)
                secondPlayerPosition++;
        }

        static void MoveSecondPlayerAI()
        {
            int randomNumber = randomGenerator.Next(0, 2);

            if (randomNumber == 0)
                MoveSecondPlayerUp();

            else if (randomNumber == 1)
                MoveSecondPlayerDown();
        }

        static void MoveBall()
        {
                
            if (ballPositionY == 0)
                ballDirectionUp = false;

            if (ballPositionY == Console.WindowHeight - 1)
                ballDirectionUp = true;

            if (ballPositionX == 0)
            {
                ballPositionX = Console.WindowWidth / 2;
                ballPositionY = Console.WindowHeight / 2;
                secondPlayerResult++;
                ballDirectionUp = true;
                ballDirectionRight = true;
                Console.WriteLine("Second Player Wins");
                Console.Write("Press a key to enter a new round");
                Console.ReadKey();
            }

            if (ballPositionX == Console.WindowWidth - 1)
            {
                ballPositionX = Console.WindowWidth / 2;
                ballPositionY = Console.WindowHeight / 2;
                ballDirectionUp = true;
                ballDirectionRight = true;
                firstPlayerResult++;
                Console.WriteLine("First Player wins");
                Console.Write("Press a key to enter a new round");
                Console.ReadKey();
            }

            if (ballPositionX < 3)
            {
                if (ballPositionY >= firstPlayerPosition && ballPositionY < firstPlayerPosition + firstPlayerPadSize)
                {
                    ballDirectionRight = true;
                }
            }

            if (ballPositionX > Console.WindowWidth - 3)
            {
                if (ballPositionY >= secondPlayerPosition && ballPositionY < secondPlayerPosition + secondPlayerPadSize)
                    ballDirectionRight = false;
            }

            if (ballDirectionUp)
                ballPositionY--;
            else
                ballPositionY++;

            if (ballDirectionRight)
                ballPositionX++;
            else
                ballPositionX--;
        }

        static void LoadMainMenu()
        {
            List<string> options = new List<string>()
            {
                "Player vs AI",
                "Player vs Player"
            };

            int heightAddition = -1;

            foreach (string option in options)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2 + ++heightAddition);
                Console.WriteLine(option);
            }

            int input = 0;

            while (!int.TryParse(Console.ReadLine(), out input))
            {
                Console.SetCursorPosition(0, 0);
                Console.Write("Please enter a number");
            }
        }

        static void Main()
        {
            RemoveScroll();

            while (true)
            {
                Console.Clear();

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo userInput = Console.ReadKey();

                    if (userInput.Key == ConsoleKey.UpArrow)
                        MoveFirstPlayerUp();

                    if (userInput.Key == ConsoleKey.DownArrow)
                        MoveFirstPlayerDown();
                }
                MoveSecondPlayerAI();
                MoveBall();

                DrawFirstPlayer();
                DrawSecondPlayer();
                DrawBall();
                PrintResult();

                Thread.Sleep(40);
            }
        }
    }
}
