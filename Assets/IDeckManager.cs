public interface IDeckManager
{
    void DealCardsToPlayer(int playerNumber, int cardCount);
    DeckManager.CardData DealCardToDealer(bool faceUp = true);
    DeckManager.CardData DealCardToPlayer(int playerNumber);
    void ResetDeck();
    void ShuffleDeck();
}