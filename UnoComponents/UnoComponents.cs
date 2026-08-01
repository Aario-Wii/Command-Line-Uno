using System;
using MethodLibrary;
using System.Collections.Generic;
using SoundFlow.Components;
using SoundFlow.Abstracts;
using SoundFlow.Structs;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Providers;
using SoundFlow.Abstracts.Devices;
using Pastel;

namespace UnoComponents
{
    public interface IManager
    {
        void InitializeGame(ref List<IPlayer> players, out IDeck deck, out Discard discard, out bool fullgameornot, out bool multiplayer, out bool started, bool swapallowed, bool shuffleallowed, bool discardallowed, ISoundPlayer player, bool launcheroverride, ILauncher overridelauncher);
        BaseCOMPLayer COMCreator(ISoundPlayer player);
        BaseHumanPlayer HumanCreator(ISoundPlayer player);
        BaseDeck DeckSelector(bool swapallowed, bool shuffleallowed, bool discardallowed, ISoundPlayer player, bool launcheroverride);
        void Settings(ref bool swapallowed, ref bool shuffleallowed, ref bool discardallowed, ref bool stackingon, ref bool launcheroverride, ref ISoundPlayer player);
        void CCardsManager(ref bool swapallowed, ref bool shuffleallowed, ref bool discardallowed, ISoundPlayer player);
        void RulesAndOverrides(ISoundPlayer player, ref bool stackingon, ref bool launcheroverride);
        void SoundSettings(ref ISoundPlayer player);
        void Game(List<IPlayer> players, Discard discard, IDeck deck, ISoundPlayer manager, bool fullgameornot, bool multiplayer, bool stackingon);
        void CardFunctions(ref bool reversed, ref int numofskips, ref int numofdraws, Discard discard, bool playedornot, int playercount, ref bool needtochoose, ref bool shuffle);
        void SwapHands(List<IPlayer> players, int player1, int player2);
    }

    public interface ISoundPlayer
    {
        DeviceInfo Device { get; }
        SoundPlayer[] SongList { get; }
        SoundPlayer[] SFXList { get; }
        AudioPlaybackDevice? Music { get; }
        void InitializeBGM();
        void PlayBGM(OST ost);
        void PlaySFX(SoundFX sfx);
        bool BGMON { get; set; }
        bool SFXON { get; set; }
        Mixer BGMixer { get; set; }
        Mixer SFXMixer { get; set; }
        void BGMToggle();
        void SFXToggle();
    }

    public interface ICard
    {
        CardColor Color { get; set; }
        bool IsPlus { get; }
    }
    public interface IDeck
    {
        void Shuffle(int numberofshuffles);
        void StartDeck();
        List<ICard> Deck { get; set; }
        void FillDeck(List<IPlayer> players);
        void OverrideLauncher(ILauncher launcher);
        ILauncher NewLauncher();
        int DangerCount { get; }
    }

    public interface ILaunchDeck : IDeck
    {
        ILauncher Launcher { get; }
    }

    public interface ILauncher
    {
        Random pressdecider { get; }
        int numofpresses { get; }
        int maxpresses { get; }
        int currentpress { get; }
        void CalculatePresses();
        bool Launch(IPlayer player, ILaunchDeck deck);
    }

    public interface IPlayer
    {
        void PlayTurn(Discard discard, IDeck cardpile, out bool playedcard, out string consolemessage, ISoundPlayer sound, bool stackingon);
        bool DrawCard(IDeck deck, bool starting = false);
        void StartHand(IDeck deck);
        bool CardLogic(Discard discard, ICard card);
        void WildChoose(ICard card, ISoundPlayer player);
        void ChoosePlayer(in List<IPlayer> players, out int playernum);
        void DiscardCards(CardColor color);
        void PlayCard(Discard discard, ICard card, ISoundPlayer player);
        void StackCard(List<ICard> stackablecards, ISoundPlayer player, Discard discard);
        public bool CardValidity(Discard discard, ref List<ICard> validcardlist);
        public bool StackAllowed(Discard discard, ref List<ICard> validcardlist);

        List<ICard> Hand { get; set; }
        string PlayerName { get; }
        bool FirstCard { get; set; }
    }


    public enum CardColor
    {
        NotChosen,
        Red,
        Blue,
        Green,
        Yellow,
        Wild
    }

    public class UnoException : Exception
    {
        public UnoException(string message)
        : base(message)
        { }
    }

    public abstract class BaseCard : ICard
    {
        public abstract bool IsPlus { get; }

        protected string colorstring()
        {
            switch (Color)
            {
                case CardColor.Red:
                    return $"{Color}".Pastel(ConsoleColor.Red);
                case CardColor.Blue:
                    return $"{Color}".Pastel(ConsoleColor.Blue);
                case CardColor.Green:
                    return $"{Color}".Pastel(ConsoleColor.Green);
                case CardColor.Yellow:
                    return $"{Color}".Pastel(ConsoleColor.Yellow);
                case CardColor.Wild:
                    return "W".Pastel(ConsoleColor.Red) + "i".Pastel(ConsoleColor.Blue) + "l".Pastel(ConsoleColor.Green) + "d".Pastel(ConsoleColor.Yellow);
            }
            return "invalid";
        }
        public override string ToString()
        {
            return "A " + colorstring() + " ";
        }
        public CardColor Color { get; set; }

        public override bool Equals(object? obj)
        {
            BaseCard? tester = obj as BaseCard;

            if (tester == null)
            {
                return false;
            }

            if (tester?.Color == Color && tester.IsPlus == IsPlus && tester.GetType() == this.GetType())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public BaseCard(CardColor color)
        {
            Color = color;
        }
    }
    public class NumberCard : BaseCard
    {
        public override string ToString()
        {
            return base.ToString() + Number;
        }

        public int Number;
        public override bool IsPlus { get => false; }

