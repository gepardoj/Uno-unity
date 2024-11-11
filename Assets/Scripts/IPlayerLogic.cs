public interface IPlayerLogic
{
    PlayerData Player { get; }
    void OnGetTurn(bool? shouldDeclareColor); // occurs when player gets their turn, return true to complete turn
    void OnEndTurn();
    void OnChooseCard(Card card);
    void Uno();
    void OnChosenColor(SuitColor color);
}