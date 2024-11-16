using UnityEngine.SceneManagement;

public static class Scene
{
    public static readonly string MENU = "MenuScene";
    public static readonly string SINGLEPLAYER = "SingleplayerScene";
    public static readonly string MULTIPLAYER = "MultiplayerScene";

    public static bool IsMenu() => SceneManager.GetActiveScene().name == MENU;
    public static bool IsSinglePlayer() => SceneManager.GetActiveScene().name == SINGLEPLAYER;
    public static bool IsMultiPlayer() => SceneManager.GetActiveScene().name == MULTIPLAYER;
}