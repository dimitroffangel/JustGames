using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JustHangMan
{
    class HangMan
    {
        static int livesLeft = 11;
        const int victimHeight = 4;
        const int victimWidth = 1;
        static int initialX = (Console.WindowWidth - 1) / 2;
        static int positionX = (Console.WindowWidth-1)/2;
        static int maxX = Console.WindowWidth-1;
        static int initialY = Console.WindowHeight / 2;
        static int positionY = Console.WindowHeight / 2;
        static bool isWritten = false;

        static void DrawNext(int livesLeft)
        {
            if (livesLeft <= 11 && livesLeft >= 6)
                DrawRod();

            else if (livesLeft < 6 && livesLeft > 4)
                DrawRope();

            else if (livesLeft == 4)
                DrawHead();

            else if (livesLeft == 3)
                DrawLeftHand();

            else if (livesLeft == 2)
                DrawRightHand();

            else if (livesLeft == 1)
                DrawLeftLeg();

            else if (livesLeft == 0)
                DrawRightLeg();
        }

        static void DrawRod()
        {
            Console.SetCursorPosition(positionX++, positionY);
            Console.Write("_");

            if (livesLeft == 6)
                positionY++;
        }

        static void DrawRope()
        {
            Console.SetCursorPosition(positionX, positionY++);
            Console.Write("!");
        }

        static void DrawingSurface()
        {
            for (int i = 0; i < 4; i++)
                Console.Write("_");
        }

        static void DrawHead()
        {
            Console.SetCursorPosition(positionX, positionY);
            Console.Write("O");

            for (int i = 0; i < 2; i++)
            {
                Console.SetCursorPosition(positionX, ++positionY);
                Console.Write("!");
            }
        }

        static void DrawLeftHand()
        {
            positionY -= 1;
            positionX -= 1;
            Console.SetCursorPosition(positionX, positionY);
            Console.Write("\\");
        }

        static void DrawRightHand()
        {
            positionX += 2;
            Console.SetCursorPosition(positionX, positionY);
            Console.Write("/");
        }

        static void DrawLeftLeg()
        {
            positionX -= 2;
            positionY += 2;
            Console.SetCursorPosition(positionX, positionY);
            Console.Write("/");
        }

        static void DrawRightLeg()
        {
            positionX += 2;
            Console.SetCursorPosition(positionX, positionY);
            Console.Write("\\");
        }

        static void DrawingVictim()
        {
            Console.SetCursorPosition(positionX, Console.WindowHeight / 2);
            DrawingSurface();
            DrawingSurface();

            for (int i = 0; i < 7; i++)
            {
                Console.SetCursorPosition(positionX,  positionY-- );
                Console.Write("!");
            }

            // to set the rod right
            positionX += 1;
            positionY += 1;
            //positionY = Console.CursorTop;
            //positionX = Console.CursorLeft;
            //for (int i = 1; i <= 2; i++)
            //{
            //    Console.SetCursorPosition(positionX, ++positionY);
            //    Console.Write("!");
            //}
        }

        static void CutRoap()
        {
            for(int i = positionY; i > positionY - victimHeight; i--)
            {
                Console.SetCursorPosition(positionX - victimWidth-1, i);

                for (int j = 0; j <= victimWidth+1; j++)
                    Console.Write(" ");
            }
        }

        static void ReleaseVictim()
        {
            positionX = initialX;
            positionY = initialY + victimHeight;

            DrawHead();
            DrawLeftHand();
            DrawRightHand();
            DrawLeftLeg();
            DrawRightLeg();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\t You were set free");
            Console.ForegroundColor = ConsoleColor.White;
        }
        
        static Dictionary<char, int> SearchWordLetters(string word)
        {
            Dictionary<char, int> table = new Dictionary<char, int>();

            foreach (var letter in word)
            {
                if (table.ContainsKey(letter))
                    table[letter] += 1;

                else
                    table.Add(letter, 1);
            }

            return table;
        }

        static void Main(string[] args)
        {
            List<string> words = new List<string>()
            {
                "promise",
                "godly",
                "creature",
                "sticky",
                "soda",
                "unlock",
                "playground",
                "strap",
                "skip",
                "bike",
                "weary",
                "muddle",
                "Make love not war",
                "Y o u    c a n    m a k e    a    q u i c k    b u c k    O N L Y    " +
                "i f    y o u    p r e s e n t    a    c o m p e l l i n g    o f f e r    t o    p e o p l e    w h o   " +
                " w a n t    t o    m a k e    a    q u i c k    b u c k"
            };

            Random random = new Random();
            string input = words.ElementAt(random.Next(0, words.Count));
            List<char> missedCharacters = new List<char>();
            List<char> foundCharacters = new List<char>();
            Dictionary<char, int> searchWordLetters = SearchWordLetters(input);
            int lettersCount = 0;

            for (int i = 0; i < input.Length; i++)
            {
                Console.Write("_ ");
            }
            
            DrawingVictim();

            while (true)
            {
                Console.SetCursorPosition(0, 2);
                Console.Write("Enter a letter: ");
                char readLetter = char.Parse(Console.ReadLine());
                bool isValid = false;

                for(int i = 0; i < input.Length; i++)
                {
                    if (readLetter == Char.ToLower(input[i]) || readLetter == Char.ToUpper(input[i]))
                    {
                        Console.SetCursorPosition(i + i, 0);
                        Console.Write(input[i]);
                        isValid = true;

                        readLetter = input[i];
                        break;
                    }
                }

                if (!isValid)
                {
                    livesLeft--;
                    missedCharacters.Add(readLetter);

                    if (livesLeft < 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("You have lost. The searched word was " + input);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        return;
                    }

                    DrawNext(livesLeft);
                }

                else
                {
                    if (foundCharacters.Count > 0)
                    {
                        char curLetter = foundCharacters[0];
                        for (int i = 0; i < foundCharacters.Count; i++)
                        {
                            curLetter = foundCharacters[i];

                            if (curLetter == readLetter)
                                break;
                        }

                        if (curLetter == readLetter)
                        {
                            Console.SetCursorPosition(0, 3);
                            Console.Write("Already written");
                           // Thread.Sleep(5000);
                            Console.SetCursorPosition(0, 3);
                            Console.Write("                ");
                                           
                            continue;
                        }
                    }

                    lettersCount++;
                    foundCharacters.Add(readLetter);

                    if (lettersCount == searchWordLetters.Keys.Count)
                    {
                        Console.Clear();
                        Console.WriteLine("Well done! You have accomplished your task with only {0} body parts remaining", livesLeft);
                        CutRoap();
                        ReleaseVictim();
                        return;
                    }
                }

                Console.SetCursorPosition(0, 4);
                Console.Write("Missed symbols: ");
                foreach(var miss in missedCharacters)
                    Console.Write("{0}, " ,miss);
            }
        }
    }
}