        public NumberCard(CardColor color, int number)
        : base(color)
        {
            Number = number;
        }
        public override bool Equals(object? obj)
        {
            NumberCard? tester = obj as NumberCard;

            if (tester == null)
            {
                return base.Equals(obj);
            }

            if (tester?.Color == Color && tester.Number == Number)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
    public class SkipCard : BaseCard
    {
        public override string ToString()
        {
            return base.ToString() + "Skip";
        }
        public override bool IsPlus { get => false; }
        public SkipCard(CardColor color)
        : base(color)
        { }
    }
    public class ReverseCard : BaseCard
    {
        public override string ToString()
        {
            return base.ToString() + "Reverse";
        }
        public override bool IsPlus { get => false; }
        public ReverseCard(CardColor color)
        : base(color)
        { }
    }
    public class PlusTwoCard : BaseCard
    {
        public override string ToString()
        {
            return base.ToString() + "Plus Two";
        }
        public override bool IsPlus { get => false; }
        public PlusTwoCard(CardColor color)
        : base(color)
        { }
    }
    public class PlusFourCard : BaseCard
    {
        public override string ToString()
        {
            return base.ToString() + "Plus Four Card";
        }
        public override bool IsPlus { get => true; }
        public PlusFourCard(CardColor color)
        : base(color)
        {}
    }
    public class WildCard : BaseCard
    {
        private string cstring()
        {
            if (Color == CardColor.Wild)
            {
                return "";
            }
            else
            {
                return "(" + colorstring() + ") ";
            }
        }
        public override string ToString()
        {
            return "A " + cstring() + "W".Pastel(ConsoleColor.Red) + "i".Pastel(ConsoleColor.Blue) + "l".Pastel(ConsoleColor.Green) + "d".Pastel(ConsoleColor.Yellow) + " Card";
        }
        public override bool IsPlus { get => false; }
        public WildCard()
        : base(CardColor.Wild)
        { }
    }
    public class SwapCard : BaseCard
    {
        public override string ToString()
        {
            return base.ToString() + "Swap Hands";
        }
        public override bool IsPlus => false;

        public SwapCard(CardColor color)
        : base(color)
        { }

    }
    public class ShuffleCard : BaseCard
    {
        public override string ToString()
        {
            return base.ToString() + "Shuffle Hands";
        }
        public override bool IsPlus => false;
        public ShuffleCard(CardColor color)
        :base(color)
        { }

    }
    public class DiscardCard : BaseCard
    {
        public override string ToString()
        {
            return base.ToString() + "Discard " + DiscardCount + " Card";
        }
        public int DiscardCount;
        public override bool IsPlus { get => false; }

        public DiscardCard(CardColor color, int numofdiscards)
        : base(color)
        {
            DiscardCount = numofdiscards;
        }
    }
    public class HitTwoCard : PlusTwoCard
    {
        public override string ToString()
        {
            return "A " + colorstring() + " Hit Two Card";
        }
        public HitTwoCard(CardColor color)
        : base(color)
        { }

    }
    public class HitFourCard : PlusFourCard
    {

        public override string ToString()
        {
            return "A " + colorstring() + " Hit Four Card";
        }
        public HitFourCard(CardColor color)
        : base(color)
        { }
    }
    public class DiscardAllCard : BaseCard
    {
        public override string ToString()
        {
            return base.ToString() + "Discard All Card";
        }
        public override bool IsPlus => false;
        public DiscardAllCard(CardColor color)
        : base(color)
        {}
    }
    public class TargetHitTwoCard : BaseCard
    {
        public override bool IsPlus => true;
        public override string ToString()
        {
            return "A " + colorstring() + " Attack-Attack Card";
        }
        public TargetHitTwoCard(CardColor color)
        : base(color)
        { }
    }
    public abstract class BaseDeck : IDeck
    {
        public List<ICard> Deck { get; set; } = new List<ICard>(108);
        public abstract void StartDeck();
        public abstract int DangerCount { get; }
        public void FillDeck(List<IPlayer> players)
        {
            ICard[] backupdeck = new ICard[Deck.Count];
            Deck.CopyTo(0, backupdeck, 0, Deck.Count);
            StartDeck();

            foreach (IPlayer player in players)
            {
                foreach (ICard card in player.Hand)
                {
                    Deck.Remove(card);
                }
            }
            foreach (ICard card in backupdeck)
            {
                Deck.Remove(card);
            }
        }
        public void Shuffle(int numberofshuffles = 1)
        {
            Random shuffler = new Random();
            for (int s = 0; s < numberofshuffles; s++)
            {
                for (int i = Deck.Count - 1; i > 0; i--)
                {
                    int rando = shuffler.Next(i + 1);
                    ICard backup = Deck[i];
                    Deck[i] = Deck[rando];
                    Deck[rando] = backup;
                }
            }

        }
        public void OverrideLauncher(ILauncher launcher)
        {
            potential = launcher;
        }
        ILauncher? potential;
        public ILauncher NewLauncher()
        {
            if (this is ILaunchDeck)
            {
                return potential!;
            }
            else
            {
                return null!;
            }
        }
    }

    public abstract class BaseLaunchDeck : BaseDeck, ILaunchDeck
    {
        public abstract ILauncher Launcher{ get; set; }
        public BaseLaunchDeck()
        {}
        public BaseLaunchDeck(ILauncher launcher)
        {
            Launcher = launcher;
        }
    }

    public class UnoDeck : BaseDeck
    {
        public override int DangerCount => 10;
        bool SwapAllowed;
        bool ShuffleAllowed;
        bool DiscardAllowed;
        public override void StartDeck()
        {
            for (int i = 0; i < 4; i++)
            {
                CardColor color = (CardColor)i + 1;

                Deck.Add(new NumberCard(color, 0));
                for (int h = 0; h < 2; h++)
                {
                    for (int c = 1; c < 10; c++)
                    {
                        Deck.Add(new NumberCard(color, c));
                    }
                    Deck.Add(new ReverseCard(color));
                    Deck.Add(new SkipCard(color));
                    Deck.Add(new PlusTwoCard(color));
                }
                Deck.Add(new PlusFourCard(CardColor.Wild));
                Deck.Add(new WildCard());
            }
            if (SwapAllowed)
            {
                Deck.Add(new SwapCard(CardColor.Wild));
            }
            if (ShuffleAllowed)
            {
                Deck.Add(new ShuffleCard(CardColor.Wild));
            }
            if (DiscardAllowed)
            {
                Deck.Add(new DiscardCard(CardColor.Wild, 5));
            }
        }

        public UnoDeck(bool swapallowed, bool shuffleallowed, bool discardallowed)
        {
            SwapAllowed = swapallowed;
            ShuffleAllowed = shuffleallowed;
            DiscardAllowed = discardallowed;
        }
    }

    public class AttackDeck : BaseLaunchDeck
    {
        public override int DangerCount { get => 30; }
        public override ILauncher Launcher { get; set; } = new AttackLauncher();
        public override void StartDeck()
        {
            for (int i = 0; i < 4; i++)
            {
                CardColor color = (CardColor)i + 1;
                for (int h = 0; h < 2; h++)
                {
                    for (int c = 1; c < 10; c++)
                    {
                        Deck.Add(new NumberCard(color, c));
                    }
                    Deck.Add(new HitTwoCard(color));
                    Deck.Add(new SkipCard(color));
                    Deck.Add(new DiscardAllCard(color));
                }
                Deck.Add(new ReverseCard(color));
                Deck.Add(new WildCard());
                Deck.Add(new TargetHitTwoCard(CardColor.Wild));
            }
            Deck.Add(new HitFourCard(CardColor.Wild));
        }
        public AttackDeck()
        :base()
        {}
        public AttackDeck(AttackLauncher launcher)
        : base(launcher)
        { }
    }

    public abstract class BaseLauncher : ILauncher
    {
        public Random pressdecider { get; } = new Random();
        public int numofpresses { get; protected set; }
        public int currentpress { get; protected set; }
        public abstract int maxpresses { get; protected set; }
        public void CalculatePresses()
        {
            int percent = pressdecider.Next(1, 101);
            if (percent <= 35)
            {
                numofpresses = pressdecider.Next(1, maxpresses + 1);
            }
            if (percent > 35 && percent <= 50)
            {
                numofpresses = pressdecider.Next((int)Math.Round((decimal)maxpresses / 2), maxpresses + 1);
            }
            if (percent > 50 && percent <= 100)
            {
                numofpresses = pressdecider.Next(1, (int)Math.Round((decimal)maxpresses / 2) + 1);
            }
        }

        public bool Launch(IPlayer player, ILaunchDeck deck)
        {
            if (deck.Deck.Count != 0)
            {
                currentpress++;
                if (currentpress >= numofpresses)
                {
                    for (int i = 0; i < numofpresses; i++)
                    {
                        try
                        {
                            player.Hand.Add(deck.Deck[deck.Deck.Count - 1]);
                            deck.Deck.RemoveAt(deck.Deck.Count - 1);
                        }
                        catch
                        {
                            break;
                        }
                    }
                    currentpress = 0;
                    CalculatePresses();
                    return true;
                }
                else
                {
                    Console.Beep();
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public class AttackLauncher : BaseLauncher
    {
        public override int maxpresses { get; protected set; } = 12;
    }
    public class Discard
    {
        public List<ICard> Pile = new List<ICard>(1) {null!};
    }

    public abstract class BasePlayer : IPlayer
    {
        public string PlayerName { get; protected set; } = "";
        public bool FirstCard { get; set; } = true;
        public List<ICard> Hand { get; set; } = new List<ICard>(7);

        public abstract void PlayTurn(Discard discard, IDeck cardpile, out bool playedcard, out string consolemessage, ISoundPlayer player, bool stackingon);
        public abstract void ChoosePlayer(in List<IPlayer> players, out int playernum);
        public abstract void StackCard(List<ICard> stackablecards, ISoundPlayer player, Discard discard);
        public bool CardValidity(Discard discard, ref List<ICard> validcardlist)
        {
            int validcards = 0;
            foreach (ICard card in Hand)
            {
                if (CardLogic(discard, card))
                {
                    validcards++;
                    validcardlist?.Add(card);
                }
            }
            if (validcards == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void DiscardCards(CardColor color)
        {
            for (int i = 0; i < Hand.Count; i++)
            {
                if (Hand[i].Color == color)
                {
                    Hand.RemoveAt(i);
                    i--;
                }
            }
        }
        public abstract void DiscardCards(int numofdiscards);

        public void PlayCard(Discard discard, ICard card, ISoundPlayer player)
        {
            if (card.Color == CardColor.Wild)
            {
               WildChoose(card, player);
            }
            if (FirstCard)
            {
                discard.Pile[discard.Pile.Count() - 1] = card;
                Hand.Remove(card);
                if (card is not DiscardAllCard)
                {
                    FirstCard = false;
                }
            }
            else
            {
                discard.Pile.Add(card);
                Hand.Remove(card);
            }
            if (card is DiscardAllCard && FirstCard)
            {
                DiscardCards(card.Color);
                FirstCard = false;
            }
            if (card is DiscardCard discardcard)
            {
                DiscardCards(discardcard.DiscardCount);
            }
        }
        public bool DrawCard(IDeck deck, bool starting = false)
        {
            while (true)
            {
                if (deck.Deck.Count > 0)
                {
                    if (deck is ILaunchDeck launchDeck && !starting)
                    {
                        if (launchDeck.Launcher.Launch(this, launchDeck))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        Hand.Add(deck.Deck[deck.Deck.Count - 1]);
                        deck.Deck.RemoveAt(deck.Deck.Count - 1);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        public void StartHand(IDeck deck)
        {
            for (int i = 0; i < 7; i++)
            {
                DrawCard(deck, true);
            }
        }

        public abstract void WildChoose(ICard card, ISoundPlayer player);

        public bool StackAllowed(Discard discard, ref List<ICard> stacklist)
        {
            stacklist.Clear();
            foreach (ICard card in Hand)
            {
                if (card.GetType() == discard.Pile[discard.Pile.Count - 1].GetType() && card is not NumberCard)
                {
                    stacklist.Add(card);
                }
                else
                {
                    if (card is NumberCard card1 && discard.Pile[discard.Pile.Count - 1] is NumberCard card2 && card1.Number == card2.Number)
                    {
                        stacklist.Add(card);
                    }
                }
            }
            if (stacklist.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CardLogic(Discard discard, ICard card)
        {
            if (card.Color == CardColor.Wild || card.Color == discard.Pile[discard.Pile.Count() - 1].Color || card.GetType() == discard.Pile[discard.Pile.Count() - 1].GetType() && card is not NumberCard)
            {
                return true;
            }
            else
            {
                if (card is NumberCard && discard.Pile[discard.Pile.Count() - 1] is NumberCard)
                {
                    NumberCard tempnum = (NumberCard)card;
                    NumberCard discnum = (NumberCard)discard.Pile[discard.Pile.Count() - 1];
                    if (tempnum.Number == discnum.Number)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        protected string drawmess(int beforedraw, IDeck deck)
        {
            string fronthalf;
            string backhalf;
            if (deck is ILaunchDeck)
            {
                fronthalf = "\n" + ToString() + " pressed the Launcher button once and has picked up ";
            }
            else
            {
                fronthalf = "\n" + ToString() + " picked up ";
            }
            if (Hand.Count - beforedraw == 1 || Hand.Count - beforedraw == 0)
            {
                backhalf = "a card";
            }
            else
            {
                backhalf = Hand.Count - beforedraw + " cards";
            }
            return fronthalf + backhalf + "\n";
        }


        public BasePlayer(string name)
        {
            PlayerName = name;
        }
        public override string ToString()
        {
            return "The Base Player object";
        }
    }

    public abstract class BaseHumanPlayer : BasePlayer
    {
        public abstract bool UseHandorvalid();

        public override void ChoosePlayer(in List<IPlayer> players, out int playernum)
        {
            while (true)
            {
                Console.WriteLine("Which player would you like to choose? (Only input the number)");
                int loopnum = -1;
                int loopcount;
                for (loopcount = 0; loopcount < players.Count - 1; loopcount++)
                {
                    loopnum++;
                    if (players[loopnum] != this)
                    {
                        Console.WriteLine(loopcount + 1 + ". " + players[loopnum].ToString() + ": Has " + players[loopnum].Hand.Count + " Cards");
                    }
                    else
                    {
                        loopcount--;
                    }
                }
                int choosenum = -1;
                if (Read.Int() is int inc && inc < players.IndexOf(this) + 1)
                {
                    choosenum = inc - 1;
                }
                else
                {
                    choosenum = inc - (loopnum - loopcount);
                }
                if (choosenum >= players.Count || choosenum < 0)
                {
                    Console.WriteLine("Please input the number of the player you would like to choose");
                }
                else
                {
                    playernum = choosenum;
                    return;
                }
            }
            
        }

        public override void DiscardCards(int numofdiscards)
        {
            int clearnum = Console.CursorTop;
            for (int i = 0; i < numofdiscards; i++)
            {
                int handnum = 0;
                Console.WriteLine("Discards Remaining: " + numofdiscards);
                Console.WriteLine("Which card would you like to discard? (Press the number of the card you want to discard)");
                foreach (ICard card in Hand)
                {
                    handnum++;
                    Console.WriteLine(handnum + ". " + card.ToString());
                }
                int removenum = Read.Int() - 1;
                Hand.RemoveAt(removenum);
                ConsoleUtilities.ClearLines(clearnum);
            }
        }
        
        public override string ToString()
{
    return PlayerName;
}

        public void HumanPlayCard(Discard discard, IDeck cardpile, out bool successornot, List<ICard> validcards, bool HandorValid, ISoundPlayer player, bool stackingon)
        {
            List<ICard> parselist = new List<ICard>(Hand.Count);
            if (HandorValid)
            {
                parselist = Hand;
            }
            else
            {
                parselist = validcards;
            }
            while (true)
            {
                int handnum = 0;
                    Console.WriteLine("This is the card at the top of the Discard Pile: " + discard.Pile[discard.Pile.Count() - 1].ToString() + "\nThese are the cards in your hand.\n");
                foreach (ICard card in parselist)
                {
                    handnum++;
                    Console.WriteLine(handnum + ". " + card.ToString());
                }
                Console.WriteLine("\nWhich card do you want to play? (Please input the number that corresponds to your choice, or press " + (handnum + 1) + " to go back)");
                int HandChoice = Read.Int() - 1;
                if (HandChoice > parselist.Count)
                {
                    player.PlaySFX(SoundFX.Error);
                    Console.WriteLine("Please input the number of the card that you want to play");
                }
                else
                {
                    player.PlaySFX(SoundFX.Select);
                    if (HandChoice == parselist.Count)
                    {
                        successornot = false;
                        return;
                    }
                    else
                    {
                        if (parselist == validcards)
                        {
                            foreach (ICard card in Hand)
                            {
                                if (card == parselist[HandChoice])
                                {
                                    HandChoice = Hand.IndexOf(card);
                                    break;
                                }
                            }
                        }
                        if (CardLogic(discard, Hand[HandChoice]))
                        {
                            PlayCard(discard, Hand[HandChoice], player);
                            while (true)
                            {
                                if (stackingon && StackAllowed(discard, ref validcards))
                                {
                                    if (!StackChoose(player, validcards, discard))
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            successornot = true;
                            return;
                        }
                        else
                        {
                            player.PlaySFX(SoundFX.Error);
                            Console.WriteLine("That card is not playable. Please choose the card with the same symbol");
                        }
                    }
                }
            }
        }
        public void HumanPlayDrawCard(Discard discard, out bool playedornot, ISoundPlayer player, bool stackingon, ref List<ICard> validcardlist)
        {
            validcardlist.Clear();
            while (true)
            {
                Console.WriteLine("You picked up this card: " + Hand[Hand.Count - 1].ToString());
                Console.WriteLine("Do you want to play it?");
                Console.WriteLine("{0,-6} | {1,-5}", "1. Yes", "2. No");
                switch (Read.String().ToLower())
                {
                    case "1":
                    case "yes":
                        player.PlaySFX(SoundFX.Select);
                        PlayCard(discard, Hand[Hand.Count - 1], player);
                        playedornot = true;
                        while (true)
                        {
                            if (StackAllowed(discard, ref validcardlist) && stackingon)
                            {
                                StackChoose(player, validcardlist, discard);
                            }
                            else
                            {
                                return;
                            }
                        }
                    case "2":
                    case "no":
                        player.PlaySFX(SoundFX.Select);
                        playedornot = false;
                        return;
                    default:
                        player.PlaySFX(SoundFX.Error);
                        Console.WriteLine("Please enter the number of the operation you wish to perform");
                        break;
                }
            }
        }

        public bool StackChoose(ISoundPlayer player, List<ICard> stackablecards, Discard discard)
        {
            int clearnum = 0;
            if (!Console.IsOutputRedirected)
            {
                clearnum = Console.CursorTop;
            }
            while (true)
                {
                    Console.WriteLine("\nDo you want to stack another card on top of the one you just played?");
                    Console.WriteLine("{0,-6} | {1,-5}\n", "1. Yes", "2. No");
                    switch (Read.String().ToLower())
                    {
                        case "1":
                        case "yes":
                            player.PlaySFX(SoundFX.Select);
                            StackCard(stackablecards, player, discard);
                            if (!Console.IsOutputRedirected)
                            {
                                ConsoleUtilities.ClearLines(clearnum);
                            }
                            return true;
                        case "2":
                        case "no":
                            player.PlaySFX(SoundFX.Select);
                            if (!Console.IsOutputRedirected)
                            {
                                ConsoleUtilities.ClearLines(clearnum);
                            }
                            return false;
                        default:
                            player.PlaySFX(SoundFX.Error);
                            Console.WriteLine("Please input the number corresponding to your choice");
                            if (!Console.IsOutputRedirected)
                            {
                                ConsoleUtilities.ClearLines(clearnum);
                            }
                            break;
                    }
                }
        }

        public override void StackCard(List<ICard> stackablecards, ISoundPlayer player, Discard discard)
        {
            int handnum = 0;
            Console.WriteLine("\n");
            foreach (ICard card in stackablecards)
            {
                handnum++;
                Console.WriteLine(handnum + ". " + card.ToString());
            }
            Console.WriteLine("\nWhich card do you want to play? (Please input the number that corresponds to your choice, or press " + (handnum + 1) + " to go back)");
            int HandChoice = Read.Int() - 1;
            if (HandChoice >= stackablecards.Count || HandChoice < 0)
            {
                player.PlaySFX(SoundFX.Error);
                Console.WriteLine("Please input the number of the card that you want to play");
            }
            else
            {
                player.PlaySFX(SoundFX.Select);
                foreach (ICard card in Hand)
                {
                    if (card == stackablecards[HandChoice])
                    {
                        HandChoice = Hand.IndexOf(card);
                        break;
                    }
                }
                PlayCard(discard, Hand[HandChoice], player);
            }

        }
        public override void WildChoose(ICard card, ISoundPlayer player)
        {
            while (true)
            {
                Console.WriteLine("What color would you like to choose?");
                Console.WriteLine("{0,-6} | {1,-7} | {2,-8} | {3,-9}", "1. " + "Red".Pastel(ConsoleColor.Red), "2. " + "Blue".Pastel(ConsoleColor.Blue), "3. " + "Green".Pastel(ConsoleColor.Green), "4. " + "Yellow".Pastel(ConsoleColor.Yellow));
                switch (Read.String().ToLower())
                {
                    case "1":
                    case "red":
                        player.PlaySFX(SoundFX.Select);
                        card.Color = CardColor.Red;
                        return;
                    case "2":
                    case "blue":
                        player.PlaySFX(SoundFX.Select);
                        card.Color = CardColor.Blue;
                        return;
                    case "3":
                    case "green":
                        player.PlaySFX(SoundFX.Select);
                        card.Color = CardColor.Green;
                        return;
                    case "4":
                    case "yellow":
                        player.PlaySFX(SoundFX.Select);
                        card.Color = CardColor.Yellow;
                        return;
                    default:
                        player.PlaySFX(SoundFX.Error);
                        Console.WriteLine("Please enter the number of the color you want to select");
                        break;
                }
            }
        }

        public override void PlayTurn(Discard discard, IDeck cardpile, out bool playedornot, out string consolemessage, ISoundPlayer sound, bool stackingon)
        {
            string playstring = "";
            bool success = false;
            List<ICard> validcards = new List<ICard>();
            bool canplay = CardValidity(discard, ref validcards);
            while (!success)
            {
                Console.WriteLine("This is the card at the top of the Discard Pile: " + discard.Pile[discard.Pile.Count() - 1].ToString());
                Console.WriteLine("What would you like to do " + ToString() + "?");
                Console.WriteLine("{0,-14} | {1,-14}", "1. Play a Card", "2. Draw a Card");
                switch (Read.String().ToLower())
                {
                    case "1":
                    case "play a card":
                        if (canplay)
                        {
                            sound.PlaySFX(SoundFX.Select);
                            HumanPlayCard(discard, cardpile, out success, validcards, UseHandorvalid(), sound, stackingon);
                            if (success)
                            {
                                string playmess = "";
                                playedornot = true;
                                foreach (ICard card in discard.Pile)
                                {
                                    playmess +=  "\n" + ToString() + " has played " + card.ToString(); //See if message outputs properly
                                }
                                playmess += "\n";
                                consolemessage = playmess;
                                sound.PlaySFX(SoundFX.CardMove);
                                return;
                            }
                        }
                        else
                        {
                            sound.PlaySFX(SoundFX.Error);
                            Console.WriteLine("You do not have any playable cards in your hand. \nPlease draw a card.");
                        }
                        break;
                    case "2":
                    case "draw a card":
                        int beforedraw = Hand.Count;
                        if (!DrawCard(cardpile))
                        {
                            if (cardpile is ILaunchDeck && cardpile.Deck.Count != 0)
                            {
                                string nomachine = "\n" + ToString() + " pressed the Launcher button once and did not recieve any cards\n";
                                consolemessage = nomachine;
                                playedornot = false;
                                sound.PlaySFX(SoundFX.Beep);
                                return;
                            }
                            else
                            {
                                if (validcards.Count == 0)
                                {
                                    Console.WriteLine("No playable cards have been found. Skipping turn...");
                                    sound.PlaySFX(SoundFX.Error);
                                    string nocardsmess = ToString() + " could not play any cards.";
                                    consolemessage = nocardsmess;
                                    playedornot = false;
                                    return;
                                }
                                else
                                {
                                    Console.WriteLine("Draw Pile is empty. Please play a card");
                                    sound.PlaySFX(SoundFX.Error);
                                }
                            }
                        }
                        else
                        {
                            sound.PlaySFX(SoundFX.CardMove);
                            if (CardLogic(discard, Hand[Hand.Count - 1]) && cardpile is not ILaunchDeck)
                            {
                                string dmess = drawmess(beforedraw, cardpile);
                                HumanPlayDrawCard(discard, out playedornot, sound, stackingon, ref validcards);
                                if (playedornot)
                                {
                                    string playmess = "";
                                    foreach (ICard card in discard.Pile)
                                    {
                                        playmess += "\n" + ToString() + " has played " + card.ToString();
                                    }
                                    playmess += "\n";
                                    playstring += playmess;
                                }
                                consolemessage = dmess + playstring;
                                sound.PlaySFX(SoundFX.CardMove);
                                return;
                            }
                            else
                            {
                                playedornot = false;
                                consolemessage = drawmess(beforedraw, cardpile);
                            }
                            return;
                        }
                        break;
                    default:
                        sound.PlaySFX(SoundFX.Error);
                        Console.WriteLine("Please input the number of the operation you wish to perform");
                        break;
                }
            }
            consolemessage = "";
            playedornot = false;
        }
        public BaseHumanPlayer(string name)
        : base(name)
        { }
    }

    public class NormHumanPlayer : BaseHumanPlayer
    {
        public override bool UseHandorvalid()
        {
            return true;
        }
        public NormHumanPlayer(string name)
        : base(name)
        { }

    }

    public class EasyHumanPlayer : BaseHumanPlayer
    {
        public override bool UseHandorvalid()
        {
            return false;
        }
        public EasyHumanPlayer(string name)
        : base(name)
        { }

    }

    public class BaseCOMPLayer : BasePlayer
    {
        public override string ToString()
        {
            return "COM " + PlayerName;
        }
        public override void PlayTurn(Discard discard, IDeck cardpile, out bool playedornot, out string consolemessage, ISoundPlayer sound, bool stackingon)
        {
            List<ICard> stackingcards = new List<ICard>();
            foreach (ICard card in Hand)
            {
                if (COMCardLogic(card, discard, cardpile))
                {
                    PlayCard(discard, card, sound);
                    if (stackingon)
                    {
                        while (StackAllowed(discard, ref stackingcards))
                        {
                            StackCard(stackingcards, sound, discard);
                        }
                    }
                    string playmess = "";
                    foreach (ICard card1 in discard.Pile)
                    {
                        playmess += "\n" + ToString() + " has played " + card1.ToString();
                    }
                    playmess += "\n";
                    playedornot = true;
                    consolemessage = "\n" + playmess;
                    return;
                }
            }
            int beforedraw = Hand.Count;
            if (DrawCard(cardpile))
            {
                if (cardpile is not ILaunchDeck)
                {
                    if (CardLogic(discard, Hand[Hand.Count - 1]))
                    {
                        string dmess = drawmess(beforedraw, cardpile);
                        PlayCard(discard, Hand[Hand.Count - 1], sound);
                        if (stackingon)
                        {
                            while (stackingcards.Count > 0)
                            {
                                StackCard(stackingcards, sound, discard);
                                StackAllowed(discard, ref stackingcards);
                            }
                        }
                        string playmess = "";
                        foreach (ICard card in discard.Pile)
                        {
                            playmess +=  "\n" + ToString() + " has played " + card.ToString();
                        }
                        playmess += "\n";
                        playedornot = true;
                        consolemessage =  dmess + playmess;
                        return;
                    }
                    else
                    {
                        playedornot = false;
                        consolemessage = drawmess(beforedraw, cardpile);
                    }
                }
                else
                {
                    playedornot = false;
                    consolemessage = drawmess(beforedraw, cardpile);
                    return;
                }
            }
            else
            {
                if (cardpile is ILaunchDeck && cardpile.Deck.Count != 0)
                {
                    string nomachine = "\n" + ToString() + " pressed the Launcher button once and did not recieve any cards\n";
                    consolemessage = nomachine;
                    playedornot = false;
                    return;
                }
                else
                {
                    string nocardmess = ToString() + " could not play any cards";
                    playedornot = false;
                    consolemessage = nocardmess;
                }
            }
        }

        public override void StackCard(List<ICard> stackablecards, ISoundPlayer player, Discard discard)
        {
            Random rand = new Random();
            PlayCard(discard, stackablecards[rand.Next(0, stackablecards.Count)], player);
        }

        public virtual bool COMCardLogic(ICard Card, Discard discard, IDeck deck)
        {
            if (CardLogic(discard, Card))
            {
                return true;
            }
            return false;
        }
        public override void WildChoose(ICard Card, ISoundPlayer sound)
        {
            List<int> counts = new List<int>(5) { -100, 0, 0, 0, 0 };
            for (int i = 0; i < 4; i++)
            {
                foreach (ICard cardcount in Hand)
                {
                    if (cardcount.Color == (CardColor)i + 1)
                    {
                        counts[i + 1]++;
                    }
                }
            }
            if (Hand.Count == 1)
            {
                Random random = new Random();
                int randindex = random.Next(1, counts.Count);
                Card.Color = (CardColor)randindex;
            }
            else
            {
                Card.Color = (CardColor)counts.IndexOf(counts.Max());
            }
        }

        public override void ChoosePlayer(in List<IPlayer> players, out int playernum)
        {
            List<int> counts = new List<int>(players.Count);
            foreach (IPlayer player in players)
            {
                counts.Add(player.Hand.Count);
            }
            int mincount = 0;
            for (int i = 0; i < counts.Count; i++)
            {
                if (counts.IndexOf(counts[i]) == players.IndexOf(this))
                {
                    counts[i] = 100;
                    continue;
                }
                if (counts[i] == counts.Min())
                {
                    mincount++;
                }
            }
            if (mincount > 1)
            {
                Random random = new Random();
                while (true)
                {
                    int candidate = random.Next(0, counts.Count);
                    if (counts[candidate] == counts.Min())
                    {
                        playernum = candidate;
                        return;
                    }
                }
            }
            else
            {
                playernum = counts.IndexOf(counts.Min());
            }
        }

        public override void DiscardCards(int numofdiscards)
        {
            Random random = new Random();
            for (int i = 0; i < numofdiscards; i++)
            {
                int handcount = Hand.Count;
                for (int h = 0; h < handcount; h++)
                {
                    if (random.Next(1, 101) > 60)
                    {
                        Hand.RemoveAt(h);
                        break;
                    }
                }
            }
        }

        public BaseCOMPLayer(string name)
        : base(name)
        { }
    }
    public class NormCOMPlayer : BaseCOMPLayer
    {
        public NormCOMPlayer(string name)
        : base(name)
        { }

    }

    public class AggroCOMPlayer : BaseCOMPLayer
    {
        Random random = new Random();
        public override bool COMCardLogic(ICard Card, Discard discard, IDeck deck)
        {
            if (base.COMCardLogic(Card, discard, deck))
            {
                if (deck.Deck.Count <= deck.DangerCount)
                {
                    return true;
                }
                else
                {
                    if (Card.IsPlus && random.Next(1, 101) < 80)
                    {
                        return true;
                    }
                    if (!Card.IsPlus && random.Next(1, 101) < 40)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override string ToString()
        {
            return "Aggressive " + base.ToString();
        }
        public AggroCOMPlayer(string name)
        : base(name)
        { }
    }

    public class PassCOMPlayer : BaseCOMPLayer
    {
        Random random = new Random();
        public override bool COMCardLogic(ICard Card, Discard discard, IDeck deck)
        {
            if (base.COMCardLogic(Card, discard, deck))
            {
                if (deck.Deck.Count <= deck.DangerCount)
                {
                    return true;
                }
                else
                {
                    if (Card.IsPlus && random.Next(1, 101) < 40)
                    {
                        return true;
                    }
                    if (!Card.IsPlus && random.Next(1, 101) < 80)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override string ToString()
        {
            return "Passive " + base.ToString();
        }
        public PassCOMPlayer(string name)
        : base(name)
        { }
    }

    public enum OST
    {
        Title,
        Settings,
        Game
    }

    public enum SoundFX
    {
        CardMove,
        Uno,
        Error,
        Select,
        Beep,
        Shuffle,
        Finish,
    }

    public class SoundManager : ISoundPlayer
    {
        public bool Initialized;
        public int CurrentSong;
        public int LastSFX;
        public bool BGMON { get; set; }
        public bool SFXON { get; set; }
        public SoundPlayer[] SongList { get; } =
        {
            BGM("Title"), BGM("Settings"), BGM("Game")
        };

        public SoundPlayer[] SFXList { get; } =
        {
            SFX("CardMove"), SFX("Uno"), SFX("Error"), SFX("Select"), SFX("Beep"), SFX("Shuffle"), SFX("Finish")
        };
        public static MiniAudioEngine Engine { get; protected set; } = new MiniAudioEngine();
        public static AudioFormat Format { get; protected set; } = AudioFormat.Dvd;
        static string SFXPath = @"Audio\SFX\";
        static string BGMPath = @"Audio\BGM\";

        static SoundPlayer BGM(string Name)
        {
            return new SoundPlayer(Engine, Format, new StreamDataProvider(Engine, Format, File.OpenRead(BGMPath + Name + ".wav")));
        }
        static SoundPlayer SFX(string Name)
        {
            return new SoundPlayer(Engine, Format, new StreamDataProvider(Engine, Format, File.OpenRead(SFXPath + Name + ".wav")));
        }

        public Mixer BGMixer { get; set; } = new Mixer(Engine, Format);
        public Mixer SFXMixer { get; set; } = new Mixer(Engine, Format);

        public DeviceInfo Device
        { get; protected set; }
        public AudioPlaybackDevice? Music { get; protected set; }
        public void InitializeBGM()
        {
            Device = Engine.PlaybackDevices.FirstOrDefault();
            Music = Engine.InitializePlaybackDevice(Device, Format);
            for (int i = 0; i < SongList.Length; i++)
            {
                BGMixer.AddComponent(SongList[i]);
                SongList[i].IsLooping = true;
            }
            for (int i = 0; i < SFXList.Length; i++)
            {
                if (i == (int)SoundFX.CardMove)
                {
                    SFXList[i].Volume += 5;
                }
                if (i == (int)SoundFX.Error)
                {
                    SFXList[i].Volume += 5;
                }
                if (i == (int)SoundFX.Shuffle)
                {
                    SFXList[i].Volume += 5;
                }
                if (i == (int)SoundFX.Finish)
                {
                    SFXList[i].PlaybackSpeed = 1.5f;
                }
                SFXMixer.AddComponent(SFXList[i]);
            }
            Music.MasterMixer.AddComponent(BGMixer);
            Music.MasterMixer.AddComponent(SFXMixer);
            Music!.Start();
        }
        public void PlayBGM(OST ost)
        {
            SongList[CurrentSong].Stop();
            SongList[(int)ost].Play();
            CurrentSong = (int)ost;
        }
        public void PlaySFX(SoundFX sfx)
        {
            SFXList[LastSFX].Stop();
            SFXList[(int)sfx].Play();
            LastSFX = (int)sfx;
        }

        public void BGMToggle()
        {
            BGMixer.Mute = !BGMixer.Mute;
        }
        public void SFXToggle()
        {
            SFXMixer.Mute = !SFXMixer.Mute;
        }
    }
    public class UnoManager : IManager
    {
        public void InitializeGame(ref List<IPlayer> players, out IDeck deck, out Discard discard, out bool fullgameornot, out bool multiplayer, out bool started, bool swapallowed, bool shuffleallowed, bool discardallowed, ISoundPlayer sound, bool launcheroverride, ILauncher OverrideLauncher)
        {
            IDeck OutDeck;
            if (launcheroverride)
            {
                OutDeck = DeckSelector(swapallowed, shuffleallowed, discardallowed, sound, launcheroverride);
                OutDeck.OverrideLauncher(OverrideLauncher);
                OutDeck = (ILaunchDeck)OutDeck;
            }
            else
            {
                OutDeck = DeckSelector(swapallowed, shuffleallowed, discardallowed, sound, launcheroverride);
            }
            Discard OutDiscard = new Discard();
            List<IPlayer> OutPlayers;
            if (players.Count == 0)
            {
                OutPlayers = new List<IPlayer>();
            }
            else
            {
                OutPlayers = players;
            }
            OutDeck.StartDeck();
            OutDeck.Shuffle(10);
            bool creating = true;
            bool OutFullGame = true;
            int clearnum = 0;
            while (creating)
            {
                if (!Console.IsInputRedirected)
                {
                    clearnum = Console.CursorTop;
                }
                int playernum = 0;
                Console.WriteLine("Current Players: ");
                foreach (IPlayer player in OutPlayers)
                {
                    playernum++;
                    Console.WriteLine(playernum + ". " + player.ToString());
                }
                Console.WriteLine("What do you want to do?");
                Console.WriteLine("{0,-22} | {1,-20} | {2, -18} | {3,-25}", "1. Create Human Player", "2. Create COM Player", "3. Remove a Player", "4. Finish Player Creation");
                switch (Read.String().ToLower())
                {
                    case "1":
                    case "create human player":
                        sound.PlaySFX(SoundFX.Select);
                        OutPlayers.Add(HumanCreator(sound));
                        sound.PlaySFX(SoundFX.Select);
                        if (!Console.IsOutputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        break;
                    case "2":
                    case "create com player":
                        sound.PlaySFX(SoundFX.Select);
                        OutPlayers.Add(COMCreator(sound));
                        sound.PlaySFX(SoundFX.Select);
                        if (!Console.IsOutputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        break;
                    case "3":
                    case "remove a player":
                        sound.PlaySFX(SoundFX.Select);
                        Console.WriteLine("Please type the number of the player you wish to remove");
                        try
                        {
                            OutPlayers.RemoveAt(Read.Int() - 1);
                            sound.PlaySFX(SoundFX.Select);
                        }
                        catch
                        {
                            sound.PlaySFX(SoundFX.Error);
                            Console.WriteLine("Operation Failed. Please refer to the numbers in the menu to make your choice");
                        }
                        if (!Console.IsOutputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        break;
                    case "4":
                    case "finish player creation":
                        if (OutPlayers.Count < 2)
                        {
                            sound.PlaySFX(SoundFX.Error);
                            Console.WriteLine("You need to have at least 2 players to start a game");
                        }
                        else
                        {
                            if (OutPlayers.Count > 15)
                            {
                                sound.PlaySFX(SoundFX.Error);
                                Console.WriteLine("You cannot have more than 15 players");
                            }
                            else
                            {
                                sound.PlaySFX(SoundFX.Select);
                                creating = false;
                            }
                        }
                        break;
                    default:
                        sound.PlaySFX(SoundFX.Error);
                        Console.WriteLine("Please input the number of the operation you wish to perform");
                        if (!Console.IsOutputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        break;

                }
            }
            if (OutPlayers.Count > 1)
            {
                multiplayer = true;
            }
            else
            {
                multiplayer = false;
            }
            bool chosen = false;
            if (OutPlayers.Count > 2)
            {
                while (!chosen)
                {
                    Console.WriteLine("Do you want the game to end after one person gets rid of all their cards, or do you want to determine 2nd, 3rd, etc?");
                    Console.WriteLine("{0,-13} | {1,-11}", "1. One Winner", "2. Rankings");
                    switch (Read.String().ToLower())
                    {
                        case "1":
                        case "one winner":
                            sound.PlaySFX(SoundFX.Select);
                            OutFullGame = false;
                            chosen = true;
                            break;
                        case "2":
                        case "rankings":
                            sound.PlaySFX(SoundFX.Select);
                            OutFullGame = true;
                            chosen = true;
                            break;
                        default:
                            sound.PlaySFX(SoundFX.Error);
                            Console.WriteLine("Please input the number of the operation you wish to perform");
                            break;
                    }
                }
            }
            else
            {
                OutFullGame = false;
            }
            chosen = false;
            fullgameornot = OutFullGame;
            while (!chosen)
            {
                Console.WriteLine("Do you want to start the game?");
                Console.WriteLine("{0,-6} | {1,-5}", "1. Yes", "2. No");
                switch (Read.String().ToLower())
                {
                    case "1":
                    case "yes":
                        sound.PlaySFX(SoundFX.Select);
                        chosen = true;
                        break;
                    case "2":
                    case "no":
                        sound.PlaySFX(SoundFX.Select);
                        players = OutPlayers;
                        deck = OutDeck;
                        discard = OutDiscard;
                        started = false;
                        return;
                    default:
                        sound.PlaySFX(SoundFX.Error);
                        Console.WriteLine("Please input the number of the operation you wish to perform");
                        break;
                }
            }
            foreach (IPlayer player in OutPlayers)
            {
                player.StartHand(OutDeck);
            }
            int valueofchange = 0;
            while (true)
            {
                valueofchange++;
                OutDiscard.Pile[OutDiscard.Pile.Count() - 1] = OutDeck.Deck[OutDeck.Deck.Count - valueofchange];
                if (OutDiscard.Pile[0] is NumberCard)
                {
                    OutDeck.Deck.RemoveAt(OutDeck.Deck.Count - valueofchange);
                    break;
                }
            }
            if (OutDeck is ILaunchDeck launchDeck)
            {
                launchDeck.Launcher.CalculatePresses();
            }
            deck = OutDeck;
            discard = OutDiscard;
            players = OutPlayers;
            started = true;
        }

        public BaseDeck DeckSelector(bool swapallowed, bool shuffleallowed, bool discardallowed, ISoundPlayer player, bool launcheroverride)
        {
            while (true)
            {
                Console.WriteLine("What deck would you like to use?");
                Console.WriteLine("{0,-15} | {1,-13}", "1. Original Uno", "2. Uno Attack");
                switch (Read.String().ToLower())
                {
                    case "1":
                    case "original uno":
                        player.PlaySFX(SoundFX.Select);
                            return new UnoDeck(swapallowed, shuffleallowed, discardallowed);
                    case "2":
                    case "uno attack":
                        player.PlaySFX(SoundFX.Select);
                        return new AttackDeck();
                    default:
                        player.PlaySFX(SoundFX.Error);
                        Console.WriteLine("Please input the number of the operation you wish to perform");
                        break;
                }
            }
        }

        public BaseHumanPlayer HumanCreator(ISoundPlayer player)
        {
            while (true)
            {
                Console.WriteLine("What kind of Player would you like to create?");
                Console.WriteLine("{0,-14} | {1,-16}", "1. Easy Player", "2. Normal Player");
                switch (Read.String().ToLower())
                {
                    case "1":
                    case "easy player":
                        player.PlaySFX(SoundFX.Select);
                        Console.WriteLine("What would you like to name this player?");
                        player.PlaySFX(SoundFX.Select);
                        return new EasyHumanPlayer(Read.String());
                    case "2":
                    case "normal player":
                        player.PlaySFX(SoundFX.Select);
                        Console.WriteLine("What would you like to name this player?");
                        player.PlaySFX(SoundFX.Select);
                        return new NormHumanPlayer(Read.String());
                    default:
                        player.PlaySFX(SoundFX.Error);
                        Console.WriteLine("Please input the number of the operation you wish to perform");
                        player.PlaySFX(SoundFX.Select);
                        break;

                }
            }
        }

        public BaseCOMPLayer COMCreator(ISoundPlayer player)
        {
            while (true)
            {
                Console.WriteLine("What kind of COM Player would you like to create?");
                Console.WriteLine("{0,-13} | {1,-17} | {2,-14}", "1. Normal COM", "2. Aggressive COM", "3. Passive COM");
                switch (Read.String().ToLower())
                {
                    case "1":
                    case "normal com":
                        player.PlaySFX(SoundFX.Select);
                        Console.WriteLine("What would you like to name this player?");
                        player.PlaySFX(SoundFX.Select);
                        return new NormCOMPlayer(Read.String());
                    case "2":
                    case "aggressive COM":
                        player.PlaySFX(SoundFX.Select);
                        Console.WriteLine("What would you like to name this player?");
                        player.PlaySFX(SoundFX.Select);
                        return new AggroCOMPlayer(Read.String());
                    case "3":
                    case "passive com":
                        player.PlaySFX(SoundFX.Select);
                        Console.WriteLine("What would you like to name this player?");
                        player.PlaySFX(SoundFX.Select);
                        return new PassCOMPlayer(Read.String());
                    default:
                        player.PlaySFX(SoundFX.Error);
                        Console.WriteLine("Please input the number of the operation you wish to perform");
                        break;

                }
            }
        }
        public void SwapHands(List<IPlayer> players, int player1, int player2)
        {
            List<ICard> backup = players[player2].Hand;
            players[player2].Hand = players[player1].Hand;
            players[player1].Hand = backup;
            Console.WriteLine(players[player1].ToString() + " swapped hands with " + players[player2].ToString() + "!\n");
        }
        public void ShuffleHands(List<IPlayer> players)
        {
            int handcount;
            UnoDeck temp = new UnoDeck(true, true, true);
            foreach (IPlayer player in players)
            {
                handcount = player.Hand.Count;
                for (int h = 0; h < handcount; h++)
                {
                    temp.Deck.Add(player.Hand[0]);
                    player.Hand.Remove(player.Hand[0]);
                }
            }
            temp.Shuffle(10);
            foreach (IPlayer player in players)
            {
                for (int i = 0; i < Math.Round((decimal)temp.Deck.Count / players.Count); i++)
                {
                    player.Hand.Add(temp.Deck[temp.Deck.Count - 1]);
                    temp.Deck.RemoveAt(temp.Deck.Count - 1);
                }
            }
            if (temp.Deck.Count != 0)
            {
                while (true)
                {
                    try
                    {
                        for (int i = 0; i < players.Count; i++)
                        {
                            players[i].Hand.Add(temp.Deck[temp.Deck.Count - 1]);
                            temp.Deck.RemoveAt(temp.Deck.Count - 1);
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            }

            Console.WriteLine("Everyone's cards were shuffled around!\n");
        }
        public void CardFunctions(ref bool reversed, ref int numofskips, ref int numofdraws, Discard discard, bool playedornot, int playercount, ref bool needtochoose, ref bool shuffle) //Make these execute each time a card is played
        {
            if (playedornot)
            {
                for (int i = 0; i < discard.Pile.Count; i++)
                {
                    switch (discard.Pile[i])
                    {
                        case SkipCard:
                            numofskips++;
                            numofdraws = 0;
                            needtochoose = false;
                            shuffle = false;
                            break;
                        case ReverseCard:
                            reversed = !reversed;
                            numofdraws = 0;
                            if (playercount == 2)
                            {
                                numofskips++;
                            }
                            else
                            {
                                numofskips = 0;
                            }
                            needtochoose = false;
                            shuffle = false;
                            break;
                        case PlusTwoCard:
                            numofdraws += 2;
                            numofskips = 1;
                            needtochoose = false;
                            shuffle = false;
                            break;
                        case PlusFourCard:
                            numofdraws += 4;
                            numofskips = 1;
                            needtochoose = false;
                            shuffle = false;
                            break;
                        case TargetHitTwoCard:
                            numofdraws += 2;
                            numofskips = 1;
                            needtochoose = true;
                            shuffle = false;
                            break;
                        case SwapCard:
                            numofdraws = 0;
                            numofskips = 0;
                            needtochoose = !needtochoose;
                            shuffle = false;
                            break;
                        case ShuffleCard:
                            numofdraws = 0;
                            numofskips = 0;
                            needtochoose = false;
                            shuffle = true;
                            break;
                        default:
                            numofdraws = 0;
                            numofskips = 0;
                            needtochoose = false;
                            shuffle = false;
                            break;
                    }
                    if (i != discard.Pile.Count - 1)
                    {
                        discard.Pile.RemoveAt(i);
                    }
                }
            }
            else
            {
                numofdraws = 0;
                numofskips = 0;
                needtochoose = false;
                shuffle = false;
            }
        }

        public void Game(List<IPlayer> players, Discard discard, IDeck deck, ISoundPlayer manager, bool stackingon, bool fullgameornot = true, bool multiplayer = false)
        {
            bool finished = false;
            int playernum = -1;
            bool reversed = false;
            int numofskips = 0;
            int numofdraws = 0;
            bool playedcard;
            int clearnum = 0;
            string writestring;
            bool needtochoose = false;
            bool quedchoose = false;
            int plussedplayernum = 0;
            bool swapornot = false;
            bool shuffle = false;
            int beforedrawnum = 0;
            List<IPlayer> finishedplayers = new List<IPlayer>(players.Count - 1);
            string drawmess(IPlayer player)
            {
                if (deck is ILaunchDeck)
                {
                    string backhalf;
                    if (player.Hand.Count - beforedrawnum == 1)
                    {
                        backhalf = "a card";
                    }
                    else
                    {
                        backhalf = player.Hand.Count - beforedrawnum + " cards";
                    }
                    return " and has picked up " + backhalf + "\n";
                }
                else
                {
                    return "";
                }
            }
            string fronthalfdraw(IPlayer player)
            {
                if (deck is ILaunchDeck)
                {
                    return player.ToString() + " pressed the Launcher button " + numofdraws + " times";
                }
                else
                {
                    return player.ToString() + " picked up " + numofdraws + " cards";
                }
            }
            if (!Console.IsOutputRedirected)
            {
                Console.Clear();
            }
            manager.PlaySFX(SoundFX.CardMove);
            Console.WriteLine("The starting card is: " + discard.Pile[discard.Pile.Count() - 1].ToString());
            while (!finished)
            {
                if (players.Count > 1)
                {
                    if (deck.Deck.Count < deck.DangerCount)
                    {
                        deck.FillDeck(players);
                    }
                    if (shuffle)
                    {
                        ShuffleHands(players);
                        manager.PlaySFX(SoundFX.Shuffle);
                    }
                    if (needtochoose)
                    {
                        if (numofdraws > 0)
                        {
                            swapornot = false;
                        }
                        else
                        {
                            swapornot = true;
                        }
                        if (multiplayer && !Console.IsOutputRedirected)
                        {
                            clearnum = Console.CursorTop;
                        }
                        players[playernum].ChoosePlayer(in players, out plussedplayernum);
                        if (multiplayer && !Console.IsOutputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        if (swapornot)
                        {
                            SwapHands(players, playernum, plussedplayernum);
                            manager.PlaySFX(SoundFX.Shuffle);
                            plussedplayernum = playernum;
                        }
                        if (quedchoose)
                        {
                            finishedplayers.Add(players[playernum]);
                            players.RemoveAt(playernum);
                            quedchoose = false;
                            if (fullgameornot == false)
                            {
                                if (finishedplayers.Count == 1)
                                {
                                    finished = true;
                                    continue;
                                }
                            }
                        }
                    }
                    if (!reversed)
                    {
                        if (!needtochoose)
                        {
                            for (int i = 0; i < numofskips + 1; i++)
                            {
                                playernum++;
                                if (playernum >= players.Count)
                                {
                                    playernum = 0;
                                }
                            }
                        }
                        else
                        {
                            playernum = plussedplayernum + 1;
                            if (playernum >= players.Count)
                            {
                                playernum = 0;
                            }
                        }
                    }
                    else
                    {
                        if (!needtochoose)
                        {
                            for (int i = 0; i < numofskips + 1; i++)
                            {
                                playernum--;
                                if (playernum < 0)
                                {
                                    playernum = players.Count - 1;
                                }
                            }
                        }
                        else
                        {
                            playernum = plussedplayernum - 1;
                            if (playernum < 0)
                            {
                                playernum = players.Count - 1;
                            }
                        }
                    }
                    if (!needtochoose)
                    {
                        if (!reversed)
                        {
                            plussedplayernum = playernum - numofskips;
                        }
                        else
                        {
                            plussedplayernum = playernum + numofskips;
                        }
                    }
                    for (int i = 0; i < numofdraws; i++)
                    {
                        if (deck.Deck.Count != 0)
                        {
                            if (deck is ILaunchDeck)
                            {
                                manager.PlaySFX(SoundFX.Beep);
                            }
                            else
                            {
                                manager.PlaySFX(SoundFX.CardMove);
                            }
                            try
                                {
                                    beforedrawnum = players[plussedplayernum].Hand.Count;

                                    if (players[plussedplayernum].DrawCard(deck))
                                    {
                                        if (i == numofdraws - 1)
                                        {
                                            Console.WriteLine(fronthalfdraw(players[plussedplayernum]) + drawmess(players[plussedplayernum]));
                                        }
                                    }
                                    else
                                    {
                                        if (i == numofdraws - 1)
                                        {
                                            Console.WriteLine(players[plussedplayernum].ToString() + " pressed the Launcher button " + numofdraws + " times and did not recieve any cards\n");
                                        }
                                    }
                                }
                                catch
                                {
                                    if (!reversed || playernum == 0 && players.Count == 2)
                                    {
                                        beforedrawnum = players[players.Count - 1].Hand.Count;
                                        if (players[players.Count - 1].DrawCard(deck))
                                        {
                                            if (i == numofdraws - 1)
                                            {
                                                Console.WriteLine(fronthalfdraw(players[players.Count - 1]) + drawmess(players[players.Count - 1]));
                                            }
                                        }
                                        else
                                        {
                                            if (i == numofdraws - 1)
                                            {
                                                Console.WriteLine(players[players.Count - 1].ToString() + " pressed the Launcher button " + numofdraws + " times and did not recieve any cards\n");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        beforedrawnum = players[0].Hand.Count;
                                        if (players[0].DrawCard(deck))
                                        {
                                            if (i == numofdraws - 1)
                                            {
                                                Console.WriteLine(fronthalfdraw(players[0]) + drawmess(players[0]));
                                            }
                                        }
                                        else
                                        {
                                            if (i == numofdraws - 1)
                                            {
                                                Console.WriteLine(players[0].ToString() + " pressed the Launcher button " + numofdraws + " times and did not recieve any cards\n");
                                            }
                                        }
                                    }
                                }
                        }
                        else
                        {
                            Console.WriteLine("The deck has run out of cards, so no more cards can be picked up\n");
                            break;
                        }
                    }

                    if (multiplayer && !Console.IsOutputRedirected)
                    {
                        clearnum = Console.CursorTop;
                    }
                    players[playernum].PlayTurn(discard, deck, out playedcard, out writestring, manager, stackingon);
                    players[playernum].FirstCard = true;
                    if (players[playernum] is not BaseHumanPlayer)
                    {
                        if (deck.Deck.Count > 0)
                        {
                            if (deck is ILaunchDeck && !playedcard)
                            {
                                manager.PlaySFX(SoundFX.Beep);
                            }
                            else
                            {
                                manager.PlaySFX(SoundFX.CardMove);
                            }
                        }
                        else
                        {
                            manager.PlaySFX(SoundFX.Error);
                        }
                    }
                    if (multiplayer && !Console.IsOutputRedirected)
                    {
                        ConsoleUtilities.ClearLines(clearnum);
                    }
                    if (!String.IsNullOrEmpty(writestring))
                    {
                        Console.WriteLine(writestring);
                    }
                    CardFunctions(ref reversed, ref numofskips, ref numofdraws, discard, playedcard, players.Count, ref needtochoose, ref shuffle);
                    if (players[playernum].Hand.Count == 1)
                    {
                        Console.WriteLine(players[playernum].ToString() + " has Uno!\n");
                        manager.PlaySFX(SoundFX.Uno);
                    }
                    if (players[playernum].Hand.Count == 0)
                    {
                        Console.WriteLine(players[playernum].ToString() + " has gotten rid of all their cards!");
                        manager.PlaySFX(SoundFX.Error);
                        if (needtochoose)
                        {
                            quedchoose = true;
                        }
                        else
                        {
                            finishedplayers.Add(players[playernum]);
                            players.RemoveAt(playernum);
                        }
                        if (fullgameornot == false)
                        {
                            if (finishedplayers.Count == 1)
                            {
                                finished = true;
                            }
                        }
                    }
                }
                else
                {
                    finishedplayers.Add(players[0]);
                    players.RemoveAt(0);
                    finished = true;
                }
            }
            manager.PlaySFX(SoundFX.Finish);
            Console.WriteLine("\nThe game has finished!");
            if (fullgameornot == false)
            {
                Console.WriteLine("\n" + finishedplayers[0].ToString() + " has won the game!\n");
            }
            else
            {
                Console.WriteLine("\nRankings: ");
                for (int i = 0; i < finishedplayers.Count; i++)
                {
                    Console.WriteLine((i + 1) + ". " + finishedplayers[i].ToString());
                }
            }
            Console.Write("\nPress any key to return to the main menu: ");
            if (!Console.IsInputRedirected)
            {
                Console.ReadKey(true);
            }
            else
            {
                Console.Read();
            }
        }

        public void Settings(ref bool swapallowed, ref bool shuffleallowed, ref bool discardallowed, ref bool stackingon, ref bool launcheroverride, ref ISoundPlayer player)
        {
            int clearnum = 0;
            while (true)
            {
                if (!Console.IsOutputRedirected)
                {
                    clearnum = Console.CursorTop;
                }
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("{0,-15} | {1,-36} | {2,-23} | {3,-15} | {4,-16}", "1. Change Rules", "2. Manage Custom Cards in Normal Uno", "3. Create a Custom Deck", "4. Edit Sound Settings", "5. Exit Settings");
                switch (Read.String().ToLower())
                {
                    case "1":
                    case "change rules":
                        player.PlaySFX(SoundFX.Select);
                        RulesAndOverrides(player, ref stackingon, ref launcheroverride);
                        if (!Console.IsOutputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        break;
                    case "2":
                    case "manage custom cards in normal uno":
                        player.PlaySFX(SoundFX.Select);
                        CCardsManager(ref swapallowed, ref shuffleallowed, ref discardallowed, player);
                        if (!Console.IsOutputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        break;
                    case "3":
                    case "create a custom deck":
                        player.PlaySFX(SoundFX.Select);
                        Console.WriteLine("Not Implemented Yet!");
                        if (!Console.IsOutputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        break;
                    case "4":
                    case "edit sound settings":
                        player.PlaySFX(SoundFX.Select);
                        SoundSettings(ref player);
                        break;
                    case "5":
                    case "exit settings":
                        player.PlaySFX(SoundFX.Select);
                        return;
                    default:
                        player.PlaySFX(SoundFX.Error);
                        Console.WriteLine("Please input the number corresponding to your choice");
                        break;
                }
            }
        }

        public void RulesAndOverrides(ISoundPlayer player, ref bool stackingon, ref bool launcheroverride)
        {
            int clearnum = 0;
            while (true)
            {
                if (!Console.IsInputRedirected)
                {
                    clearnum = Console.CursorTop;
                }
                Console.WriteLine("Please input the number corresponding to the setting you want to toggle");
                Console.WriteLine("{0,-20} | {1,-29} | {2,-7}", "1. Stacking: " + BoolString(stackingon), "2. Launcher Override: " + BoolString(launcheroverride), "3. Exit");
                switch (Read.String().ToLower())
                {
                    case "1":
                    case "stacking":
                        player.PlaySFX(SoundFX.Select);
                        stackingon = !stackingon;
                        if (!Console.IsInputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        break;
                    case "2":
                    case "launcher override":
                        player.PlaySFX(SoundFX.Select);
                        launcheroverride = !launcheroverride;
                        if (!Console.IsInputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        break;
                    case "3":
                    case "exit":
                        player.PlaySFX(SoundFX.Select);
                        if (!Console.IsInputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        return;
                    default:
                        player.PlaySFX(SoundFX.Error);
                        Console.WriteLine("Please input the number corresponding to your input");
                        if (!Console.IsInputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        break;  
                }

            }

        }

        public void CCardsManager(ref bool swapallowed, ref bool shuffleallowed, ref bool discardallowed, ISoundPlayer player)
        {
            int clearnum = 0;
            while (true)
            {
                if (!Console.IsOutputRedirected)
                {
                    clearnum = Console.CursorTop;
                }
                Console.WriteLine("Please press the number corresponding to the card you want to toggle");
                Console.WriteLine("{0,-22} | {1,-25} | {2,-24} | {3,-7}", "1. Swap Hands: " + BoolString(swapallowed), "2. Shuffle Hands: " + BoolString(shuffleallowed), "3. Discard Five: " + BoolString(discardallowed), "4. Exit");
                switch (Read.String().ToLower())
                {
                    case "1":
                    case "swap hands":
                        player.PlaySFX(SoundFX.Select);
                        swapallowed = !swapallowed;
                        if (!Console.IsOutputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        break;
                    case "2":
                    case "shuffle hands":
                        player.PlaySFX(SoundFX.Select);
                        shuffleallowed = !shuffleallowed;
                        if (!Console.IsOutputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        break;
                    case "3":
                    case "discard five":
                        player.PlaySFX(SoundFX.Select);
                        discardallowed = !discardallowed;
                        if (!Console.IsOutputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        break;
                    case "4":
                    case "exit":
                        player.PlaySFX(SoundFX.Select);
                        return;
                    default:
                        player.PlaySFX(SoundFX.Error);
                        Console.WriteLine("Please input the number corresponding to your input");
                        break;
                }
            }
        }

        public void SoundSettings(ref ISoundPlayer player)
        {
            int clearnum = 0;
            while (true)
            {
                if (!Console.IsOutputRedirected)
                {
                    clearnum = Console.CursorTop;
                }
                Console.WriteLine("Please press the number corresponding to the setting you want to toggle");
                Console.WriteLine("{0,-22} | {1,-25} | {2,-24} | {3,-7}", "1. Overall Sound: " + BoolString(!player.Music!.MasterMixer.Mute), "2. Background Music: " + BoolString(!player.BGMixer.Mute), "3. Sound Effects: " + BoolString(!player.SFXMixer.Mute), "4. Exit");
                switch (Read.String().ToLower())
                {
                    case "1":
                    case "overall sound":
                        player.PlaySFX(SoundFX.Select);
                        player.Music.MasterMixer.Mute = player.BGMixer.Mute = player.SFXMixer.Mute = !player.Music.MasterMixer.Mute;
                        if (!Console.IsOutputRedirected)
                            {
                                ConsoleUtilities.ClearLines(clearnum);
                            }
                        break;
                    case "2":
                    case "background music":
                        player.PlaySFX(SoundFX.Select);
                        player.BGMixer.Mute = !player.BGMixer.Mute;
                        if (!Console.IsOutputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        break;
                    case "3":
                    case "sound effects":
                        player.PlaySFX(SoundFX.Select);
                        player.SFXMixer.Mute = !player.SFXMixer.Mute;
                        if (!Console.IsOutputRedirected)
                        {
                            ConsoleUtilities.ClearLines(clearnum);
                        }
                        break;
                    case "4":
                    case "exit":
                        player.PlaySFX(SoundFX.Select);
                        return;
                    default:
                        player.PlaySFX(SoundFX.Error);
                        Console.WriteLine("Please input the number corresponding to your input");
                        break;
                }
            }
        }

        public string BoolString(bool inbool)
        {
            if (inbool)
            {
                return "Enabled".Pastel(ConsoleColor.Green);
            }
            else
            {
                return "Disabled".Pastel(ConsoleColor.Red);
            }
        }
    }
}