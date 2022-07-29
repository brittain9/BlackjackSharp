using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    using deck_t = System.Collections.Generic.List<Card>;

    class Player
    {
        private deck_t _m_Hand;
        public deck_t m_Hand { get{ return _m_Hand; }}
        private deck_t _m_Split1;
        public deck_t m_Split1 { get{ return _m_Split1; } }
        private deck_t _m_Split2;
        public deck_t m_Split2 { get{ return _m_Split2; }  }

        private int _m_BankRoll = 1000;
        public int m_BankRoll
        {
            get { return _m_BankRoll;}
            set { _m_BankRoll = value; }
        }
        private int _m_HighestBankroll = 1000;
        public int m_HighestBankroll
        {
            get { return _m_HighestBankroll; }
            set { _m_HighestBankroll = value; }
        }
        private int _m_Bet;
        public int m_Bet
        {
            get { return _m_Bet; }
            set { _m_Bet = value; }
        }
        private int _m_LastBet;
        public int m_LastBet
        {
            get { return _m_LastBet; }
            set { _m_LastBet = value; }
        }
        private int _m_Split1Bet;
        public int m_Split1Bet
        {
            get { return _m_Split1Bet; }
            set { _m_Split1Bet = value; }
        }
        private int _m_Split2Bet;
        public int m_Split2Bet
        {
            get { return _m_Split2Bet; }
            set { _m_Split2Bet = value; }
        }
        private int _m_InsuranceBet;
        public int m_InsuranceBet
        {
            get { return _m_InsuranceBet; }
            set { _m_InsuranceBet = value; }
        }

        public Player()
        {
            _m_Hand = new deck_t { };
            _m_Split1 = new deck_t { };
            _m_Split2 = new deck_t { };
        }
    }

    class Dealer
    {
        private deck_t _m_Hand;
        public deck_t m_Hand {get { return _m_Hand; } }
        private int _m_DealerStand = 17;

        public Dealer()
        {
            _m_Hand = new deck_t { };
        }

        public void PrintDealerUpCard()
        {
            _m_Hand[0].PrintCard(); 
            Console.Write(" ??");
        }
        public int DealerUpCardValue() { return _m_Hand[0].RankValue(); }

        public bool AI(Deck deck)
        {
            // Return true if dealer busts, false if not.
            Console.Write("Dealers Turn.\n\tDealer hand: ");
            Blackjack.PrintCards(_m_Hand);
            while (true)
            {
                if (Blackjack.CheckBust(_m_Hand))
                    return true;
                if (Blackjack.GetHandValue(_m_Hand) >= _m_DealerStand)
                {
                    Console.WriteLine("Dealer stands.");
                    return false;
                }
                _m_Hand.Add(deck.DrawCard());
                Console.Write("\tDealer hand: ");
                Blackjack.PrintCards(_m_Hand);
                Console.WriteLine($"\tDealer hand value : {Blackjack.GetHandValue(_m_Hand)}");
            }
        }
    }
}
