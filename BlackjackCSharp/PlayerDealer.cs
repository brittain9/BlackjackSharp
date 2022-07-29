using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    using deck_t = System.Collections.Generic.List<Card>;

    class Player
    {
        private deck_t Hand;
        private deck_t Split1;
        private deck_t Split2;

        private int m_BankRoll = 1000;
        private int m_HighestBankroll = 1000;
        private int m_Bet = 0;
        private int m_LastBet = 0;
        private int m_Split1Bet = 0;
        private int m_Split2Bet = 0;
        private int m_InsuranceBet = 0;
        

        public Player()
        {
            Hand = new deck_t { };
            Split1 = new deck_t { };
            Split2 = new deck_t { };
        }

        // Getters and setters
        public deck_t getHand() { return Hand; }
        public deck_t getSplit1() { return Split1; }
        public deck_t getSplit2() { return Split2; }
        public int getBankRoll() { return m_BankRoll;}
        public int getHighBankRoll() { return m_HighestBankroll; }
        public int getBet() { return m_Bet; }
        public int getLastBet() { return m_LastBet; }
        public int getSplit1Bet()  { return m_Split1Bet; }
        public int getSplit2Bet() { return m_Split2Bet; }
        public int getInsureBet() { return m_InsuranceBet; }

        public void setBankRoll(int num) { m_BankRoll = num; }
        public void setHighBankRoll(int num) { m_HighestBankroll = num; }
        public void setBet(int num) { m_Bet = num; }
        public void setLastBet(int num) { m_LastBet = num; }
        public void setSplit1Bet(int num) { m_Split1Bet = num; }
        public void setSplit2Bet(int num) { m_Split2Bet = num; }
        public void setInsureBet(int num) { m_InsuranceBet = num; }
    }

    class Dealer
    {
        private deck_t Hand;
        private int DealerStand = 17;

        public Dealer()
        {
            Hand = new deck_t { };
        }

        public deck_t getHand()
        {
            return Hand;
        }

        public void PrintDealerUpCard()
        {
            Hand[0].PrintCard(); 
            Console.Write(" ??");
        }
        public int DealerUpCardValue() { return Hand[0].RankValue(); }


        public bool AI(ref Deck deck)
        {
            // Return true if dealer busts, false if not.
            Console.Write("Dealers Turn.\n\tDealer hand: ");
            Blackjack.PrintCards(Hand);
            while (true)
            {
                if (Blackjack.CheckBust(Hand))
                    return true;
                if (Blackjack.GetHandValue(Hand) >= DealerStand)
                {
                    Console.WriteLine("Dealer stands.");
                    return false;
                }
                Hand.Add(deck.DrawCard());
                Console.Write("\tDealer hand: ");
                Blackjack.PrintCards(Hand);
                Console.WriteLine($"\tDealer hand value : {Blackjack.GetHandValue(Hand)}");
            }
        }
    }
}
