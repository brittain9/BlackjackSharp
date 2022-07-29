
namespace Blackjack
{
    using deck_t = System.Collections.Generic.List<Card>;



    class Program
    {
        static void Main()
        {
            

            Deck deck = new Deck(1, true);
            Player player = new Player();
            Dealer dealer = new Dealer();

            for (var i = 0; i < 10000; i++)
            {
                deck.DrawCard();
            }

            Console.WriteLine(deck);

            // Game loop
            while (!Blackjack.END_GAME && player.m_BankRoll > 0)
            {
                int outcome = Blackjack.PlayBlackjack(player, dealer, deck);
                Blackjack.HandleOutcomes(player, dealer, outcome);

                if (player.m_BankRoll <= 0)
                    Console.WriteLine("you're broke");
                    
                if (player.m_BankRoll > player.m_HighestBankroll)
                    player.m_HighestBankroll = player.m_BankRoll;
            }

        }
    }
}

