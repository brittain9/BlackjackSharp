using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    using deck_t = System.Collections.Generic.List<Card>;

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
        static public int m_PlayerWins = 0;
        static public int m_DealerWins = 0;
        static public int m_Pushes = 0;
        static public int m_PlayerBlackjacks = 0;
        static public int m_DealerBlackjacks = 0;

        static public bool betsOn = true; // remove later
        static public bool END_GAME = false;

        public static int PlayBlackjack(ref Player player,ref Dealer dealer,ref Deck deck)
        {
            // Clear everything from last round
            player.getHand().Clear();
            player.getSplit1().Clear();
            player.getSplit2().Clear();
            dealer.getHand().Clear();
            player.setSplit1Bet(0);
            player.setSplit2Bet(0);
            player.setInsureBet(0);

            if (betsOn)
            {
                Console.WriteLine($"\n\n========================================================\n\nTotal Bank: ${player.getBankRoll()}");
                Bet(ref player);
            }

            if (END_GAME)
                return (int)Outcomes.SPLIT_DECIDED;

            player.getHand().Add(deck.DrawCard());
            dealer.getHand().Add(deck.DrawCard());
            player.getHand().Add(deck.DrawCard());
            dealer.getHand().Add(deck.DrawCard());

            Console.Write("\n\t");
            dealer.PrintDealerUpCard();
            Console.Write("\n\t");
            PrintCards(player.getHand());

            int dealerPeekBJ = 0;
            if (betsOn && dealer.DealerUpCardValue() == 1)
                dealerPeekBJ = InsuranceBet(ref player, dealer);

            if(dealerPeekBJ == 1)
                return (int)Outcomes.DEALER_Blackjack;

            if (betsOn)
                Console.WriteLine($"\n\tBet: {player.getBet()}");

            int winner = HandleBlackjacks(player, dealer);
            if (winner != 0)
                return winner;

            GetPlayerInput(ref player, dealer, ref deck);

            if (player.getSplit1().Any())
                return (int)Outcomes.SPLIT_DECIDED;

            if (CheckBust(player.getHand()))
            {
                Console.WriteLine("Player busted");
                return (int)Outcomes.DEALER_WINS;
            }

            dealer.AI(ref deck);
            if (CheckBust(dealer.getHand()))
            {
                Console.WriteLine("Dealer busted");
                return (int)Outcomes.PLAYER_WINS;
            }

            // Determine who won
            if (GetHandValue(player.getHand()) < GetHandValue(dealer.getHand()))
                return (int)Outcomes.DEALER_WINS;
            if (GetHandValue(player.getHand()) > GetHandValue(dealer.getHand()))
                return (int)Outcomes.PLAYER_WINS;
            return (int)Outcomes.PUSH;
        }

        private static void Bet(ref Player player)
        {
            while (true)
            {
                int bet = getIntPut("Enter bet or 0 for hotkeys: ");

                if (bet > player.getBankRoll())
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
                        if (player.getLastBet() == 0)
                        {
                            Console.WriteLine("You don't have a last bet.");
                            continue;
                        }

                        if (player.getLastBet() > player.getBankRoll())
                        {
                            Console.WriteLine("\nYou cannot afford to bet your last bet.");
                            continue;
                        }

                        player.setBet(player.getLastBet());
                        return;
                    case 2:
                        if (player.getLastBet() == 0)
                        {
                            Console.WriteLine("You don't have a last bet.");
                            continue;
                        }

                        if ((int)(player.getLastBet() / 2.0f) > player.getBankRoll())
                        {
                            Console.WriteLine("\nYou cannot afford to bet your last bet.");
                            continue;
                        }

                        player.setBet((int)(player.getLastBet() / 2.0f));
                        return;
                    case 9:
                        END_GAME = true;
                        return;
                    default:
                        player.setBet(bet);
                        player.setLastBet(player.getBet());
                        return;
                }
            }
        }

        private static int InsuranceBet(ref Player player, Dealer dealer)
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
                        if (player.getLastBet() < player.getBet() + player.getInsureBet())
                        {
                            Console.WriteLine("You cannot afford this insurance bet.");
                            continue;
                        }

                        if (bet > (int)(player.getBet() / 2))
                        {
                            Console.WriteLine("\nThe maximum value for an insurance bet is half your original stake.\n");
                            continue;
                        }
                        player.setInsureBet(bet);
                        break;
                }

                Console.WriteLine("Dealer is peeking other card to check for Blackjack");
                if (Blackjack.checkBlackjack(dealer.getHand()))
                {
                    Console.Write("Dealer has blackjack.");
                    if (bet > 0)
                    {
                        Console.WriteLine($"You made 2:1 on your insurance bet paying out {bet * 2}!");
                        player.setBankRoll(player.getBankRoll() + bet * 2);
                    }
                    return (int)Outcomes.DEALER_Blackjack;
                }
                Console.WriteLine("No Blackjack.");
                player.setBankRoll(player.getBankRoll() - bet);
                return 0;
            }
        }

        private static int HandleBlackjacks(Player player, Dealer dealer)
        {
            // Returns 0 if no BJs
            if (checkBlackjack(player.getHand()))
            {
                Console.WriteLine("Player Blackjack!");
                if (checkBlackjack(dealer.getHand()))
                {
                    Console.WriteLine("Dealer also had Blackjack!");
                    Console.Write("\tBlackjack: ");
                    PrintCards(dealer.getHand());
                    return (int)Outcomes.PUSH;
                }
                return (int)Outcomes.PLAYER_Blackjack;
            }
            if (checkBlackjack(dealer.getHand()))
            {
                Console.WriteLine("Dealer Blackjack!");
                Console.Write("\tBlackjack: ");
                PrintCards(dealer.getHand());
                return (int)Outcomes.DEALER_Blackjack;
            }
            return 0;
        }

        private static void GetPlayerInput(ref Player player, Dealer dealer, ref Deck deck,
            bool splitAces = false, int splitInfo = 0)
        {
            Console.WriteLine("Players Turn.    ");
            int choice;
            while (true)
            {
                Console.Write("Enter 1 to stand    Enter 2 to hit    ");
                if (betsOn && player.getHand().Count == 2)
                    Console.Write("Enter 3 to double down    ");
                if (isSplittable(player.getHand(), true))
                    Console.Write("Enter 4 to split    ");

                choice = getIntPut("Decision: ");
                switch (choice)
                {
                    case (int)Choices.STAND:
                        Console.WriteLine("\tPlayer Stands.\n");
                        return;
                    case (int)Choices.HIT:
                        if (splitAces && player.getHand().Count > 2)
                        {
                            Console.WriteLine("\nYou can only hit ace splits once.\n");
                            continue;
                        }
                        player.getHand().Add(deck.DrawCard());
                        Console.Write("\tPlayer hand: ");
                        PrintCards(player.getHand());
                        if (CheckBust(player.getHand()))
                            return;
                        if (GetHandValue(player.getHand()) == 21) 
                            return;
                        continue;
                    case (int)Choices.DOUBLE_DOWN:
                        if (player.getHand().Count > 2)
                        {
                            Console.WriteLine("\tYou didn't even have the option to double down.\n");
                            continue;
                        }
                        if (player.getBankRoll() < player.getBet() * 2 + player.getSplit1Bet() + player.getSplit2Bet())
                        {
                            Console.WriteLine("\tYou're too poor to double down.\n");
                            continue;
                        }
                        if (splitInfo == (int)SplitCases.SplitHand)
                            player.setSplit1Bet(player.getSplit1Bet() * 2);
                        else if (splitInfo >= (int)SplitCases.Split2Hand)
                            player.setSplit2Bet(player.getSplit2Bet() * 2); // TODO TEST
                        else
                            player.setBet(player.getBet()*2);

                        player.getHand().Add(deck.DrawCard());
                        Console.Write("\tPlayer hand: ");
                        PrintCards(player.getHand());
                        return;
                    case (int)Choices.SPLIT:
                        if (!isSplittable(player.getHand(), true))
                        {
                            Console.WriteLine("\nYou weren't even given the option to split this hand.");
                            continue;
                        }
                        if (player.getBankRoll() < player.getBet() + player.getSplit1Bet() + player.getSplit2Bet())
                        {
                            Console.WriteLine("\tYou're too poor to double down.\n");
                            continue;
                        }

                        if (player.getHand().ElementAt(0).RankValue() == 1)
                            splitAces = true;

                        switch (splitInfo)
                        {
                            case (int)SplitCases.NoSplit:
                                player.getSplit1().Add(player.getHand().ElementAt(1));
                                player.getSplit1().Add(deck.DrawCard());
                                player.setSplit1Bet(player.getBet());

                                player.getHand().RemoveAt(1);
                                player.getHand().Add(deck.DrawCard());
                                splitInfo = (int)SplitCases.SplitHand;
                                break;
                            case (int)SplitCases.SplitHand:
                                break;
                            case (int)SplitCases.Split2Hand:
                                player.getSplit2().Add(player.getHand().ElementAt(1));
                                player.getSplit2().Add(deck.DrawCard());
                                player.setSplit2Bet(player.getBet());

                                player.getHand().RemoveAt(1);
                                player.getHand().Add(deck.DrawCard());
                                splitInfo = (int)SplitCases.SplitMax;
                                break;
                            case (int)SplitCases.Split2Split1:
                                player.getSplit2().Add(player.getSplit1().ElementAt(1));
                                player.getSplit2().Add(deck.DrawCard());
                                player.setSplit2Bet(player.getBet());

                                player.getSplit1().RemoveAt(1);
                                player.getSplit1().Add(deck.DrawCard());
                                splitInfo = (int)SplitCases.SplitMax;
                                break;
                            case (int)SplitCases.SplitMax:
                                Console.WriteLine("\nYou can only split twice.\n");
                                continue;
                        }

                        if (splitInfo == (int)SplitCases.SplitHand || splitInfo == (int)SplitCases.Split2Hand || splitInfo == (int)SplitCases.SplitMax)
                        {
                            Console.Write("\tFIRST hand: ");
                            PrintCards(player.getHand());
                            Console.Write("\tSecond hand: ");
                            PrintCards(player.getSplit1());
                            if (player.getSplit2().Any())
                            {
                                Console.Write("\tThird hand: ");
                                PrintCards(player.getSplit2());
                            }
                            GetPlayerInput(ref player, dealer, ref deck, splitAces, (int)SplitCases.Split2Hand);
                        }

                        if (splitInfo == (int)SplitCases.SplitHand || splitInfo == (int)SplitCases.Split2Hand || splitInfo == (int)SplitCases.Split2Split1 || splitInfo == (int)SplitCases.SplitMax)
                        {
                            Console.Write("\tFirst hand: ");
                            PrintCards(player.getHand());
                            Console.Write("\tSECOND hand: ");
                            PrintCards(player.getSplit1());
                            if (player.getSplit2().Any())
                            {
                                Console.Write("\tThird hand: ");
                                PrintCards(player.getSplit2());
                            }
                            GetPlayerInput(ref player, dealer,ref deck, splitAces, (int)SplitCases.Split2Split1);
                        }

                        if (splitInfo == (int)SplitCases.Split2Hand || splitInfo == (int)SplitCases.Split2Split1 || splitInfo == (int)SplitCases.SplitMax) // if we have split once but not twice
                        {
                            Console.Write("\tFirst hand: ");
                            PrintCards(player.getHand());
                            Console.Write("\tSecond hand: ");
                            PrintCards(player.getSplit1());
                            Console.Write("\tTHIRD hand: ");
                            PrintCards(player.getSplit2());
                            
                            GetPlayerInput(ref player, dealer, ref deck, splitAces, (int)SplitCases.SplitMax);
                        }

                        DetermineSplits(player, dealer, ref deck);
                        return;
                }
            }
        }

        private static int DetermineSplits(Player player, Dealer dealer,ref Deck deck)
        {
            dealer.AI(ref deck);

            int firstOutcome;
            if (CheckBust(player.getHand()))
            {
                firstOutcome = (int)Outcomes.DEALER_WINS;
            }
            else
            {
                if (GetHandValue(player.getHand()) < GetHandValue(dealer.getHand()))
                    firstOutcome = (int)Outcomes.DEALER_WINS;
                else if (GetHandValue(player.getHand()) > GetHandValue(dealer.getHand()))
                    firstOutcome = (int)Outcomes.PLAYER_WINS;
                else firstOutcome = (int)Outcomes.PUSH;
            }

            int secondOutcome;
            if (CheckBust(player.getSplit1()))
            {
                secondOutcome = (int)Outcomes.DEALER_WINS;
            }
            else
            {
                if (GetHandValue(player.getSplit1()) < GetHandValue(dealer.getHand()))
                    secondOutcome = (int)Outcomes.DEALER_WINS;
                else if (GetHandValue(player.getSplit1()) > GetHandValue(dealer.getHand()))
                    secondOutcome = (int)Outcomes.PLAYER_WINS;
                else secondOutcome = (int)Outcomes.PUSH;
            }

            int thirdOutcome = (int)Outcomes.SPLIT_DECIDED;
            if (player.getSplit2().Any())
            {
                if (CheckBust(player.getSplit2()))
                {
                    thirdOutcome = (int)Outcomes.DEALER_WINS;
                }
                else
                {
                    if (GetHandValue(player.getSplit2()) < GetHandValue(dealer.getHand()))
                        thirdOutcome = (int)Outcomes.DEALER_WINS;
                    else if (GetHandValue(player.getSplit2()) > GetHandValue(dealer.getHand()))
                        thirdOutcome = (int)Outcomes.PLAYER_WINS;
                    else thirdOutcome = (int)Outcomes.PUSH;
                }
            }

            if (CheckBust(dealer.getHand()))
            {
                // if dealer busted and player not already busted.
                if (!CheckBust(player.getHand()))
                    firstOutcome = (int)Outcomes.PLAYER_WINS;
                if (!CheckBust(player.getSplit1()))
                    secondOutcome = (int)Outcomes.PLAYER_WINS;
                if (player.getSplit2().Any())
                    if (CheckBust(player.getSplit2()))
                        thirdOutcome = (int)Outcomes.PLAYER_WINS;
            }

            HandleOutcomes(player, dealer, firstOutcome);
            HandleOutcomes(player, dealer, secondOutcome, 1);
            if (player.getSplit2().Any())
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
                        Console.Write($" ${player.getBet()}!");
                        player.setBankRoll(player.getBankRoll() + player.getBet());
                    }
                    if (betsOn && split == 1)
                    {
                        Console.Write($" ${player.getSplit1Bet()} on split 1!");
                        player.setBankRoll(player.getBankRoll() + player.getSplit1Bet());

                    }
                    if (betsOn && split == 2)
                    {
                        Console.Write($" ${player.getSplit2Bet()} on split 2!");
                        player.setBankRoll(player.getBankRoll() + player.getSplit2Bet());
                    }
                    break;
                case (int)Outcomes.DEALER_WINS:
                    m_DealerWins++;
                    Console.Write("\nDealer wins.");
                    if (betsOn && split == 0)
                    {
                        Console.Write($" Player loses ${player.getBet()}!");
                        player.setBankRoll(player.getBankRoll() - player.getBet());
                    }
                    if (betsOn && split == 1)
                    {
                        Console.Write($" Player loses ${player.getSplit1Bet()} on split 1!");
                        player.setBankRoll(player.getBankRoll() - player.getSplit1Bet());

                    }
                    if (betsOn && split == 2)
                    {
                        Console.Write($" Player loses ${player.getSplit2Bet()} on split 2!");
                        player.setBankRoll(player.getBankRoll() - player.getSplit2Bet());
                    }
                    break;
                case (int)Outcomes.PUSH:
                    m_Pushes++;
                    Console.Write("Draw!");
                    if (betsOn && split == 0)
                    {
                        Console.Write($" Returned ${player.getBet()}.\n");
                    }
                    if (betsOn && split == 1)
                    {
                        Console.Write($" Returned  ${player.getSplit1Bet()} on split 1!");
                    }
                    if (betsOn && split == 2)
                    {
                        Console.Write($" Returned ${player.getSplit2Bet()} on split 2!");
                    }
                    break;

                case (int)Outcomes.PLAYER_Blackjack:
                    m_PlayerWins++;
                    m_PlayerBlackjacks++;
                    Console.Write("\n\tBlackjack for the player!\n");
                    if (betsOn)
                    {
                        Console.Write($" They win ${(int)(player.getBet() * 1.5f)}!\n");
                        player.setBankRoll(player.getBankRoll() + (int)(player.getBet() * 1.5f));
                    }
                    break;
                case (int)Outcomes.DEALER_Blackjack:
                    m_DealerWins++;
                    m_DealerBlackjacks++;
                    Console.Write("\n\tBlackjack for the dealer!\n");
                    if (betsOn)
                    {
                        Console.Write($" They lose ${player.getBet()}!\n");
                        player.setBankRoll(player.getBankRoll() - (int)(player.getBet()));
                    }
                    break;

                case (int)Outcomes.SPLIT_DECIDED:
                    break;
                    
            }
        }
        // Static Functions
        public static void PrintCards(deck_t deck1)
        {
            foreach (var card in deck1)
            {
                card.PrintCard();
                Console.Write(' ');
            }
            Console.Write('\n');
        }

        public static int GetHandValue(deck_t hand)
        {
            int value = 0;
            bool hasAce = false;
            foreach (var card in hand)
            {
                value += card.RankValue();
                if (card.RankValue() == 1)
                {
                    // if has ace in hand
                    hasAce = true;
                }
            }
            if (hasAce && (value + 10) <= 21)
                value += 10;
            return value;
        }

        public static bool CheckBust(deck_t hand)
        {
            return GetHandValue(hand) > 21;
        }

        public static bool checkBlackjack(deck_t hand)
        {
            return (hand[0].RankValue() == 1 || hand[1].RankValue() == 1) && !(hand[0].RankValue() == 1 && hand[1].RankValue() == 1) 
                    && (hand[0].RankValue() == 10 || hand[1].RankValue() == 10); // If 1 card is an ace and the other is 10 value
        }

        public static bool isSplittable(deck_t hand, bool byValue)
        {
            if (byValue)
                if (hand[0].RankValue() == hand[1].RankValue())
                    return true;
            if (hand[0].RankNumber() == hand[1].RankNumber())
                return true;
            return false;

        }

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
