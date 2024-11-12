public interface IPlayerLogic
{
    PlayerData Player { get; }
    /// <summary>
    /// Occurs when player gets their turn, return true to complete turn
    /// </summary>
    /// <param name="shouldDeclareColor"></param>
    /// <param name="prevPlayerSaidUno">Null if there's no need to check condition</param>
    void OnGetTurn(bool? shouldDeclareColor = null, bool? prevPlayerSaidUno = null);
    void OnEndTurn();
    void OnChosenCard(Card card);
    void Uno();
    void OnChosenColor(SuitColor color);
}