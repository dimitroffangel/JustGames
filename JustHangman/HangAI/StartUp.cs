using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HangAI
{
    class StartUp
    {
        static int LivesLeft = 11;

        static bool FindPattern(string a, string b)
        {
            int[,] countTable = new int[a.Length+1, b.Length+1];
            char[,] roadSave = new char[a.Length+1, b.Length+1];

            // set to null the first row and each first col
            // cuz the array starts from the 1 not from 0

            for(int row = 1; row <= a.Length; row++)
            {
                roadSave[row,0] = '0';
                countTable[row,0] = 0;
            }

            for(int col = 0; col <= b.Length; col++)
            {
                roadSave[0,col] = '0';
                countTable[0, col] = 0;
            }

            for(int i = 1; i <= a.Length; i++)
            {
                for(int j = 1; j <= b.Length; j++)
                {
                    if (a[i - 1] == b[j - 1]) // if the letters are the same -> roadSave[...] = '\' ; counter++
                    {
                        roadSave[i, j] = '\\';
                        countTable[i, j] = countTable[i - 1, j - 1] + 1;
                    }

                    else if (countTable[i - 1, j] > countTable[i, j - 1]) // save the value from above 
                    {
                        roadSave[i, j] = '^';
                        countTable[i, j] = countTable[i - 1, j];
                    }

                    else // save the value from left
                    {
                        roadSave[i, j] = '<';
                        countTable[i, j] = countTable[i, j - 1];
                    }
                }
            }

            // now take the pattern from the roadSave table

            int tableRow = a.Length;
            int tableCol = b.Length;
            string foundPattern = "";

            while(tableRow != 0 && tableCol != 0)
            {
                if (roadSave[tableRow, tableCol] == '\\')
                {
                    foundPattern += a[tableRow - 1];
                    tableRow--;
                    tableCol--;
                }

                else if (roadSave[tableRow, tableCol] == '^')
                    tableRow--;
                else if (roadSave[tableRow, tableCol] == '<')
                    tableCol--;
            }

            if (foundPattern.Length != b.Length)
                return false;

            // reverse the pattern
            for(int i = foundPattern.Length-1, j = 0; i >= 0; i--, j++)
            {
                if (foundPattern[i] != b[j])
                    return false;
            }

            return true;
        }

        static void FindWord(string searchedWord, IOrderedEnumerable<KeyValuePair<char, int>> letterFrequency, Dictionary<int, List<string>> dictionary)
        {
            SortedDictionary<int, char> lettersOrder = new SortedDictionary<int, char>();
            List<string> dictionaryWords = new List<string>();
            List<string> usableWords = new List<string>();
            List<char> foundLetters = new List<char>();
            List<char> ilicitLetters = new List<char>();
            Dictionary<char, int> leftLetters = new Dictionary<char, int>();
            char chosenLetter = ' ';
            int lettersCount = searchedWord.Length; // the letters on the word
            
            // first add all words to dictionaryWords so that the usable words may be updated on the still usable
            foreach(var word in dictionary[lettersCount])
                dictionaryWords.Add(word);

            /*everytime a deduction is made if it is right then add to table only the words who has a pattern what you have already 
             * found
            */


            while(LivesLeft > 0 && lettersOrder.Count != searchedWord.Length && dictionaryWords.Count > 0)
            {
                usableWords = new List<string>();
                leftLetters = new Dictionary<char, int>();

                chosenLetter = letterFrequency.First().Key;

                if (searchedWord.Contains(chosenLetter))
                {
                    for (int i = 0; i < lettersCount; i++)
                    {
                        if (searchedWord[i] == chosenLetter)
                        {
                            // add the letter to the place it holds in the searched word
                            lettersOrder.Add(i, chosenLetter);
                            foundLetters.Add(chosenLetter); // save all the found letters
                        }
                    }
                }

                else
                {
                    ilicitLetters.Add(chosenLetter);
                    LivesLeft--;
                }

                // find usable words
                foreach (var word in dictionaryWords)
                {
                    bool isUsable = true;

                    // check if the chosen word contains a letter which cannot be obligated with the conscript word
                    foreach (var letter in word)
                    {
                        if (ilicitLetters.Contains(letter))
                        {
                            isUsable = false;
                            break;
                        }
                    }

                    if (isUsable && foundLetters.Count > 0) // if the word is still usable examine her letters to be confident it has the found letters
                    {
                        string curWord = ""; // assemble a pattern from the foundLetters
                        foreach (var letter in lettersOrder)
                            curWord += letter.Value;

                        if(!FindPattern(word, curWord))
                            isUsable = false;
                    }

                    if (isUsable)
                        usableWords.Add(word);
                }

                // find the left frequency with the equivalent method
                foreach (var word in usableWords)
                {
                    foreach (var letter in word)
                    {
                        if (leftLetters.ContainsKey(letter))
                            leftLetters[letter]++;

                        else if(!foundLetters.Contains(letter)) // we do not want to use the same letter again
                            leftLetters.Add(letter, 1);
                    }
                }

                // if foundLetters.count is near the letter count in the wanted word
                // try guessing it let's say for start that if only a letter remains try to guess it

                dictionaryWords = usableWords;
                letterFrequency = leftLetters.OrderByDescending(pair => pair.Value);
            }
            
            if (LivesLeft > 0 && dictionaryWords.Count > 0)
            {
                Console.WriteLine("Found the word");
                Console.Write("the word is: ");
                
                foreach(var letter in lettersOrder)
                {
                    Console.Write(letter.Value);
                }

                Console.WriteLine(" Lives left " + LivesLeft);
            }

            else
                Console.WriteLine("I failed");
        }

        static void Main()
        {
            string searchedWord = Console.ReadLine().ToLower();
            List<char> wordLetters = new List<char>();

            for (int i = 0; i < searchedWord.Length; i++)
            {
                if (!wordLetters.Contains(searchedWord[i]))
                    wordLetters.Add(searchedWord[i]);
            }

            string[] lines = File.ReadAllLines("./test.txt");
            
            Dictionary<int, List<string>> dictionary = new Dictionary<int, List<string>>();
            Dictionary<int, Dictionary<char, int>> letterFrequency = new Dictionary<int, Dictionary<char, int>>();

            List<int> arr = new List<int>(){ 42, 23, 25, -6, 100, 5 };
            
            foreach(var line in lines)
            {
                // if the size of the word is already in the dictionary add only the word
                if (dictionary.ContainsKey(line.Length))
                {
                    dictionary[line.Length].Add(line.ToLower());
                    continue;
                }

                // else just add a new table with such length
                dictionary.Add(line.Length, new List<string>());
                letterFrequency.Add(line.Length, new Dictionary<char, int>());
                dictionary[line.Length].Add(line);
            }
            
            /* for every word length table get the letter's frequency
                everytime a letter is chosen a recalculation must be called on the same table
                and so forth 'till the word is chosen or the tries have expired
             */

            /*
                when frequency is found you need to add all other letters in order to try to find the word
             */

            foreach(var wordLength in dictionary)
            { 
                foreach(var word in wordLength.Value)
                {
                    foreach(var letter in word)
                    {
                        if (letterFrequency[wordLength.Key].ContainsKey(letter))
                        {
                            letterFrequency[wordLength.Key][letter]++;
                            continue;
                        }
                        letterFrequency[wordLength.Key].Add(letter, 1);
                    }
                }

                for(int j = 97; j <= 122; j++)
                {
                    if (letterFrequency[wordLength.Key].ContainsKey((char)(j)))
                        continue;

                    letterFrequency[wordLength.Key].Add((char)(j), 0);
                }
            }

            // now start ordering, however, you need to order only the table with the same word lenth
            // else you will waste dozens of hours only to order the damn table

            bool canFindWord = false;

            foreach(var wordLength in dictionary)
            {
                if(wordLength.Key == searchedWord.Length)
                {
                    canFindWord = true;
                    break;
                }
            }

            if (!canFindWord)
                Console.WriteLine("The dictionary cannot find such a word length, hence the AI is not abled to solve the puzzle");

            else
            {
                var neededFrequency = letterFrequency[searchedWord.Length].
                                    OrderByDescending(pair => pair.Value);
                neededFrequency = neededFrequency.OrderByDescending(pair => pair.Value);

                FindWord(searchedWord, neededFrequency, dictionary);
            }
            Console.WriteLine("Done");
        }
    }
}
