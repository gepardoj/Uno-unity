using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

public enum Endpoints : byte
{
    playersInLobby,
    startGame,
}

public class API : MonoBehaviour
{
    [SerializeField, RequiredMember] private TextMeshProUGUI _playersNumber;

    public void OnMessage(byte[] data)
    {
        if (data == null) return;
        var endpoint = (Endpoints)data[0];
        if (endpoint == Endpoints.playersInLobby) OnPlayersInLobby(data);
        else if (endpoint == Endpoints.startGame) OnStartGame();
    }

    public void OnPlayersInLobby(byte[] data)
    {
        var playersInLobby = data[1];
        _playersNumber.text = $"Players: {playersInLobby}";
    }

    public void OnStartGame()
    {
        SceneManager.LoadScene(Scene.MULTIPLAYER);
    }
}