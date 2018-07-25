using System;
using System.Collections.Generic;
using System.Threading;

namespace JustPingPong
{
    class StartUp
    {
        static int firstPlayerPadSize = 4;
        static int secondPlayerPadSize = 12;
        static int firstPlayerPosition = Console.WindowHeight/ 2 - firstPlayerPadSize / 2;
        static int secondPlayerPosition = Console.WindowHeight / 2- secondPlayerPadSize / 2;
        static int ballPositionX = Console.WindowWidth / 2;
        static int ballPositionY = Console.WindowHeight / 2;
        static int printResult_Width = Console.WindowWidth / 2;
        static int printResult_Height = 0;
        static bool ballDirectionUp = true;
        static bool ballDirectionRight = true;
        static int firstPlayerResult = 0;
        static int secondPlayerResult = 0;
        static string currentMode = "void";

        static int UI_button_width = Console.WindowWidth / 2 - 10;
        static int UI_button_height = Console.WindowHeight / 2;
        static Random randomGenerator = new Random();

        static void RemoveScroll() // pre-game operations
        {
            Console.CursorVisible = false;
            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.WindowWidth;
        }

        static void PrintResult() // main function for drawing
        {
            Console.SetCursorPosition(printResult_Width, printResult_Height);
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
                PrintAtPosition(Console.WindowWidth-2, y, '|');
                PrintAtPosition(Console.WindowWidth-3, y, '|');
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
            if(currentMode == "2) Player vs Player")
            {
                if (secondPlayerPosition > 0)
                    secondPlayerPosition--;

                return;
            }

            if (secondPlayerPosition > 0) // PVP
                secondPlayerPosition--;
        }

        static void MoveSecondPlayerDown()
        {
            if (currentMode == "2) Player vs Player")
            {
                if (secondPlayerPosition < Console.WindowHeight - secondPlayerPadSize)
                    secondPlayerPosition++;

                return;
            }

            if (secondPlayerPosition < Console.WindowHeight - secondPlayerPadSize) // PVP
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
                ballDirectionUp = false; // change the direction of the ball

            if (ballPositionY == Console.WindowHeight - 1) // set the coure to go up 
                ballDirectionUp = true;

            if (ballPositionX == 0) // has surpassed the defence of player 1
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

            if (ballPositionX == Console.WindowWidth - 1) // has surpassed the defence of player 2 / AI
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

            if (ballPositionX == 2) // if it is colliding with player 1
            { 
                if (ballPositionY >=  firstPlayerPosition && ballPositionY <= 
                    firstPlayerPosition + firstPlayerPadSize)
                {
                    ballDirectionRight = true;
                }
            }

            if (ballPositionX > Console.WindowWidth - 4) // if it is colliding with the player 2/ AI
            {
                if (ballPositionY >= secondPlayerPosition && 
                    ballPositionY < secondPlayerPosition + secondPlayerPadSize)
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
                Console.Write("Please enter a number");
                LoadMainMenu();
            }

            currentMode = options[input-1];

            if (input == 1)
                LoadPVE();
            else if (input == 2)
                LoadPVP();
            else if (input == 3)
                Environment.Exit(0);

            LoadMainMenu();
        }

        static void GameFunctions()
        {
            MoveBall();

            DrawFirstPlayer();
            DrawSecondPlayer();
            DrawBall();
            PrintResult();

            Thread.Sleep(40);
        }

        static void LoadPVE()
        {
            while (true)
            {
                Console.Clear();

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo userInput = Console.ReadKey();

                    if (userInput.Key == ConsoleKey.UpArrow)
                        MoveFirstPlayerUp();

                    else if (userInput.Key == ConsoleKey.DownArrow)
                        MoveFirstPlayerDown();
                }
                MoveSecondPlayerAI();
                GameFunctions();
            }
        }

        static void LoadPVP()
        {
            // setting players padding length
            #region
            int input = 0;

            Console.WriteLine("How Long do you want to be your pad size player 1 ?: ");
            while(!int.TryParse(Console.ReadLine(), out input) ||
                input <= 0 || input > (double)(Console.WindowHeight * 0.6))
                Console.WriteLine("Please enter a number between 0 and {0}", 
                    Console.WindowHeight / 2 + 1);

            firstPlayerPadSize = input;
            firstPlayerPosition = Console.WindowHeight / 2 - firstPlayerPadSize / 2;
            input = 0;

            Console.WriteLine("How Long do you want to be your pad size player 2 ?: ");
            while (!int.TryParse(Console.ReadLine(), out input) ||
                input <= 0 || input > (double)(Console.WindowHeight * 0.6))
                Console.WriteLine("Please enter a number between 0 and {0}", 
                    Console.WindowHeight / 2 + 1);

            secondPlayerPadSize = input;
            secondPlayerPosition = Console.WindowHeight / 2 - secondPlayerPadSize / 2;
            #endregion
            while (true)
            {
                Console.Clear();

                if(Console.KeyAvailable)
                {
                    ConsoleKeyInfo userInput = Console.ReadKey();

                    if (userInput.Key == ConsoleKey.UpArrow)
                        MoveFirstPlayerUp();
                    else if (userInput.Key == ConsoleKey.DownArrow)
                        MoveFirstPlayerDown();
                    else if (userInput.Key == ConsoleKey.W)
                        MoveSecondPlayerUp();
                    else if (userInput.Key == ConsoleKey.S)
                        MoveSecondPlayerDown();
                }
                GameFunctions();
            }
        }

        static void Main()
        {
            RemoveScroll();
            LoadMainMenu();
        }
    }
}
