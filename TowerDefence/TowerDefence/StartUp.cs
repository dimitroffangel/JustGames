using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace TowerDefence
{
    class StartUp
    {

        static void Main()
        {

            Console.CursorVisible = false;
            SetUpVariables variables = new SetUpVariables();
            GameActions gameActions = new GameActions(ref variables);
            ClassConfig config = new ClassConfig(ref variables);
            config.ReadImportantInformation();
            gameActions.LoadStartingMenu(); 
        }
    }
}
