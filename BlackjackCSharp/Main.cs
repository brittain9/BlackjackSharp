
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
            while (!Blackjack.END_GAME && player.getBankRoll() > 0)
            {
                int outcome = Blackjack.PlayBlackjack(ref player,ref dealer,ref deck);
                Blackjack.HandleOutcomes(player, dealer, outcome);

                if (player.getBankRoll() <= 0)
                    Console.WriteLine("you're broke");
                    
                if (player.getBankRoll() > player.getHighBankRoll())
                    player.setHighBankRoll(player.getBankRoll());
            }

        }
    }
}

