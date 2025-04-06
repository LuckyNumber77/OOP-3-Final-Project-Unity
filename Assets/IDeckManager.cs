public interface IDeckManager
{
    void DealCardsToPlayer(int playerNumber, int cardCount);
    DeckManager.CardData DealCardToDealer();
    DeckManager.CardData DealCardToPlayer(int playerNumber);
    void ResetDeck();
    void ShuffleDeck();
}