
namespace Blackjack
{
    using deck_t = System.Collections.Generic.List<Card>;

    class Program
    {
        static void Main()
        {
            Deck deck = new Deck(6, true);

            Player player = new Player();
            Dealer dealer = new Dealer();

            player.getHand().Add(deck.DrawCard());
            dealer.getHand().Add(deck.DrawCard());
            player.getHand().Add(deck.DrawCard());
            dealer.getHand().Add(deck.DrawCard());

            dealer.PrintDealerUpCard();
            Deck.PrintCards(player.getHand());

            player.getHand().Clear();
            dealer.getHand().Clear();
            Console.WriteLine();


            player.getHand().Add(deck.DrawCard());
            dealer.getHand().Add(deck.DrawCard());
            player.getHand().Add(deck.DrawCard());
            dealer.getHand().Add(deck.DrawCard());

            dealer.PrintDealerUpCard();
            Deck.PrintCards(player.getHand());

        }
    }
}

