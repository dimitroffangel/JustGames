using System;
using System.Collections.Generic;
using System.Threading;

namespace JustPopcorn
{
    class StartUp
    {
        class Position
        {
            public int X;
            public int Y;
            public char Symbol;

            public Position(int x, int y, char symbol)
            {
                this.X = x;
                this.Y = y;
                this.Symbol = symbol;
            }

            public Position(int row, int col)
            {
                this.X = row;
                this.Y = col;
            }
        }

        // const values
        const float BallMoveRate = 0.1f;

        //Initializing the player position and padding
        const int InitialPlayerPadding = 10;
        const int InitialShieldDuration = 245;
        static int PlayerPadding;
        static Position InitialBallPosition;
        static Position playerPosition;

        //Declaring position and direction of the ball
        static bool IsMovingUp;
        static bool IsMovingRight;
        static Position ballPosition;
        static DateTime ballLastMoveDate;
        static Position InitialBulletPosition;
        static int[,] Bricks;
        static int BallsLeft;
        static int Score;
        static List<Position> SpecialItemsPositions;
        static Random RandomGenerator = new Random();
        static float Speed;
        static bool HasShield = false;
        static int ShieldDuration;
        static List<Position> bullets = new List<Position>();
        static int BulletsAmmo;

        static void InitializeVariables()
        {
            PlayerPadding = InitialPlayerPadding;
            playerPosition = new Position(Console.WindowWidth / 2 - 10, Console.WindowHeight - 1);

            InitialBallPosition = new Position(playerPosition.X, playerPosition.Y - 10);

            IsMovingUp = false;
            IsMovingRight = true;
            ballPosition = new Position(playerPosition.X, playerPosition.Y - 10);
            InitialBulletPosition = new Position((PlayerPadding + playerPosition.X) / 2, playerPosition.Y - 1);
            int maxBoard = Math.Max(Console.WindowWidth, Console.WindowHeight);
            Bricks = new int[maxBoard, maxBoard];
            BallsLeft = int.MaxValue;
            Score = 0;
            SpecialItemsPositions = new List<Position>();
            Speed = 50;
            ShieldDuration = 45;
            BulletsAmmo = 45;
        }

        static void DrawFigure(Position pos, char character)
        {
            Console.SetCursorPosition(pos.X, pos.Y);
            Console.Write(character);
        }

        static void DrawFigure(int x, int y, char character)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(character);
        }

        static void DrawPlayer()
        {
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = playerPosition.X; i < playerPosition.X + PlayerPadding; i++)
                DrawFigure(i, playerPosition.Y, '=');
        }

        static void ClearPlayer()
        {
            for (int i = playerPosition.X; i < playerPosition.X + PlayerPadding; i++)
                DrawFigure(i, playerPosition.Y, ' ');
        }

        static void DrawBall()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            DrawFigure(ballPosition, '@');
        }

        static void DrawBricks()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            for (int col = 5; col < Console.WindowHeight - 15; col++)
            {
                for (int row = 5; row < Console.WindowWidth - 5; row++)
                {
                    Bricks[row, col] = 1;
                    DrawFigure(row, col, '&');
                }
            }
        }

        #region SpecialItems
        static void DrawSpecialItem(char symbol)
        {
            int count = RandomGenerator.Next(1, 3);

            for (int i = 0; i < count; i++)
            {
                Position placement = new Position(RandomGenerator.Next(5, Console.WindowWidth - 5),
                                                  RandomGenerator.Next(5, Console.WindowHeight - 15), symbol);
                SpecialItemsPositions.Add(placement);
                DrawFigure(placement, symbol);
            }
        }
