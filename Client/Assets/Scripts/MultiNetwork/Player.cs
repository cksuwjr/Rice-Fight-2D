using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();   //  < id, Player.cs > ��� �ڷᱸ������
    public ushort Id { get; private set; }  // ���� ID
    public bool IsLocal { get; private set; }  //  �� �÷��̾��ΰ�
    private string username;

    [SerializeField] private AnimManager animManager;       // AnimManager.cs

    private void OnDestroy()
    {
        list.Remove(Id);    
    }
    private void Move(Vector2 newPosition)   //  �����κ��� ServerToClientId.playerMovement ������ Player.cs �ϴ��� �ܿ��� �ش�������� ǥ���� Player.cs ȣ��.
    {
        transform.position = newPosition;                     // �̵� ��ǥ �޾ƿͼ� �̵���Ű��

        if (IsLocal)                                         // �� Player.cs ��
        {
            animManager.AnimateBasedOnSpeed();
        }
    }

    public static void Spawn(ushort id, string username, Vector2 position)   //  �÷��̾� ��ȯ
    {
        Player player;
        if (id == NetworkManager.Singleton.Client.Id)  // �����̸�
        {
            player = Instantiate(GameLogic.Singleton.LocalPlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = true;
        }
        else  // Ÿ���̸�
        {
            player = Instantiate(GameLogic.Singleton.PlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = false;
        }

        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.username = username;

        list.Add(id, player);

    }

    [MessageHandler((ushort)ServerToClientId.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector2());
    }
    [MessageHandler((ushort)ServerToClientId.playerMovement)]
    private static void PlayerMovement(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
            player.Move(message.GetVector2());
    }
}
