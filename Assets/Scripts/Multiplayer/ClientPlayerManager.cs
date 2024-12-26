using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

public class ClientPlayerManager : MonoBehaviour
{
    [SerializeField, RequiredMember] private PlayerData _player;
    [SerializeField, RequiredMember] private List<PlayerData> _otherPlayers;
    [SerializeField, RequiredMember] private Sprite[] _playerAvatars;

    public PlayerData Player => _player;
    public List<PlayerData> OtherPlayers => _otherPlayers;
    public PlayerData CurrentPlayer { get; set; }
    public Sprite[] PlayerAvatars => _playerAvatars;

    public PlayerData AddPlayer(string id, byte number)
    {
        var freePlayer = _otherPlayers.Find(_ => _.Id == null);
        freePlayer.Id = id;
        //freePlayer.Avatar.SetSprite(MultiplayerGame.Instance.PlayerManager.PlayerAvatars[number]);
        // print($"free player, {freePlayer.name} {freePlayer.gameObject.activeSelf}");
        return freePlayer;
    }

    public bool IsLocalPlayer(PlayerData player)
    {
        return _player.Id == player.Id;
    }

    public bool IsLocalPlayer(string id)
    {
        return _player.Id == id;
    }

    public bool IsLocalPlayersTurn()
    {
        return _player.Id == CurrentPlayer.Id;
    }

    public PlayerData[] GetAllPlayers()
    {
        return OtherPlayers.Concat(new[] { Player }).ToArray();
    }

    public PlayerData GetPlayerById(string id)
    {
        return Player.Id == id ? Player : OtherPlayers.Find(_ => _.Id == id);
    }

    public void OnPlayerWin(string id)
    {
        var player = GetPlayerById(id);
        player.StatusText.AddPlay(Const.WIN_TEXT);
        player.Avatar.Play();
    }
}
