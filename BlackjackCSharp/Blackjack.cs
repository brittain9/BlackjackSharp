using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    enum Outcomes
    {
        PLAYER_WINS, DEALER_WINS, PUSH, PLAYER_Blackjack, DEALER_Blackjack, SPLIT_DECIDED

    };

    enum Choices
    {
        STAND = 1, HIT, DOUBLE_DOWN, SPLIT
    };

    enum SplitCases
    {
        // Player class has 3 hands: Referred to as: Hand, Split1, Split2
        NoSplit,
        SplitHand, // we split Hand giving us Hand and Split1.
        Split2Hand, // we split Hand again giving us Hand, Split1, and Split2 (created from Hand)
        Split2Split1, // we split Split1 giving us Hand, Split1, and Split2 (created from Split1)
        SplitMax
    };

    static class Blackjack
    {
        // Game stats
        static private int _m_PlayerWins;
        static public int m_PlayerWins
        {
            get { return _m_PlayerWins;}
            set { _m_PlayerWins = value; }
        }
        static private int _m_DealerWins;
        static public int m_DealerWins
        {
            get { return _m_DealerWins; }
            set { _m_DealerWins = value; }
        }
        static private int _m_Pushes;
        static public int m_Pushes
        {
            get { return _m_Pushes; }
            set { _m_Pushes = value; }
        }
        static private int _m_PlayerBlackjacks;
        static public int m_PlayerBlackjacks
        {
            get { return _m_PlayerBlackjacks; }
            set { _m_PlayerBlackjacks = value; }
        }
        static private int _m_DealerBlackjacks;
        static public int m_DealerBlackjacks
        {
            get { return _m_DealerBlackjacks; }
            set { _m_DealerBlackjacks = value; }
        }

        static public bool betsOn = true; // remove later
        static public bool END_GAME = false;

        public static int PlayBlackjack(Player player, Dealer dealer, Deck deck)
        {
            // Clear everything from last round
            player.m_Hand.m_CardList.Clear();
            player.m_Split1.m_CardList.Clear();
            player.m_Split2.m_CardList.Clear();
            dealer.m_Hand.m_CardList.Clear();
            player.m_Split1Bet = 0;
            player.m_Split2Bet = 0;
            player.m_InsuranceBet = 0;

            if (betsOn)
            {
                Console.WriteLine($"\n\n========================================================\n\nTotal Bank: ${player.m_BankRoll}");
                Bet(player);
            }

            if (END_GAME)
                return (int)Outcomes.SPLIT_DECIDED;

            player.m_Hand.m_CardList.Add(deck.DrawCard());
            dealer.m_Hand.m_CardList.Add(deck.DrawCard());
            player.m_Hand.m_CardList.Add(deck.DrawCard());
            dealer.m_Hand.m_CardList.Add(deck.DrawCard());

            Console.Write("\n\t");
            dealer.PrintDealerUpCard();
            Console.Write("\n\t");
            Console.Write(player.m_Hand);

            int dealerPeekBJ = 0;
            if (betsOn && dealer.DealerUpCardValue() == 1)
                dealerPeekBJ = InsuranceBet(player, dealer);

            if(dealerPeekBJ == 1)
                return (int)Outcomes.DEALER_Blackjack;

            if (betsOn)
                Console.WriteLine($"\n\tBet: {player.m_Bet}");

            int winner = HandleBlackjacks(player, dealer);
            if (winner != 0)
                return winner;

            GetPlayerInput(player, dealer, deck);

            if (player.m_Split1.m_CardList.Any())
                return (int)Outcomes.SPLIT_DECIDED;

            if (player.m_Hand.CheckBust())
            {
                Console.WriteLine("Player busted");
                return (int)Outcomes.DEALER_WINS;
            }

            dealer.AI(deck);
            if (dealer.m_Hand.CheckBust())
            {
                Console.WriteLine("Dealer busted");
                return (int)Outcomes.PLAYER_WINS;
            }

            // Determine who won
            if (player.m_Hand.GetHandValue() < dealer.m_Hand.GetHandValue())
                return (int)Outcomes.DEALER_WINS;
            if (player.m_Hand.GetHandValue() > dealer.m_Hand.GetHandValue())
                return (int)Outcomes.PLAYER_WINS;
            return (int)Outcomes.PUSH;
        }

        private static void Bet(Player player)
        {
            while (true)
            {
                int bet = getIntPut("Enter bet or 0 for hotkeys: ");

                if (bet > player.m_BankRoll)
                {
                    Console.WriteLine("You cannot afford that bet.");
                    continue;
                }

                if (bet < 0)
                {
                    Console.WriteLine("Don't try to cheat the house.");
                    continue;
                }

                switch (bet)
                {
                    case 0:
                        Console.WriteLine(
                            "Press 1 for last bet size\nPress 2 for half last bet size\nPress 9 to end and show results\n");
                        continue;
                    case 1:
                        if (player.m_LastBet == 0)
                        {
                            Console.WriteLine("You don't have a last bet.");
                            continue;
                        }

                        if (player.m_LastBet > player.m_BankRoll)
                        {
                            Console.WriteLine("\nYou cannot afford to bet your last bet.");
                            continue;
                        }
                        
                        player.m_Bet = player.m_LastBet;
                        return;
                    case 2:
                        if (player.m_LastBet == 0)
                        {
                            Console.WriteLine("You don't have a last bet.");
                            continue;
                        }

                        if ((int)(player.m_LastBet / 2.0f) > player.m_BankRoll)
                        {
                            Console.WriteLine("\nYou cannot afford to bet your last bet.");
                            continue;
                        }

                        player.m_Bet = (int)(player.m_LastBet / 2.0f);
                        return;
                    case 9:
                        END_GAME = true;
                        return;
                    default:
                        player.m_Bet = bet;
                        player.m_LastBet = player.m_Bet;
                        return;
                }
            }
        }

        private static int InsuranceBet(Player player, Dealer dealer)
        {
            while (true)
            {
                int bet = getIntPut("Dealer showing ace. Enter insurance bet (0 for no bet): ");
                switch (bet)
                {
                    case 0:
                        Console.WriteLine("No insurance bet placed.");
                        break;
                    default:
                        if (player.m_LastBet < player.m_Bet + player.m_InsuranceBet)
                        {
                            Console.WriteLine("You cannot afford this insurance bet.");
                            continue;
                        }

                        if (bet > (int)(player.m_Bet / 2))
                        {
                            Console.WriteLine("\nThe maximum value for an insurance bet is half your original stake.\n");
                            continue;
                        }
                        player.m_InsuranceBet = bet;
                        break;
                }

                Console.WriteLine("Dealer is peeking other card to check for Blackjack");
                if (dealer.m_Hand.CheckBlackjack())
                {
                    Console.Write("Dealer has blackjack.");
                    if (bet > 0)
                    {
                        Console.WriteLine($"You made 2:1 on your insurance bet paying out {bet * 2}!");
                        player.m_BankRoll += bet * 2;
                    }
                    return (int)Outcomes.DEALER_Blackjack;
                }
                Console.WriteLine("No Blackjack.");
                player.m_BankRoll -= bet;
                return 0;
            }
        }

        private static int HandleBlackjacks(Player player, Dealer dealer)
        {
            // Returns 0 if no BJs
            if (player.m_Hand.CheckBlackjack())
            {
                Console.WriteLine("Player Blackjack!");
                if (dealer.m_Hand.CheckBlackjack())
                {
                    Console.WriteLine("Dealer also had Blackjack!");
                    Console.Write("\tBlackjack: ");
                    Console.Write(dealer.m_Hand);
                    return (int)Outcomes.PUSH;
                }
                return (int)Outcomes.PLAYER_Blackjack;
            }
            if (dealer.m_Hand.CheckBlackjack())
            {
                Console.WriteLine("Dealer Blackjack!");
                Console.Write("\tBlackjack: ");
                Console.Write(dealer.m_Hand);
                return (int)Outcomes.DEALER_Blackjack;
            }
            return 0;
        }

        private static void GetPlayerInput(Player player, Dealer dealer, Deck deck,
            bool splitAces = false, int splitInfo = 0)
        {
            Console.WriteLine("\nPlayers Turn.    ");
            int choice;
            while (true)
            {
                Console.Write("Enter 1 to stand    Enter 2 to hit    ");
                if (betsOn && player.m_Hand.m_CardList.Count == 2)
                    Console.Write("Enter 3 to double down    ");
                if (player.m_Hand.isSplittable(true))
                    Console.Write("Enter 4 to split    ");

                choice = getIntPut("Decision: ");
                switch (choice)
                {
                    case (int)Choices.STAND:
                        Console.WriteLine("\tPlayer Stands.\n");
                        return;
                    case (int)Choices.HIT:
                        if (splitAces && player.m_Hand.m_CardList.Count > 2)
                        {
                            Console.WriteLine("You can only hit ace splits once.");
                            continue;
                        }
                        player.m_Hand.m_CardList.Add(deck.DrawCard());
                        Console.WriteLine("\tPlayer hand: " + player.m_Hand);
                        if (player.m_Hand.CheckBust())
                            return;
                        if (player.m_Hand.GetHandValue() == 21) 
                            return;
                        continue;
                    case (int)Choices.DOUBLE_DOWN:
                        if (player.m_Hand.m_CardList.Count > 2)
                        {
                            Console.WriteLine("\tYou didn't even have the option to double down.\n");
                            continue;
                        }
                        if (player.m_BankRoll < player.m_Bet * 2 + player.m_Split1Bet + player.m_Split2Bet)
                        {
                            Console.WriteLine("\tYou're too poor to double down.\n");
                            continue;
                        }
                        if (splitInfo == (int)SplitCases.SplitHand)
                            player.m_Split1Bet *= 2;
                        else if (splitInfo >= (int)SplitCases.Split2Hand)
                            player.m_Split2Bet *= 2; // TODO TEST
                        else
                            player.m_Bet *= 2;

                        player.m_Hand.m_CardList.Add(deck.DrawCard());
                        Console.Write("\tPlayer hand: " + player.m_Hand);
                        return;
                    case (int)Choices.SPLIT:
                        if (!player.m_Hand.isSplittable(true))
                        {
                            Console.WriteLine("\nYou weren't even given the option to split this hand.");
                            continue;
                        }
                        if (player.m_BankRoll < player.m_Bet + player.m_Split1Bet + player.m_Split2Bet)
                        {
                            Console.WriteLine("\tYou're too poor to double down.\n");
                            continue;
                        }

                        if (player.m_Hand.m_CardList.ElementAt(0).RankValue() == 1)
                            splitAces = true;

                        switch (splitInfo)
                        {
                            case (int)SplitCases.NoSplit:
                                player.m_Split1.m_CardList.Add(player.m_Hand.m_CardList.ElementAt(1));
                                player.m_Split1.m_CardList.Add(deck.DrawCard());
                                player.m_Split1Bet = player.m_Bet;

                                player.m_Hand.m_CardList.RemoveAt(1);
                                player.m_Hand.m_CardList.Add(deck.DrawCard());
                                splitInfo = (int)SplitCases.SplitHand;
                                break;
                            case (int)SplitCases.SplitHand:
                                break;
                            case (int)SplitCases.Split2Hand:
                                player.m_Split2.m_CardList.Add(player.m_Hand.m_CardList.ElementAt(1));
                                player.m_Split2.m_CardList.Add(deck.DrawCard());
                                player.m_Split2Bet = player.m_Bet;

                                player.m_Hand.m_CardList.RemoveAt(1);
                                player.m_Hand.m_CardList.Add(deck.DrawCard());
                                splitInfo = (int)SplitCases.SplitMax;
                                break;
                            case (int)SplitCases.Split2Split1:
                                player.m_Split2.m_CardList.Add(player.m_Split1.m_CardList.ElementAt(1));
                                player.m_Split2.m_CardList.Add(deck.DrawCard());
                                player.m_Split2Bet = player.m_Bet;

                                player.m_Split1.m_CardList.RemoveAt(1);
                                player.m_Split1.m_CardList.Add(deck.DrawCard());
                                splitInfo = (int)SplitCases.SplitMax;
                                break;
                            case (int)SplitCases.SplitMax:
                                Console.WriteLine("\nYou can only split twice.\n");
                                continue;
                        }

                        if (splitInfo == (int)SplitCases.SplitHand || splitInfo == (int)SplitCases.Split2Hand || splitInfo == (int)SplitCases.SplitMax)
                        {
                            Console.Write("\tFIRST hand: " + player.m_Hand + "\tSecond hand: " + player.m_Split1);
                            if (player.m_Split2.m_CardList.Any())
                                Console.Write("\tThird hand: " + player.m_Split2);

                            GetPlayerInput(player, dealer, deck, splitAces, (int)SplitCases.Split2Hand);
                        }

                        if (splitInfo == (int)SplitCases.SplitHand || splitInfo == (int)SplitCases.Split2Hand || splitInfo == (int)SplitCases.Split2Split1 || splitInfo == (int)SplitCases.SplitMax)
                        {
                            Console.Write("\tFIRST hand: " + player.m_Hand + "\tSecond hand: " + player.m_Split1);
                            if (player.m_Split2.m_CardList.Any())
                                Console.Write("\tThird hand: " + player.m_Split2);

                            GetPlayerInput(player, dealer, deck, splitAces, (int)SplitCases.Split2Split1);
                        }

                        if (splitInfo == (int)SplitCases.Split2Hand || splitInfo == (int)SplitCases.Split2Split1 || splitInfo == (int)SplitCases.SplitMax) // if we have split once but not twice
                        {
                            Console.Write("\tFIRST hand: " + player.m_Hand + "\tSecond hand: " + player.m_Split1 + "\tTHIRD hand: " + player.m_Split2);
                            GetPlayerInput(player, dealer, deck, splitAces, (int)SplitCases.SplitMax);
                        }

                        DetermineSplits(player, dealer, ref deck);
                        return;
                }
            }
        }

        private static int DetermineSplits(Player player, Dealer dealer,ref Deck deck)
        {
            dealer.AI(deck);

            int firstOutcome;
            if (player.m_Hand.CheckBust())
            {
                firstOutcome = (int)Outcomes.DEALER_WINS;
            }
            else
            {
                if (player.m_Hand.GetHandValue() < dealer.m_Hand.GetHandValue())
                    firstOutcome = (int)Outcomes.DEALER_WINS;
                else if (player.m_Hand.GetHandValue() > dealer.m_Hand.GetHandValue())
                    firstOutcome = (int)Outcomes.PLAYER_WINS;
                else firstOutcome = (int)Outcomes.PUSH;
            }

            int secondOutcome;
            if (player.m_Split1.CheckBust())
            {
                secondOutcome = (int)Outcomes.DEALER_WINS;
            }
            else
            {
                if (player.m_Split1.GetHandValue() < dealer.m_Hand.GetHandValue())
                    secondOutcome = (int)Outcomes.DEALER_WINS;
                else if (player.m_Split1.GetHandValue() > dealer.m_Hand.GetHandValue())
                    secondOutcome = (int)Outcomes.PLAYER_WINS;
                else secondOutcome = (int)Outcomes.PUSH;
            }

            int thirdOutcome = (int)Outcomes.SPLIT_DECIDED;
            if (player.m_Split2.m_CardList.Any())
            {
                if (player.m_Split2.CheckBust())
                {
                    thirdOutcome = (int)Outcomes.DEALER_WINS;
                }
                else
                {
                    if (player.m_Split2.GetHandValue() < dealer.m_Hand.GetHandValue())
                        thirdOutcome = (int)Outcomes.DEALER_WINS;
                    else if (player.m_Split2.GetHandValue() > dealer.m_Hand.GetHandValue())
                        thirdOutcome = (int)Outcomes.PLAYER_WINS;
                    else thirdOutcome = (int)Outcomes.PUSH;
                }
            }

            if (dealer.m_Hand.CheckBust())
            {
                // if dealer busted and player not already busted.
                if (!player.m_Hand.CheckBust())
                    firstOutcome = (int)Outcomes.PLAYER_WINS;
                if (!player.m_Split1.CheckBust())
                    secondOutcome = (int)Outcomes.PLAYER_WINS;
                if (player.m_Split2.m_CardList.Any())
                    if (!player.m_Split2.CheckBust())
                        thirdOutcome = (int)Outcomes.PLAYER_WINS;
            }

            HandleOutcomes(player, dealer, firstOutcome);
            HandleOutcomes(player, dealer, secondOutcome, 1);
            if (player.m_Split2.m_CardList.Any())
                HandleOutcomes(player, dealer, thirdOutcome, 2);

            return (int)Outcomes.SPLIT_DECIDED;
        }

        public static void HandleOutcomes(Player player, Dealer dealer, int outcome, int split = 0)
        {
            switch (outcome)
            {
                case (int)Outcomes.PLAYER_WINS:
                    m_PlayerWins++;
                    Console.Write("\nPlayer wins");
                    if (betsOn && split == 0)
                    {
                        Console.Write($" ${player.m_Bet}!");
                        player.m_BankRoll += player.m_Bet;
                    }
                    if (betsOn && split == 1)
                    {
                        Console.Write($" ${player.m_Split1Bet} on split 1!");
                        player.m_BankRoll += player.m_Split1Bet;

                    }
                    if (betsOn && split == 2)
                    {
                        Console.Write($" ${player.m_Split2Bet} on split 2!");
                        player.m_BankRoll += player.m_Split2Bet;
                    }
                    break;
                case (int)Outcomes.DEALER_WINS:
                    m_DealerWins++;
                    Console.Write("\nDealer wins.");
                    if (betsOn && split == 0)
                    {
                        Console.Write($" Player loses ${player.m_Bet}!");
                        player.m_BankRoll -= player.m_Bet;
                    }
                    if (betsOn && split == 1)
                    {
                        Console.Write($" Player loses ${player.m_Split1Bet} on split 1!");
                        player.m_BankRoll -= player.m_Split1Bet;

                    }
                    if (betsOn && split == 2)
                    {
                        Console.Write($" Player loses ${player.m_Split2Bet} on split 2!");
                        player.m_BankRoll -= player.m_Split2Bet;
                    }
                    break;
                case (int)Outcomes.PUSH:
                    m_Pushes++;
                    Console.Write("Draw!");
                    if (betsOn && split == 0)
                    {
                        Console.Write($" Returned ${player.m_Bet}.\n");
                    }
                    if (betsOn && split == 1)
                    {
                        Console.Write($" Returned  ${player.m_Split1Bet} on split 1!");
                    }
                    if (betsOn && split == 2)
                    {
                        Console.Write($" Returned ${player.m_Split2Bet} on split 2!");
                    }
                    break;

                case (int)Outcomes.PLAYER_Blackjack:
                    m_PlayerWins++;
                    m_PlayerBlackjacks++;
                    Console.Write("\n\tBlackjack for the player!\n");
                    if (betsOn)
                    {
                        Console.Write($" They win ${(int)(player.m_Bet * 1.5f)}!\n");
                        player.m_BankRoll += (int)(player.m_Bet * 1.5f);
                    }
                    break;
                case (int)Outcomes.DEALER_Blackjack:
                    m_DealerWins++;
                    m_DealerBlackjacks++;
                    Console.Write("\n\tBlackjack for the dealer!\n");
                    if (betsOn)
                    {
                        Console.Write($" They lose ${player.m_Bet}!\n");
                        player.m_BankRoll -= (int)(player.m_Bet);
                    }
                    break;
                case (int)Outcomes.SPLIT_DECIDED:
                    break;
                    
            }
        }
        // Static Functions

        public static int getIntPut(string message)
        {
            Console.Write(message);
            int input;
            while (true)
            {
                try
                {
                    input = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Please enter a single integer only.");
                    continue;
                }
                break;
            }
            return input;
        }
    }
}
