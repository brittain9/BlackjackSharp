using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Blackjack;

/*
 * class: reference type, created on heap and garbage collected. Cannot put in array, must use list.
 * struct: value type, created on stack.
 *  We can mess with this and see performance difference. I like C++ in classes and structs cause I tell it exactly what to do.
 */

namespace Blackjack
{

    public enum suit_t
    {
        hearts,
        diamonds,
        clubs,
        spades
    };

    public enum rank_t
    {
        ace = 1,
        two,
        three,
        four,
        five,
        six,
        seven,
        eight,
        nine,
        ten,
        jack,
        queen,
        king
    };

    class CardList
    {
        protected List<Card> _m_CardList;

        public List<Card> m_CardList
        {
            get { return _m_CardList; }
            set { _m_CardList = value; }
        }

        public override string ToString()
        {
            string str = "";
            foreach (var card in m_CardList)
            {
                str += card.ToString() + " ";
            }
            return str;
        }
    }

    class Deck : CardList
    {
        public List<Card> m_Deck
        {
            get { return _m_CardList; }
            set { _m_CardList = value; }
        }

        public
            Deck(int numDecks, bool shuffled)
        {
            _m_CardList = new List<Card>(_MakeDeck(numDecks, shuffled));
        }

        private static List<Card> _MakeDeck(int numDecks, bool shuffled)
        {
            rank_t[] allRanks =
            {
                rank_t.ace,
                rank_t.two,
                rank_t.three,
                rank_t.four,
                rank_t.five,
                rank_t.six,
                rank_t.seven,
                rank_t.eight,
                rank_t.nine,
                rank_t.ten,
                rank_t.jack,
                rank_t.queen,
                rank_t.king
            };

            suit_t[] allSuits =
            {
                suit_t.hearts,
                suit_t.diamonds,
                suit_t.clubs,
                suit_t.spades
            };

            List<Card> deck = new List<Card>();

            for (int i = 0; i < numDecks; i++)
            {
                foreach (var rank in allRanks)
                {
                    foreach (var suit in allSuits)
                    {
                        Card card = new Card(rank, suit);
                        deck.Add(card);
                    }
                }
            }
            if (shuffled)
            {
                Random rng = new Random();

                int n = deck.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    Card tempCard = deck[k];
                    deck[k] = deck[n];
                    deck[n] = tempCard;
                }
            }
            return deck;
        }
        public Card DrawCard()
        {
            if (!m_Deck.Any())
            {
                Console.WriteLine("All cards have been used. Reshuffling.");
                m_Deck = _MakeDeck(6, true);
                Console.WriteLine("Created new deck.");
            }
            Card returnCard = m_Deck[0];
            m_Deck.Remove(m_Deck[0]);
            return returnCard;
        }
    }

    class Hand : CardList
    {
        public List<Card> m_hand
        {
            get { return _m_CardList; }
            set { _m_CardList = value; }
        }

        public Hand()
        {
            _m_CardList = new List<Card>() { };
        }

        public Hand(List<Card> hand)
        {
            _m_CardList = hand;
        }

        public int GetHandValue()
        {
            int value = 0;
            bool hasAce = false;
            foreach (var card in m_hand)
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

        public bool CheckBust()
        {
            return GetHandValue() > 21;
        }

        public bool CheckBlackjack()
        {
            return (m_hand[0].RankValue() == 1 || m_hand[1].RankValue() == 1) && !(m_hand[0].RankValue() == 1 && m_hand[1].RankValue() == 1)
                                                                              && (m_hand[0].RankValue() == 10 || m_hand[1].RankValue() == 10); // If 1 card is an ace and the other is 10 value
        }

        public bool isSplittable(bool byValue)
        {
            if (byValue)
                if (m_hand[0].RankValue() == m_hand[1].RankValue())
                    return true;
            if (m_hand[0].RankNumber() == m_hand[1].RankNumber())
                return true;
            return false;
        }
    }

    struct Card
    {
        public int RankValue()
        {
            switch (rank)
            {
                case rank_t.ace:
                    return 1;
                case rank_t.two:
                    return 2;
                case rank_t.three:
                    return 3;
                case rank_t.four:
                    return 4;
                case rank_t.five:
                    return 5;
                case rank_t.six:
                    return 6;
                case rank_t.seven:
                    return 7;
                case rank_t.eight:
                    return 8;
                case rank_t.nine:
                    return 9;
                case rank_t.ten:
                case rank_t.jack:
                case rank_t.queen:
                case rank_t.king:
                    return 10;
            }
            return 0;
        }
        public int RankNumber()
        {
            switch (rank)
            {
                case rank_t.ace:
                    return 1;
                case rank_t.two:
                    return 2;
                case rank_t.three:
                    return 3;
                case rank_t.four:
                    return 4;
                case rank_t.five:
                    return 5;
                case rank_t.six:
                    return 6;
                case rank_t.seven:
                    return 7;
                case rank_t.eight:
                    return 8;
                case rank_t.nine:
                    return 9;
                case rank_t.ten:
                    return 10;
                case rank_t.jack:
                    return 11;
                case rank_t.queen:
                    return 12;
                case rank_t.king:
                    return 13;
            }
            return 0;
        }

        public override string ToString()
        {
            string card = "";
            switch (rank)
            {
                case rank_t.ace: card += 'A'; break;
                case rank_t.two: card += '2'; break;
                case rank_t.three: card += '3'; break;
                case rank_t.four: card += '4'; break;
                case rank_t.five: card += '5'; break;
                case rank_t.six: card += '6'; break;
                case rank_t.seven: card += '7'; break;
                case rank_t.eight: card += '8'; break;
                case rank_t.nine: card += '9'; break;
                case rank_t.ten: card += "10"; break;
                case rank_t.jack: card += 'J'; break;
                case rank_t.queen: card += 'Q'; break;
                case rank_t.king: card += 'K'; break;
            }
            switch (suit)
            {
                case suit_t.clubs: card += 'C'; break;
                case suit_t.diamonds: card += 'D'; break;
                case suit_t.hearts: card += 'H'; break;
                case suit_t.spades: card += 'S'; break;
            }
            return card;
        }

        private rank_t rank;
        public rank_t Rank { get { return rank; } }
        private suit_t suit;
        public suit_t Suit { get { return suit; } }

        public Card(rank_t r, suit_t s)
        {
            rank = r;
            suit = s;
        }
    }
}


