using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class ClientPlayerManager : MonoBehaviour
{
    [SerializeField, RequiredMember] private PlayerData _player;

    [SerializeField, RequiredMember] private List<PlayerData> _otherPlayers;

    public PlayerData Player => _player;

    public PlayerData AddPlayer(string id)
    {
        var freePlayer = _otherPlayers.Find(_ => _.Id == null);
        freePlayer.Id = id;
        // print($"free player, {freePlayer.name} {freePlayer.gameObject.activeSelf}");
        return freePlayer;
    }
}
