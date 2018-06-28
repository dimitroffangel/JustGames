using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JustPopcorn
{
    class StartUp
    {
        class Position
        {
            public int row;
            public int col;
            public char symbol;

            public Position(int row, int col, char symbol)
            {
                this.row = row;
                this.col = col;
                this.symbol = symbol;
            }

            public Position(int row, int col)
            {
                this.row = row;
                this.col = col;
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
        static bool MoveUp;
        static bool MoveRight;
        static int BallX;
        static int BallY;
        static int BulletX;
        static int BulletY;
        static int[,] Bricks;
        static int BallsLeft;
        static int Score;
        static List<Position> SpecialItemsPositions;
        static Random RandomGenerator = new Random();
        static double Speed;
        static bool HasShield = false;
        static int ShieldDuration;
        static int BulletsAmmo;
        static bool isFired;

        static void InitializeVariables()
        {
            PlayerPadding = InitialPlayerPadding;
            playerX = Console.WindowWidth / 2 - 10;
            playerY = Console.WindowHeight - 1;

            BallInitialX = playerX;
            BallInitialY = playerY - 10;

            MoveUp = false;
            MoveRight = true;
            BallX = playerX;
            BallY = playerY - 10;
            BulletX = (PlayerPadding + playerX) / 2;
            BulletY = playerY - 1;
            int maxBoard = Math.Max(Console.WindowWidth, Console.WindowHeight);
            Bricks = new int[maxBoard, maxBoard];
            BallsLeft = int.MaxValue;
            Score = 0;
            SpecialItemsPositions = new List<Position>();
            Speed = 80;
            ShieldDuration = 45;
            BulletsAmmo = 1;
            isFired = false;
        }

        static void DrawPlayer()
        {
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = playerX; i < playerX + InitialPlayerPadding; i++)
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
                DrawFigure(placement.row, placement.col, '<');
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
                DrawFigure(placement.row, placement.col, '>');
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
                DrawFigure(placement.row, placement.col, '#');
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
                DrawFigure(placement.row, placement.col, '*');
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
                DrawFigure(placement.row, placement.col, (char)128);
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
                DrawFigure(placement.row, placement.col, (char)158);
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

        static void DrawFigure(int x, int y, char character)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(character);
        }

        static void MovePlayer()
        {
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo userInput = Console.ReadKey();
                
                if (userInput.Key == ConsoleKey.LeftArrow && playerX > 0)
                {
                    playerX--;
                    Console.SetCursorPosition(playerX + InitialPlayerPadding, playerY);
                    Console.Write(" ");
                }

                if (userInput.Key == ConsoleKey.RightArrow && playerX + InitialPlayerPadding < Console.WindowWidth - 2)
                {
                    Console.SetCursorPosition(playerX, playerY);
                    Console.Write(" ");
                    playerX++;
                }
            }
        }

        static void MoveBall()
        {
            Console.SetCursorPosition(BallX, BallY);
            Console.Write(" ");

            if (MoveUp)
                BallY--;
            else
                BallY++;

            if (MoveRight)
                BallX++;
            else
                BallX--;

            if (BallX == 0)
                MoveRight = true;

            if (BallX == Console.WindowWidth - 2)
                MoveRight = false;

            if (BallY == 0)
                MoveUp = false;

            if (BallY == Console.WindowHeight - 1 && !HasShield)
            {
                BallY = BallInitialY;
                BallX = BallInitialX;
                PlayerPadding = InitialPlayerPadding;
                DrawBall();
            }

            else if (BallY == Console.WindowHeight - 2 && HasShield)
                MoveUp = true;

            if (BallY + 1 == playerY)
            {
                if (BallX >= playerX - 1 && BallX <= playerX + InitialPlayerPadding)
                {
                    Random random = new Random();
                    MoveUp = true;
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

            if (BallY <= Console.WindowHeight - 2 && BallX <= Console.WindowWidth - 2 && BallX > 0 && BallY > 0)
            {
                Position rightUp = new Position(BallX + 1, BallY + 1);
                Position up = new Position(BallX, BallY + 1);
                Position leftUp = new Position(BallX - 1, BallY);

                if (SpecialItemsPositions.Contains(rightUp))
                    CheckCharacter(rightUp);

                if (SpecialItemsPositions.Contains(up))
                    CheckCharacter(up);

                if (SpecialItemsPositions.Contains(leftUp))
                    CheckCharacter(leftUp);

                //if it is a brick
                //ball watching from down
                if (Bricks[BallX + 1, BallY + 1] != 0)
                {
                    DrawFigure(BallX + 1, BallY + 1, ' ');
                    Bricks[BallX + 1, BallY + 1] = 0;
               //     MoveUp = false;
             //       MoveRight = false;
                    Score += 25;
                }

                if (Bricks[BallX, BallY + 1] != 0)
                {
                    DrawFigure(BallX + 1, BallY + 1, ' ');
                    Bricks[BallX, BallY + 1] = 0;
                 //   MoveUp = false;
                    Score += 25;
                }

                if (Bricks[BallX - 1, BallY + 1] != 0)
                {
                    DrawFigure(BallX - 1, BallY + 1, ' ');
                    Bricks[BallX - 1, BallY + 1] = 0;
                //    MoveUp = false;
              //      MoveRight = true;
                    Score += 25;
                }
            }

        }
        static void CheckCharacter(Position suspect)
        {
            
            for(int i = 0; i < SpecialItemsPositions.Count; i++)
            {
                Position item = SpecialItemsPositions[i];

                if (item.row == suspect.row && item.col == suspect.col)
                {
                    if (item.symbol == '>')
                        Speed = Speed / 2.5;

                    else if (item.symbol == '<')
                        Speed = Speed * 1.5;

                    else if (item.symbol == '#')
                        PlayerPadding *= 3;

                    else if (item.symbol == '*')
                        Score = (int)(Score * 1.5);

                    else if (item.symbol == (char)128)
                    {
                        HasShield = true;
                        playerY = Console.WindowHeight - 2;
                        for (int x = 0; x < Console.WindowWidth - 1; x++)
                            DrawFigure(x, playerY + 1, '-');
                        ShieldDuration = 45;
                    }

                    else if (item.symbol == (char)158)
                    {
                        BulletsAmmo = 3;
                    }

                    SpecialItemsPositions.Remove(item);
                    return;
                }
            }
        }

        static void MoveBullet()
         {
             Thread.Sleep(100);
             if (Console.KeyAvailable && !isFired)
             {
                ConsoleKeyInfo userInput = Console.ReadKey();

                if (userInput.Key == ConsoleKey.Spacebar)
                    isFired = true;    
             }

             else if (isFired)
             {
                 DrawFigure(BulletX, BulletY, ' ');
                 BulletY--;

                 if (BulletY == 0)
                 {
                     Console.ForegroundColor = ConsoleColor.Black;
                     BulletY = playerY - 2;
                     BulletsAmmo--;
                 }

                 DrawFigure(BulletX, BulletY, '|');
             }
        }

        static void WriteScore()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write("Score: {0}", Score);
            Console.Write(" Balls left: {0} ", BallsLeft);
            Console.Write("Bullets: {0}", BulletsAmmo);
        }

        static void Main()
        {
            Console.CursorVisible = false;
            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.WindowWidth;

            InitializeVariables();
            DrawBricks();
            DrawSpecialItems();

            while (BallsLeft > 0)
            {
                if (HasShield)
                {
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
                
                MovePlayer();
                MoveBall();

                if (BulletsAmmo > 0)
                    MoveBullet();

                DrawBall();
                DrawPlayer();

                WriteScore();
                Thread.Sleep(60);
            }

            if (BallsLeft < 0)
            {
                Console.SetCursorPosition(10, playerY - 5);
                Console.WriteLine("You have failed with a tiny score of {0}", Score);
            }
        }
    }
}
