using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();
    public ushort Id { get; private set; }
    public bool IsLocal { get; private set; }

    [SerializeField] private Transform camTransform;
    [SerializeField] private AnimManager playerAnimationManager; 

    private string username;

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    private void Move(Vector2 newPosition, Vector2 forward)
    {
        transform.position = newPosition;

        if(!IsLocal)
        {
            camTransform.forward = forward;
            playerAnimationManager.AnimateBasedOnSpeed();
        }
    }


    public static void Spawn(ushort id, string username, Vector2 position)
    {
        Player player;

        if(id == NetworkManager.Singleton.Client.Id)
        {
            player = Instantiate(GameLogic.Singleton.LocalPlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = true;
        }
        else
        {
            player = Instantiate(GameLogic.Singleton.PlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = true;
        }

        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.username = username;

        list.Add(id, player);
    }

    #region Messages
    [MessageHandler((ushort)ServerToClientId.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), new Vector2(0,0));
    }

    [MessageHandler((ushort)ServerToClientId.playerMovement)]
    private static void PlayerMovement(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
            player.Move(message.GetVector2(), message.GetVector2());
    }
    #endregion
}
