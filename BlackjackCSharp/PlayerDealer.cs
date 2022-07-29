using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    class Player
    {
        private Hand _m_Hand;
        public Hand m_Hand { get{ return new Hand(_m_Hand.m_hand); } }
        private Hand _m_Split1;
        public Hand m_Split1 {get { return new Hand(_m_Split1.m_hand); } }
        private Hand _m_Split2;
        public Hand m_Split2 { get{ return new Hand(_m_Split2.m_hand); } }

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
            _m_Hand = new Hand { };
            _m_Split1 = new Hand { };
            _m_Split2 = new Hand { };
        }
    }

    class Dealer
    {
        private Hand _m_Hand;

        public Hand m_Hand
        {
            get
            {
                return new Hand(_m_Hand.m_hand);
            }
        }
        private int _m_DealerStand = 17;

        public Dealer()
        {
            _m_Hand = new Hand();
        }

        public void PrintDealerUpCard()
        {
            Console.Write(_m_Hand.m_CardList[0] + " ??");
        }
        public int DealerUpCardValue() { return _m_Hand.m_CardList[0].RankValue(); }

        public bool AI(Deck deck)
        {
            // Return true if dealer busts, false if not.
            Console.Write("Dealers Turn.\n\tDealer hand: "); 
            Console.Write(_m_Hand);
            while (true)
            {
                if (_m_Hand.CheckBust())
                {
                    Console.WriteLine($"\tDealer hand value : {_m_Hand.GetHandValue()}");
                    return true;
                }
                
                if (_m_Hand.GetHandValue() >= _m_DealerStand)
                {
                    Console.WriteLine($"\tDealer hand value : {_m_Hand.GetHandValue()}");
                    Console.WriteLine("Dealer stands.");
                    return false;
                }
                _m_Hand.m_CardList.Add(deck.DrawCard());
                Console.Write("\tDealer hand: ");
                Console.Write(_m_Hand);
            }
        }
    }
}
