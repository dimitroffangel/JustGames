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

            public Position(int row, int col, char symbol)
            {
                this.X = row;
                this.Y = col;
                this.Symbol = symbol;
            }

            public Position(int row, int col)
            {
                this.X = row;
                this.Y = col;
            }
        }

        const int InitialPlayerPadding = 10;
        static int PlayerPadding;
        static int BallInitialX;
        static int BallInitialY;

        //Initializing the player position and padding
        static int playerX;
        static int playerY;

        //Declaring position and direction of the ball
        static bool IsMovingUp;
        static bool IsMovingRight;
        static int BallX;
        static int BallY;
        static DateTime ballLastMoveDate;
        const float BallMoveRate = 0.1f;
        static int BulletX;
        static int BulletY;
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
        static bool isFired;

        static void InitializeVariables()
        {
            PlayerPadding = InitialPlayerPadding;
            playerX = Console.WindowWidth / 2 - 10;
            playerY = Console.WindowHeight - 1;

            BallInitialX = playerX;
            BallInitialY = playerY - 10;

            IsMovingUp = false;
            IsMovingRight = true;
            BallX = playerX;
            BallY = playerY - 10;
            BulletX = (PlayerPadding + playerX) / 2;
            BulletY = playerY - 1;
            int maxBoard = Math.Max(Console.WindowWidth, Console.WindowHeight);
            Bricks = new int[maxBoard, maxBoard];
            BallsLeft = int.MaxValue;
            Score = 0;
            SpecialItemsPositions = new List<Position>();
            Speed = 50;
            ShieldDuration = 45;
            BulletsAmmo = 45;
            isFired = false;
        }

        static void DrawFigure(int x, int y, char character)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(character);
        }

        static void DrawPlayer()
        {
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = playerX; i < playerX + PlayerPadding; i++)
            {
                DrawFigure(i, playerY, '=');
            }
        }

        static void DrawBall()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            DrawFigure(BallX, BallY, '@');
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
        static void DrawDecrease()
        {
            int count = RandomGenerator.Next(1, 3);

            for (int i = 0; i < count; i++)
            {
                Position placement = new Position(RandomGenerator.Next(5, Console.WindowWidth - 5),
                                                  RandomGenerator.Next(5, Console.WindowHeight - 15), '<');
                SpecialItemsPositions.Add(placement);
                DrawFigure(placement.X, placement.Y, '<');
            }
        }
        static void DrawIncrease()
        {
            int count = RandomGenerator.Next(1, 3);

            for (int i = 0; i < count; i++)
            {
                Position placement = new Position(RandomGenerator.Next(5, Console.WindowWidth - 5),
                                                  RandomGenerator.Next(5, Console.WindowHeight - 15), '>');
                SpecialItemsPositions.Add(placement);
                DrawFigure(placement.X, placement.Y, '>');
            }
        }
        static void DrawWidenItem()
        {
            int count = RandomGenerator.Next(1, 3);

            for (int i = 0; i < count; i++)
            {
                Position placement = new Position(RandomGenerator.Next(5, Console.WindowWidth - 5),
                                                  RandomGenerator.Next(5, Console.WindowHeight - 15), '#');
                SpecialItemsPositions.Add(placement);
                DrawFigure(placement.X, placement.Y, '#');
            }
        }
        static void DrawScoreMultiplier()
        {
            int count = RandomGenerator.Next(1, 3);

            for (int i = 0; i < count; i++)
            {
                Position placement = new Position(RandomGenerator.Next(5, Console.WindowWidth - 5),
                                                  RandomGenerator.Next(5, Console.WindowHeight - 15), '*');
                SpecialItemsPositions.Add(placement);
                DrawFigure(placement.X, placement.Y, '*');
            }
        }
        static void DrawShield()
        {
            int count = RandomGenerator.Next(1, 3);

            for (int i = 0; i < count; i++)
            {
                Position placement = new Position(RandomGenerator.Next(5, Console.WindowWidth - 5),
                                                  RandomGenerator.Next(5, Console.WindowHeight - 15), (char)(128));
                SpecialItemsPositions.Add(placement);
                DrawFigure(placement.X, placement.Y, (char)128);
            }
        }
        static void DrawWeapon()
        {
            int count = RandomGenerator.Next(1, 3);

            for (int i = 0; i < count; i++)
            {
                Position placement = new Position(RandomGenerator.Next(5, Console.WindowWidth - 5),
                                                  RandomGenerator.Next(5, Console.WindowHeight - 15), (char)158);
                SpecialItemsPositions.Add(placement);
                DrawFigure(placement.X, placement.Y, (char)158);
            }
        }
