public interface IPlayer
{
    bool GetTurn(); // occurs when player gets their turn, return true to complete turn
    void OnEndTurn();
    void UseCard();
    void Uno();
}

class AIPlayer : IPlayer
{
    public bool GetTurn() { return false; }
    public void OnEndTurn() { }
    public void UseCard() { }
    public void Uno() { }
}