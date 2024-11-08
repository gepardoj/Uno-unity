public interface IPlayerLogic
{
    PlayerData Player { get; }
    void GetTurn(); // occurs when player gets their turn, return true to complete turn
    void OnEndTurn();
    void UseCard(Card card);
    void Uno();
    void OnPullCards();
    void OnChooseColor(SuitColor color);
}