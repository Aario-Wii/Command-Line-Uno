/*Potential Updates: 
Add settings to add rules like Seven-O and add a user-defined custom deck, custom launchers?
Add Uno No Mercy, Uno Flip, Uno Console Edition
Add Launcher Override in
Make computers only swap if they don't have the least amount of cards
Add new sound effects for new cards if they need it
Fix Clear Lines
Add ? descriptions for everything to explain what it does*/

using UnoComponents;
using MethodLibrary;
public class Program
{
    public static void Main()
    {
        UnoManager Manager = new UnoManager();
        List<IPlayer> Players = new List<IPlayer>();
        IDeck Deck;
        Discard Discard;
        bool fullgameornot;
        bool multiplayer;
        bool started;
        bool playing = true;
        bool swapallowed = true;
        bool shuffleallowed = false;
        bool discardallowed = false;
        bool stackingon = true;
        bool launcheroverride = false;
        SoundManager sound = new SoundManager();
        ILauncher launcher = new AttackLauncher();
        string versionnum = "v1.0.0";
        Console.Title = "Command Line Uno";
        
        sound.InitializeBGM();
        sound.PlayBGM(OST.Title);
        while (playing)
        {
            if (!Console.IsOutputRedirected)
            {
                Console.Clear();
            }
            Console.WriteLine(versionnum + "\n");
            Console.WriteLine("Welcome to Command Line Uno!");
            Console.WriteLine("What would you like to do?");
            Console.WriteLine($"{"1. Start A Game",-15} | {"2. Settings",-11} | {"3. Exit the Game",-20}");
            switch (Read.String().ToLower())
            {
                case "1":
                case "start a game":
                    sound.PlaySFX(SoundFX.Select);
                    Manager.InitializeGame(ref Players, out Deck, out Discard, out fullgameornot, out multiplayer, out started, swapallowed, shuffleallowed, discardallowed, sound, launcheroverride, launcher);

                    if (started)
                    {
                        sound.PlayBGM(OST.Game);
                        Manager.Game(Players, Discard, Deck, sound, stackingon, fullgameornot, multiplayer);
                        sound.PlayBGM(OST.Title);
                    }
                    break;
                case "2":
                case "settings":
                    sound.PlaySFX(SoundFX.Select);
                    if (!Console.IsOutputRedirected)
                    {
                        Console.Clear();
                    }
                    sound.PlayBGM(OST.Settings);
                    if (sound is ISoundPlayer player)
                    {
                        Manager.Settings(ref swapallowed, ref shuffleallowed, ref discardallowed, ref stackingon, ref launcheroverride, ref player);
                    }
                    sound.PlayBGM(OST.Title);
                    break;
                case "3":
                case "exit the game":
                    sound.PlaySFX(SoundFX.Select);
                    playing = false;
                    break;
                default:
                    sound.PlaySFX(SoundFX.Error);
                    Console.WriteLine("Please input the number corresponding to your operation");
                    break;

            }
        }
    }
}