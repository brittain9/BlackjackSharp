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

        public Player()
        {
            Hand = new deck_t { };
            Split1 = new deck_t { };
            Split2 = new deck_t { };
        }

        public deck_t getHand()
        {
            return Hand;
        }
        public deck_t getSplit1()
        {
            return Split1;
        }
        public deck_t getSplit2()
        {
            return Split2;
        }
    }

    class Dealer
    {
        private deck_t Hand;

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
            Console.Write(" ??\n");
        }
    }
}
