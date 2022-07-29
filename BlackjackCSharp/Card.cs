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
    using deck_t = System.Collections.Generic.List<Card>;

    class Deck
    {
        private deck_t _m_deck;
        public deck_t deck { get{ return _m_deck; } }

        public
            Deck(int numDecks, bool shuffled)
        {
            _m_deck = new deck_t(_MakeDeck(numDecks, shuffled));
        }

        private static deck_t _MakeDeck(int numDecks, bool shuffled)
        {
            Card.rank_t[] allRanks =
            {
                Card.rank_t.ace,
                Card.rank_t.two,
                Card.rank_t.three,
                Card.rank_t.four,
                Card.rank_t.five,
                Card.rank_t.six,
                Card.rank_t.seven,
                Card.rank_t.eight,
                Card.rank_t.nine,
                Card.rank_t.ten,
                Card.rank_t.jack,
                Card.rank_t.queen,
                Card.rank_t.king
            };

            Card.suit_t[] allSuits =
            {
                Card.suit_t.hearts,
                Card.suit_t.diamonds,
                Card.suit_t.clubs,
                Card.suit_t.spades
            };

            deck_t deck = new deck_t();

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
            if (!deck.Any())
            {
                Console.WriteLine("All cards have been used. Reshuffling.");
                _m_deck = _MakeDeck(6, true);
                Console.WriteLine("Created new deck.");
            }
            Card returnCard = deck[0];
            deck.Remove(deck[0]);
            return returnCard;
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

        public void PrintCard()
        {
            switch (rank)
            {
                case Card.rank_t.ace: Console.Write('A'); break;
                case Card.rank_t.two: Console.Write('2'); break;
                case Card.rank_t.three: Console.Write('3'); break;
                case Card.rank_t.four: Console.Write('4'); break;
                case Card.rank_t.five: Console.Write('5'); break;
                case Card.rank_t.six: Console.Write('6'); break;
                case Card.rank_t.seven: Console.Write('7'); break;
                case Card.rank_t.eight: Console.Write('8'); break;
                case Card.rank_t.nine: Console.Write('9'); break;
                case Card.rank_t.ten: Console.Write("10"); break;
                case Card.rank_t.jack: Console.Write('J'); break;
                case Card.rank_t.queen: Console.Write('Q'); break;
                case Card.rank_t.king: Console.Write('K'); break;
            }
            switch (suit)
            {
                case Card.suit_t.clubs: Console.Write('C'); break;
                case Card.suit_t.diamonds: Console.Write('D'); break;
                case Card.suit_t.hearts: Console.Write('H'); break;
                case Card.suit_t.spades: Console.Write('S'); break;
            }
        }

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

        private rank_t rank;
        public rank_t Rank { get { return rank; } } // cool getter methods compared to C++
        private suit_t suit;
        public suit_t Suit { get { return suit; } }

        public Card(rank_t r, suit_t s)
        {
            rank = r;
            suit = s;
        }
    }
}


