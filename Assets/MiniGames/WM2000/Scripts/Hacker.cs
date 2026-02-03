using UnityEngine;

using Scripts;

public class Hacker : MonoBehaviour
{

    float currentLevel;
    ScreenEnum currentScreen = ScreenEnum.MainMenu;
    string currentWord;
    Leveling stage;

    void Start()
    {
        ShowMainMenu("Yoga");
    }

    void ShowMainMenu(string name)
    {
        currentScreen = ScreenEnum.MainMenu;
        Terminal.ClearScreen();
        Terminal.WriteLine($"Hello {name}");
        Terminal.WriteLine("What would you like to decode into?");
        Terminal.WriteLine("");
        Terminal.WriteLine("Level 1: Wanderer world");
        Terminal.WriteLine("Level 2: Alchemist's beaker");
        Terminal.WriteLine("Level 3: Black Witch crystal ball");
        Terminal.WriteLine("");
        Terminal.WriteLine("Type your level number/'menu' to menu:");
    }

    void OnUserInput(string input)
    {
        if (input == "menu")
        {
            ShowMainMenu("Yoga");
        }
        else if (currentScreen == ScreenEnum.MainMenu)
        {
            SelectLevel(input);
        }
        else if (currentScreen == ScreenEnum.Decoder)
        {
            checkWord(input);
        }

    }

    void SelectLevel(string input)
    {
        bool canInverted = float.TryParse(input, out float parsed);
        bool isValidLevel = (parsed > 0 && parsed < 4);
        if (canInverted && isValidLevel)
        {
            currentLevel = parsed;
            ShowGameScreen(currentLevel);
        }
        else
        {
            Terminal.WriteLine("Wrong input!");
        }
    }

    void ShowGameScreen(float level)
    {
        currentScreen = ScreenEnum.Decoder;
        instantiateLevelAndRandomizeWord(level);

        Terminal.ClearScreen();
        Terminal.WriteLine($"Welcome to level {stage.level} ({stage.name})");
        Terminal.WriteLine($"Enter password : (hint: {currentWord.Anagram()})");
    }

    void checkWord(string input)
    {
        if (input == currentWord)
        {
            DisplayFinishScreen();
        }
        else
        {
            ShowGameScreen(currentLevel);
        }
    }

    void instantiateLevelAndRandomizeWord(float level)
    {
        Leveling levelInstance = new Leveling(level);
        levelInstance.DetermineLevelName(level);
        stage = levelInstance;
        currentWord = stage.words[Random.Range(0, stage.words.Count)];
        print(currentWord);
    }

    void DisplayFinishScreen()
    {
        currentScreen = ScreenEnum.Finish;
        Terminal.ClearScreen();
        switch (currentLevel)
        {
            case 1:
                Terminal.WriteLine("MAN OF FOREST!");
                break;
            case 2:
                Terminal.WriteLine("MAN FOUND CURE!");
                break;
            case 3:
                Terminal.WriteLine("JOIN THE DARK?!");
                break;
        }
        Terminal.WriteLine(@"
  __
 /  \_________
| 0  ___ ___  /
 \__/   ^   ^

HERE IS THE KEY!
");
        Terminal.WriteLine("Type \"menu\" to return to Main Menu");

    }
}