#endregion

        static void DrawSpecialItems()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            DrawDecrease();
            DrawIncrease();
            DrawWidenItem();
            DrawScoreMultiplier();
            DrawShield();
            DrawWeapon();
        }

        static void MovePlayer()
        {
            if(Console.KeyAvailable)
            {
                ConsoleKeyInfo userInput = Console.ReadKey();
                
                if (userInput.Key == ConsoleKey.LeftArrow && playerX > 0)
                {
                    Console.SetCursorPosition(playerX + InitialPlayerPadding, playerY);
                    Console.Write(" ");
                    playerX--;
                }

                if (userInput.Key == ConsoleKey.RightArrow && 
                    playerX + InitialPlayerPadding < Console.WindowWidth - 2)
                {
                    Console.SetCursorPosition(playerX, playerY);
                    Console.Write(" ");
                    playerX++;
                }

                if (userInput.Key == ConsoleKey.Spacebar && BulletsAmmo > 0)
                {
                    BulletsAmmo--;
                    BulletX = playerX;
                    bullets.Add(new Position(BulletX, BulletY));
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
            Position rightUp = new Position(BallX + 1, BallY - 1);
            Position up = new Position(BallX, BallY - 1);
            Position leftUp = new Position(BallX - 1, BallY - 1);

            if (ContainsSpecialItem(rightUp))
                CheckCharacter(rightUp);

            if (ContainsSpecialItem(up))
                CheckCharacter(up);

            if (ContainsSpecialItem(leftUp))
                CheckCharacter(leftUp);

            if (Bricks[BallX + 1, BallY - 1] != 0)
            {
                DrawFigure(BallX + 1, BallY - 1, ' ');
                Bricks[BallX + 1, BallY - 1] = 0;
                Score += 25;
            }

            if (Bricks[BallX, BallY - 1] != 0)
            {
                DrawFigure(BallX, BallY - 1, ' ');
                Bricks[BallX, BallY - 1] = 0;
                Score += 25;
            }

            if (Bricks[BallX - 1, BallY - 1] != 0)
            {
                DrawFigure(BallX - 1, BallY - 1, ' ');
                Bricks[BallX - 1, BallY - 1] = 0;
                Score += 25;
            }
        }

        static void BallBottomLogic() // when the ball approaches from the bot
        {
            // searching for collision with bricks
            if (!IsMovingUp)
                return;

            Position down = new Position(BallX, BallY + 1);
            Position downLeft = new Position(BallX - 1, BallY + 1);
            Position downRight = new Position(BallX + 1, BallY + 1);

            if (ContainsSpecialItem(down))
                CheckCharacter(down);

            if (ContainsSpecialItem(downRight))
                CheckCharacter(downRight);

            if (ContainsSpecialItem(downLeft))
                CheckCharacter(downLeft);

            if (Bricks[BallX + 1, BallY + 1] != 0)
            {
                DrawFigure(BallX + 1, BallY + 1, ' ');
                Bricks[BallX + 1, BallY + 1] = 0;
                Score += 25;
            }

            if (Bricks[BallX, BallY + 1] != 0)
            {
                DrawFigure(BallX, BallY + 1, ' ');
                Bricks[BallX, BallY + 1] = 0;
                Score += 25;
            }

            if (Bricks[BallX - 1, BallY + 1] != 0)
            {
                DrawFigure(BallX - 1, BallY + 1, ' ');
                Bricks[BallX - 1, BallY + 1] = 0;
                Score += 25;
            }
        }

        static void MoveBall()
        {
            if ((DateTime.Now - ballLastMoveDate).TotalSeconds < BallMoveRate)
                return;

            ballLastMoveDate = DateTime.Now;

            Console.SetCursorPosition(BallX, BallY);
            Console.Write(" ");

            if (IsMovingUp)
                BallY--;
            else
                BallY++;

            if (IsMovingRight)
                BallX++;
            else
                BallX--;

            if (BallX == 0)
                IsMovingRight = true;

            if (BallX == Console.WindowWidth - 2)
                IsMovingRight = false;

            if (BallY == 0)
                IsMovingUp = false;

            if (BallY == Console.WindowHeight - 1 && !HasShield)
            {
                BallY = BallInitialY;
                BallX = BallInitialX;
                PlayerPadding = InitialPlayerPadding;
                DrawBall();
            }

            else if (BallY == Console.WindowHeight - 2 && HasShield)
                IsMovingUp = true;

            if (BallY + 1 == playerY)
            {
                if (BallX >= playerX - 1 && BallX <= playerX + InitialPlayerPadding)
                {
                    Random random = new Random();
                    IsMovingUp = true;
                    int xShift = random.Next(1, 100);
                    if (xShift < 50)
                        BallX += (xShift% 10)%5;
                    else
                        BallX -= (xShift % 10) % 5;

                    if (BallX <= 0)
                        BallX += 1;
                    else if (BallX >= Console.WindowWidth - 1)
                        BallX = Console.WindowHeight - 2;
                }
            }

            DrawBall();

            if (BallY <= Console.WindowHeight - 2 && BallX <= Console.WindowWidth - 2 && BallX > 0 && BallY > 0)
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
                        PlayerPadding *= 3;

                    else if (item.Symbol == '*')
                        Score = (int)(Score * 1.5);

                    else if (item.Symbol == (char)128)
                    {
                         HasShield = true;
                        playerY = Console.WindowHeight - 2;
                        for (int x = 0; x < Console.WindowWidth - 1; x++)
                            DrawFigure(x, playerY + 1, '-');
                        ShieldDuration = 13335;
                    }

                    else if (item.Symbol == (char)158)
                    {
                        BulletsAmmo += 3;
                    }

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

                if (ContainsSpecialItem(new Position(BulletX, BulletY)))
                    CheckCharacter(new Position(BulletX, BulletY));

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
                    DrawFigure(x, playerY + 1, ' ');
                    DrawFigure(x, playerY, ' ');
                }
                playerY = Console.WindowHeight - 1;
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

            MovePlayer();
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
                Console.SetCursorPosition(10, playerY - 5);
                Console.WriteLine("You have failed with a tiny score of {0}", Score);
            }
        }
    }
}