#endregion

        static void DrawSpecialItems()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            DrawSpecialItem('<'); // decrease item
            DrawSpecialItem('>'); // increase
            DrawSpecialItem('#'); // widenItem
            DrawSpecialItem('*'); // multiplier
            DrawSpecialItem((char)128); // shield
            DrawSpecialItem((char)158);   // weapon
        }

        static void ReadUserInput()
        {
            if(Console.KeyAvailable)
            {
                ConsoleKeyInfo userInput = Console.ReadKey();
                
                if (userInput.Key == ConsoleKey.LeftArrow && playerPosition.X > 0)
                {
                    Console.SetCursorPosition(playerPosition.X + PlayerPadding, playerPosition.Y);
                    Console.Write(" ");
                    playerPosition.X--;
                }

                if (userInput.Key == ConsoleKey.RightArrow && 
                    playerPosition.X + PlayerPadding < Console.WindowWidth - 2)
                {
                    Console.SetCursorPosition(playerPosition.X, playerPosition.Y);
                    Console.Write(" ");
                    playerPosition.X++;
                }

                if (userInput.Key == ConsoleKey.Spacebar && BulletsAmmo > 0)
                {
                    BulletsAmmo--;
                    InitialBulletPosition.X = playerPosition.X;
                    bullets.Add(new Position(InitialBulletPosition.X, InitialBulletPosition.Y));
                }
            }
        }

        static bool ContainsSpecialItem(Position pos)
        {
            foreach(var position in SpecialItemsPositions)
            {
                if (position.X == pos.X && position.Y == pos.Y)
                    return true;
            }

            return false;
        }

        static void BallTopLogic() // when the ball approaches from top
        {
            if (IsMovingUp)
                return;

            // searching for collision with bricks
            Position rightUp = new Position(ballPosition.X + 1, ballPosition.Y - 1);
            Position up = new Position(ballPosition.X, ballPosition.Y - 1);
            Position leftUp = new Position(ballPosition.X - 1, ballPosition.Y - 1);

            if (ContainsSpecialItem(rightUp))
                CheckCharacter(rightUp);

            if (ContainsSpecialItem(up))
                CheckCharacter(up);

            if (ContainsSpecialItem(leftUp))
                CheckCharacter(leftUp);

            if (Bricks[ballPosition.X + 1, ballPosition.Y - 1] != 0)
            {
                DrawFigure(ballPosition.X + 1, ballPosition.Y - 1, ' ');
                Bricks[ballPosition.X + 1, ballPosition.Y - 1] = 0;
                Score += 25;
            }

            if (Bricks[ballPosition.X, ballPosition.Y - 1] != 0)
            {
                DrawFigure(ballPosition.X, ballPosition.Y - 1, ' ');
                Bricks[ballPosition.X, ballPosition.Y - 1] = 0;
                Score += 25;
            }

            if (Bricks[ballPosition.X - 1, ballPosition.Y - 1] != 0)
            {
                DrawFigure(ballPosition.X - 1, ballPosition.Y - 1, ' ');
                Bricks[ballPosition.X - 1, ballPosition.Y - 1] = 0;
                Score += 25;
            }
        }

        static void BallBottomLogic() // when the ball approaches from the bot
        {
            // searching for collision with bricks
            if (!IsMovingUp)
                return;

            Position down = new Position(ballPosition.X, ballPosition.Y + 1);
            Position downLeft = new Position(ballPosition.X - 1, ballPosition.Y + 1);
            Position downRight = new Position(ballPosition.X + 1, ballPosition.Y + 1);

            if (ContainsSpecialItem(down))
                CheckCharacter(down);

            if (ContainsSpecialItem(downRight))
                CheckCharacter(downRight);

            if (ContainsSpecialItem(downLeft))
                CheckCharacter(downLeft);

            if (Bricks[ballPosition.X + 1, ballPosition.Y + 1] != 0)
            {
                DrawFigure(ballPosition.X + 1, ballPosition.Y + 1, ' ');
                Bricks[ballPosition.X + 1, ballPosition.Y + 1] = 0;
                Score += 25;
            }

            if (Bricks[ballPosition.X, ballPosition.Y + 1] != 0)
            {
                DrawFigure(ballPosition.X, ballPosition.Y + 1, ' ');
                Bricks[ballPosition.X, ballPosition.Y + 1] = 0;
                Score += 25;
            }

            if (Bricks[ballPosition.X - 1, ballPosition.Y + 1] != 0)
            {
                DrawFigure(ballPosition.X - 1, ballPosition.Y + 1, ' ');
                Bricks[ballPosition.X - 1, ballPosition.Y+ 1] = 0;
                Score += 25;
            }
        }

        static void MoveBall()
        {
            if ((DateTime.Now - ballLastMoveDate).TotalSeconds < BallMoveRate)
                return;

            ballLastMoveDate = DateTime.Now;
            Console.SetCursorPosition(ballPosition.X, ballPosition.Y);
            Console.Write(" ");

            if (IsMovingUp)
                ballPosition.Y--;
            else
                ballPosition.Y++;

            if (IsMovingRight)
                ballPosition.X++;
            else
                ballPosition.X--;

            if (ballPosition.X == 0)
                IsMovingRight = true;

            if (ballPosition.X == Console.WindowWidth - 2)
                IsMovingRight = false;

            if (ballPosition.Y == 0)
                IsMovingUp = false;

            if (ballPosition.Y == Console.WindowHeight - 1 && !HasShield) // draw a new ball
            {
                ballPosition.X = InitialBallPosition.X;
                ballPosition.Y = InitialBallPosition.Y;
                ClearPlayer();
                PlayerPadding = InitialPlayerPadding;
                DrawBall();
            }

            else if (ballPosition.Y == Console.WindowHeight - 1 && HasShield)
                IsMovingUp = true;

            if (ballPosition.Y + 1 == playerPosition.Y)
            {
                if (ballPosition.X >= playerPosition.X - 1 &&
                    ballPosition.X <= playerPosition.X + PlayerPadding)
                {
                    Random random = new Random();
                    IsMovingUp = true;
                    int xShift = random.Next(1, 100);
                    if (xShift < 50)
                        ballPosition.X += (xShift % 10) % 5;
                    else
                        ballPosition.X -= (xShift % 10) % 5;

                    if (ballPosition.X <= 0)
                        ballPosition.X += 1;
                    else if (ballPosition.X >= Console.WindowWidth - 1)
                        ballPosition.X = Console.WindowWidth - 3;
                }
            }

            DrawBall();

            if (ballPosition.Y <= Console.WindowHeight - 2 &&
                ballPosition.X <= Console.WindowWidth - 2 && ballPosition.X > 0 && ballPosition.Y > 0)
            {
                BallBottomLogic();
                BallTopLogic();
            }
        }
        static void CheckCharacter(Position suspect)
        {
            for(int i = 0; i < SpecialItemsPositions.Count; i++)
            {
                Position item = SpecialItemsPositions[i];

                if (item.X == suspect.X && item.Y == suspect.Y)
                {
                    if (item.Symbol == '>')
                        Speed = Speed / 2.5f;

                    else if (item.Symbol == '<')
                        Speed = Speed * 1.5f;

                    else if (item.Symbol == '#')
                    {
                        if (playerPosition.X + (PlayerPadding *3) >= Console.WindowWidth - 2)
                        {
                            ClearPlayer();
                            playerPosition.X = 0;
                        }

                        PlayerPadding *= 3;
                    }
                    else if (item.Symbol == '*')
                        Score = (int)(Score * 1.5);

                    else if (item.Symbol == (char)128)
                    {
                        HasShield = true;
                        playerPosition.Y = Console.WindowHeight - 2;
                        for (int x = 0; x < Console.WindowWidth - 1; x++)
                            DrawFigure(x, playerPosition.Y + 1, '-');
                        ShieldDuration = InitialShieldDuration;
                    }

                    else if (item.Symbol == (char)158)
                        BulletsAmmo += 3;

                    SpecialItemsPositions.Remove(item);
                    return;
                }
            }
        }

        static void MoveBullet()
         {
            if (BulletsAmmo < 0)
                return;

             for(int i = 0; i < bullets.Count; i++)
             {
                if (Bricks[bullets[i].X, bullets[i].Y] != 0)
                {
                    DrawFigure(bullets[i].X, bullets[i].Y, ' ');
                    Bricks[bullets[i].X, bullets[i].Y] = 0;
                    Score += 25;
                }

                if (ContainsSpecialItem(new Position(bullets[i].X, bullets[i].Y)))
                    CheckCharacter(new Position(bullets[i].X, bullets[i].Y));

                 DrawFigure(bullets[i].X, bullets[i].Y, ' ');
                 bullets[i].Y--;

                 if (bullets[i].Y == 0)
                 {
                    Console.ForegroundColor = ConsoleColor.Black;
                    bullets.Remove(bullets[i]);
                    i--;
                    continue;
                 }

                 DrawFigure(bullets[i].X, bullets[i].Y, '|');
             }
        }

        static void WriteScore()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write("Score: {0}", Score);
            Console.Write(" Balls left: {0} ", BallsLeft);
            Console.Write("Bullets: {0}", BulletsAmmo);
        }

        static void ShieldLogic()
        {
            if (!HasShield)
                return;
            
            ShieldDuration--;

            if (ShieldDuration == 0)
            {
                HasShield = false;
                for (int x = 0; x < Console.WindowWidth - 1; x++)
                {
                    DrawFigure(x, playerPosition.Y + 1, ' ');
                    DrawFigure(x, playerPosition.Y, ' ');
                }
                playerPosition.Y = Console.WindowHeight - 1;
            }
        }

        static void InitializeGame()
        {
            Console.CursorVisible = false;
            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.WindowWidth;

            InitializeVariables();
            DrawBricks();
            DrawSpecialItems();
        }

        static void GameFunctions()
        {
            Thread.Sleep((int)Speed);

            ShieldLogic();

            ReadUserInput();
            MoveBall();
            MoveBullet();

          //  DrawBall();
            DrawPlayer();

            WriteScore();
        }

        static void Main()
        {
            InitializeGame();

            while (BallsLeft > 0)
                GameFunctions();

            // After game...
            if (BallsLeft < 0)
            {
                Console.SetCursorPosition(10, playerPosition.Y - 5);
                Console.WriteLine("You have failed with a tiny score of {0}", Score);
            }
        }
    }
}
