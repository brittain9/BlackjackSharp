
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

            // Game loop
            while (!Blackjack.END_GAME && player.m_BankRoll > 0)
            {
                int outcome = Blackjack.PlayBlackjack(player, dealer, deck);
                Blackjack.HandleOutcomes(player, dealer, outcome);

                Console.WriteLine("\n\n");
                Blackjack.PrintCards(deck.deck);

                if (player.m_BankRoll <= 0)
                    Console.WriteLine("you're broke");
                    
                if (player.m_BankRoll > player.m_HighestBankroll)
                    player.m_HighestBankroll = player.m_BankRoll;
            }

        }
    }
}

