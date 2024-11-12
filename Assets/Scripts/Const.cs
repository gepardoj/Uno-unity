public static class Const
{
    public static readonly string SKIP_TEXT = "Skip";
    public static string DRAW_TEXT(int n) => $"Draw {n}";
    public static readonly string REVERSE_TEXT = "Reverse";
    public static string SHUFFLED_TEXT = "Shuffled cards";
    public static string UNO_TEXT = "Uno";
    public static string WIN_TEXT = "Win";

    public static readonly int START_CARDS_N = 7;
    public static readonly int DRAW_CARDS_N = 1;
    public static readonly int SKIP_DRAW_CARDS_N = 2;
    public static readonly int WILDDRAW_CARDS_N = 4;
    public static readonly int UNO_PENALTY_N = 2;
}