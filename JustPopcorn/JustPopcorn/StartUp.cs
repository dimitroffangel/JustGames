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
        const float SpeedIncreaser = 2.5f;
        const float SpeedDecreaseer = 3f;
        const float PaddingMultiplier = 3;

        //Initializing the player position and padding
        const int InitialPlayerPadding = 10;
        const int InitialShieldDuration = 245;
        static int PlayerPadding;
        static Position InitialBallPosition;
        static Position PlayerPosition;

        //Declaring position and direction of the ball
        static bool IsMovingUp;
        static bool IsMovingRight;
        static Position BallPosition;
        static DateTime BallLastMoveDate;
        static Position InitialBulletPosition;
        static int[,] Bricks;
        static int BallsLeft;
        static int Score;
        static List<Position> SpecialItemsPositions;
        static Random RandomGenerator = new Random();
        static float Speed;
        static bool HasShield = false;
        static int ShieldDuration;
        static List<Position> Bullets = new List<Position>();
        static int BulletsAmmo;

        static void InitializeVariables()
        {
            PlayerPadding = InitialPlayerPadding;
            PlayerPosition = new Position(Console.WindowWidth / 2 - 10, Console.WindowHeight - 1);

            InitialBallPosition = new Position(PlayerPosition.X, PlayerPosition.Y - 10);

            IsMovingUp = false;
            IsMovingRight = true;
            BallPosition = new Position(PlayerPosition.X, PlayerPosition.Y - 10);
            InitialBulletPosition = new Position((PlayerPadding + PlayerPosition.X) / 2, PlayerPosition.Y - 1);
            int maxBoard = Math.Max(Console.WindowWidth, Console.WindowHeight);
            Bricks = new int[maxBoard, maxBoard];
            BallsLeft = int.MaxValue;
            Score = 0;
            SpecialItemsPositions = new List<Position>();
            Speed = 50;
            ShieldDuration = 45;
            BulletsAmmo = 45;
        }

        #region DrawingFunctions
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
            for (int i = PlayerPosition.X; i < PlayerPosition.X + PlayerPadding; i++)
                DrawFigure(i, PlayerPosition.Y, '=');
        }

        static void ClearPlayer()
        {
            for (int i = PlayerPosition.X; i < PlayerPosition.X + PlayerPadding; i++)
                DrawFigure(i, PlayerPosition.Y, ' ');
        }

        static void DrawBall()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            DrawFigure(BallPosition, '@');
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
        #endregion

        static void ReadUserInput()
        {
            if(Console.KeyAvailable)
            {
                ConsoleKeyInfo userInput = Console.ReadKey();
                
                if (userInput.Key == ConsoleKey.LeftArrow && PlayerPosition.X > 0)
                {
                    // remove the tale
                    Console.SetCursorPosition(PlayerPosition.X + PlayerPadding, PlayerPosition.Y);
                    Console.Write(" ");
                    // move the player to the left
                    PlayerPosition.X--;
                }

                if (userInput.Key == ConsoleKey.RightArrow && 
                    PlayerPosition.X + PlayerPadding < Console.WindowWidth - 2)
                {
                    // remove the tale
                    Console.SetCursorPosition(PlayerPosition.X, PlayerPosition.Y);
                    Console.Write(" ");
                    // move the player to the right
                    PlayerPosition.X++;
                }

                if (userInput.Key == ConsoleKey.Spacebar && BulletsAmmo > 0)
                {
                    /*
                     * decrease the bulletAmmo
                     * set the initialPosition and added to the gameBullets
                     */

                    BulletsAmmo--;
                    InitialBulletPosition.X = PlayerPosition.X;
                    Bullets.Add(new Position(InitialBulletPosition.X, InitialBulletPosition.Y));
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

        static void CheckCharacter(Position suspect)
        {
            for (int i = 0; i < SpecialItemsPositions.Count; i++)
            {
                Position item = SpecialItemsPositions[i];

                if (item.X == suspect.X && item.Y == suspect.Y)
                {
                    if (item.Symbol == '>')
                        Speed /= SpeedIncreaser;

                    else if (item.Symbol == '<')
                        Speed *= SpeedDecreaseer;

                    else if (item.Symbol == '#')
                    {
                        if (PlayerPosition.X + (PlayerPadding * 3) >= Console.WindowWidth - 2)
                        {
                            ClearPlayer();
                            PlayerPosition.X = 0;
                        }

                        PlayerPadding *= (int)PaddingMultiplier;
                    }
                    else if (item.Symbol == '*')
                        Score = (int)(Score * 1.5);

                    else if (item.Symbol == (char)128)
                    {
                        HasShield = true;
                        PlayerPosition.Y = Console.WindowHeight - 2;
                        for (int x = 0; x < Console.WindowWidth - 1; x++)
                            DrawFigure(x, PlayerPosition.Y + 1, '-');
                        ShieldDuration = InitialShieldDuration;
                    }

                    else if (item.Symbol == (char)158)
                        BulletsAmmo += 3;

                    SpecialItemsPositions.Remove(item);
                    return;
                }
            }
        }

        static void BallTopLogic() // when the ball approaches from top
        {
            if (IsMovingUp)
                return;

            // searching for collision with bricks
            Position rightUp = new Position(BallPosition.X + 1, BallPosition.Y - 1);
            Position up = new Position(BallPosition.X, BallPosition.Y - 1);
            Position leftUp = new Position(BallPosition.X - 1, BallPosition.Y - 1);

            if (ContainsSpecialItem(rightUp))
                CheckCharacter(rightUp);

            if (ContainsSpecialItem(up))
                CheckCharacter(up);

            if (ContainsSpecialItem(leftUp))
                CheckCharacter(leftUp);

            if (Bricks[BallPosition.X + 1, BallPosition.Y - 1] != 0)
            {
                DrawFigure(BallPosition.X + 1, BallPosition.Y - 1, ' ');
                Bricks[BallPosition.X + 1, BallPosition.Y - 1] = 0;
                Score += 25;
            }

            if (Bricks[BallPosition.X, BallPosition.Y - 1] != 0)
            {
                DrawFigure(BallPosition.X, BallPosition.Y - 1, ' ');
                Bricks[BallPosition.X, BallPosition.Y - 1] = 0;
                Score += 25;
            }

            if (Bricks[BallPosition.X - 1, BallPosition.Y - 1] != 0)
            {
                DrawFigure(BallPosition.X - 1, BallPosition.Y - 1, ' ');
                Bricks[BallPosition.X - 1, BallPosition.Y - 1] = 0;
                Score += 25;
            }
        }

        static void BallBottomLogic() // when the ball approaches from the bot
        {
            // searching for collision with bricks
            if (!IsMovingUp)
                return;

            Position down = new Position(BallPosition.X, BallPosition.Y + 1);
            Position downLeft = new Position(BallPosition.X - 1, BallPosition.Y + 1);
            Position downRight = new Position(BallPosition.X + 1, BallPosition.Y + 1);

            if (ContainsSpecialItem(down))
                CheckCharacter(down);

            if (ContainsSpecialItem(downRight))
                CheckCharacter(downRight);

            if (ContainsSpecialItem(downLeft))
                CheckCharacter(downLeft);

            if (Bricks[BallPosition.X + 1, BallPosition.Y + 1] != 0)
            {
                DrawFigure(BallPosition.X + 1, BallPosition.Y + 1, ' ');
                Bricks[BallPosition.X + 1, BallPosition.Y + 1] = 0;
                Score += 25;
            }

            if (Bricks[BallPosition.X, BallPosition.Y + 1] != 0)
            {
                DrawFigure(BallPosition.X, BallPosition.Y + 1, ' ');
                Bricks[BallPosition.X, BallPosition.Y + 1] = 0;
                Score += 25;
            }

            if (Bricks[BallPosition.X - 1, BallPosition.Y + 1] != 0)
            {
                DrawFigure(BallPosition.X - 1, BallPosition.Y + 1, ' ');
                Bricks[BallPosition.X - 1, BallPosition.Y+ 1] = 0;
                Score += 25;
            }
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
                    DrawFigure(x, PlayerPosition.Y + 1, ' ');
                    DrawFigure(x, PlayerPosition.Y, ' ');
                }
                PlayerPosition.Y = Console.WindowHeight - 1;
            }
        }

        #region MovingFunctions
        static void MoveBall()
        {
            /*
             *if the time passed is too short the ball cannot move in this way the speed of the player differentiate 
             * from the ball's speed
             */
            if ((DateTime.Now - BallLastMoveDate).TotalSeconds < BallMoveRate)
                return;

            BallLastMoveDate = DateTime.Now;
            Console.SetCursorPosition(BallPosition.X, BallPosition.Y);
            Console.Write(" ");

            if (IsMovingUp)
                BallPosition.Y--;
            else
                BallPosition.Y++;

            if (IsMovingRight)
                BallPosition.X++;
            else
                BallPosition.X--;

            if (BallPosition.X == 0)
                IsMovingRight = true;

            if (BallPosition.X == Console.WindowWidth - 2)
                IsMovingRight = false;

            if (BallPosition.Y == 0)
                IsMovingUp = false;

            if (BallPosition.Y == Console.WindowHeight - 1 && !HasShield) // draw a new ball
            {
                BallPosition.X = InitialBallPosition.X;
                BallPosition.Y = InitialBallPosition.Y;
                ClearPlayer();
                PlayerPadding = InitialPlayerPadding;
                DrawBall();
            }

            else if (BallPosition.Y == Console.WindowHeight - 1 && HasShield)
                IsMovingUp = true;

            if (BallPosition.Y + 1 == PlayerPosition.Y)
            {
                if (BallPosition.X >= PlayerPosition.X - 1 &&
                    BallPosition.X <= PlayerPosition.X + PlayerPadding) // colliding with player
                {
                    // random new trajectory inorder to fight the limit in the console I have seen
                    
                    Random random = new Random();
                    IsMovingUp = true;
                    int xShift = random.Next(1, 100);
                    if (xShift < 50)
                        BallPosition.X += (xShift % 10) % 5;
                    else
                        BallPosition.X -= (xShift % 10) % 5;

                    if (BallPosition.X <= 0)
                        BallPosition.X += 1;
                    else if (BallPosition.X >= Console.WindowWidth - 1)
                        BallPosition.X = Console.WindowWidth - 3;
                }
            }

            DrawBall();

            if (BallPosition.Y <= Console.WindowHeight - 2 &&
                BallPosition.X <= Console.WindowWidth - 2 && BallPosition.X > 0 && BallPosition.Y > 0)
            {
                BallBottomLogic();
                BallTopLogic();
            }
        }

        static void MoveBullet()
        {
            if (BulletsAmmo < 0)
                return;

            for (int i = 0; i < Bullets.Count; i++)
            {
                if (Bricks[Bullets[i].X, Bullets[i].Y] != 0)
                {
                    DrawFigure(Bullets[i].X, Bullets[i].Y, ' ');
                    Bricks[Bullets[i].X, Bullets[i].Y] = 0;
                    Score += 25;
                }

                if (ContainsSpecialItem(new Position(Bullets[i].X, Bullets[i].Y)))
                    CheckCharacter(new Position(Bullets[i].X, Bullets[i].Y));

                DrawFigure(Bullets[i].X, Bullets[i].Y, ' ');
                Bullets[i].Y--;

                if (Bullets[i].Y == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Bullets.Remove(Bullets[i]);
                    i--;
                    continue;
                }

                DrawFigure(Bullets[i].X, Bullets[i].Y, '|');
            }
        }
        #endregion

        static void WriteScore()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write("Score: {0}", Score);
            Console.Write(" Balls left: {0} ", BallsLeft);
            Console.Write("Bullets: {0}", BulletsAmmo);
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
                Console.SetCursorPosition(10, PlayerPosition.Y - 5);
                Console.WriteLine("You have failed with a tiny score of {0}", Score);
            }
        }
    }
}
