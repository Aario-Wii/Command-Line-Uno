# Command Line Uno
Command Line Uno is a port of Uno made for the Command Line using C# and the .NET Framework. It was originally created in the summer of 2025, and was uploaded to Github in the summer of 2026.<br><br>
**This project is not affiliated with or endorsed by Mattel. UNO is a trademark of Mattel Inc.**<br>
> This project uses the NuGet packages [Pastel](https://github.com/silkfire/Pastel "Snazz up your console output!") and [SoundFlow](https://github.com/LSXPrime/SoundFlow "A high-performance, modular audio & MIDI engine for .NET 8+."); their licenses can be found in [THIRD-PARTY-NOTICES.md](https://github.com/Aario-Wii/Command-Line-Uno/blob/main/THIRD-PARTY-NOTICES.md)
## How to Play
To navigate through the application, type the number that corresponds to the choice that you would like to select and press Enter.
### Decks
Currently, Command Line Uno has two decks:
  1. **Uno**
       - The original card game. Uses a simulated deck.
       - [Rules](unorules.com "Original Uno")
  2. **Uno Attack**
       - Uses a simulated Launcher.
       - [Rules](https://www.unorules.com/uno-attack-rules/ "Uno Attack")<br>

### Types of Players
There two categories of players in Command Line Uno: Human Players and COM Players. Human Players are controlled by humans, while COM Players are controlled by the computer.<br>
  **Types of Human Players:**
  1. Normal Player
      - Lets you see all of your cards
      - Best for strategizing
  2. Easy Player
      - Only lets you see the cards that can currently be played
      - Best for beginners.<br>

  **Types of COM Players**
  1. Normal COM Player
       - Plays normally
  2. Aggressive COM Player
       - Plays aggressively
  3. Passive COM Player
       -Plays passively<br>

### Settings
- Launcher Override:
    - Uses a simulated launcher instead of a simulated deck for games that play normal Uno
- Stacking:
    - Lets you "stack" cards of the same type (ex. playing a Green and Red 4 on the same turn)
