using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();   //  < id, Player.cs > 담는 자료구조생성
    public ushort Id { get; private set; }  // 개인 ID
    public bool IsLocal { get; private set; }  //  내 플레이어인가
    private string username;
    public string Character;

    [SerializeField] private MoveController moveController;
    [SerializeField] private AnimManager animManager;
    [SerializeField] private PlayerSkill playerSkill;
    [SerializeField] private PlayerUIManager playerUIManager;

    Rigidbody2D rb;
    private void OnDestroy()
    {
        list.Remove(Id);    
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    public void Gethurt(float damage)
    {
        Debug.Log(Id + "데미지" + damage + "달음");
        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.GetDamage);
        message.AddUShort(Id);
        message.AddFloat(damage);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void SetState(string state)
    {
        switch (state) {
            case "Nothing":
                moveController.state = MoveController.State.Nothing;
                break;
            case "Charging": // AttackCantMove
                moveController.state = MoveController.State.Charging;
                break;
            case "AttackMoving": // AttackCanMove
                moveController.state = MoveController.State.AttackMoving;
                break;
        }
    }











    public static void Spawn(ushort id, string username, string chartype, Vector2 position)   //  플레이어 소환
    {
        GameObject CharacterPrefab;
        switch (chartype) {
            case "Lection":
                CharacterPrefab = GameLogic.Singleton.Lection;
                break;
            case "Kara":
                CharacterPrefab = GameLogic.Singleton.Kara;
                break;
            case "Crollo":
                CharacterPrefab = GameLogic.Singleton.Crollo;
                break;
            default:
                CharacterPrefab = GameLogic.Singleton.LocalPlayerPrefab;
                break;
        }


        Player player;
        player = Instantiate(CharacterPrefab, position, Quaternion.identity).GetComponent<Player>(); //GameLogic.Singleton.LocalPlayerPrefab
        if (id == NetworkManager.Singleton.Client.Id)  // 본인이면
        {
            player.IsLocal = true;
        }
        else
        {
            player.IsLocal = false;
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            Destroy(player.transform.Find("PlayerUI").gameObject);
            Destroy(player.transform.Find("camProxy").gameObject);
            GameObject playerUI = Instantiate(GameLogic.Singleton.OtherPlayerUI, player.transform);
            player.playerUIManager.UIReSetting(player, playerUI.transform.GetChild(0), playerUI.transform.GetChild(2));
        }
        
        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.username = username;
        player.Character = chartype;
        player.playerUIManager.SetNickname($" {(string.IsNullOrEmpty(username) ? $"Player {id} (Guest)" : username)}");
        list.Add(id, player);
    }

    public static void Move(ushort id, int direction, bool jump)
    {
        if (list.TryGetValue(id, out Player player))
        {
            if (direction != 0 && player.moveController.state == MoveController.State.Nothing)
                player.transform.GetChild(1).localScale = new Vector3(-direction, 1, 1);

            if(NetworkManager.Singleton.Client.Id == id)
                player.moveController.Move(direction, jump);
        }
    }
    public void PositionUpdated(Vector3 position)
    {
        rb.MovePosition(position); 
        // 1.  transform.position = position;     오브젝트 위치 즉시 순간이동,  모든 콜리더가 리지드바디 위치 재계산, 성능저하
        // 2.  rb.position = position;            다음 물리 시뮬레이션 이후 오브젝트 위치 순간이동, 10배 정도 tranfrom보다 빠름
        // 3.  rb.MovePosition(position);         자연스러운 움직임
    }
    [MessageHandler((ushort)ServerToClientId.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(),message.GetString() ,message.GetVector2());
    }

    [MessageHandler((ushort)ServerToClientId.Move)]
    private static void SendMoveAccept(Message message)
    {//NetworkManager.Singleton.Client.Id
        Move(message.GetUShort(), message.GetInt(), message.GetBool());
    }

    [MessageHandler((ushort)ServerToClientId.PoseUpdate)]
    private static void PoseUpdateAccept(Message message)
    {
        ushort id = message.GetUShort();
        if (list.TryGetValue(id, out Player player))
        {
            if(id != NetworkManager.Singleton.Client.Id)
                player.transform.position = message.GetVector2();
        }
    }
    [MessageHandler((ushort)ServerToClientId.AnimUpdate)] // 나를 제외한 다른놈들의 애니메이션 업데이트
    private static void AnimUpdateAccept(Message message)
    {
        ushort id = message.GetUShort();
        bool[] animParameter = message.GetBools(3);
        if (list.TryGetValue(id, out Player player))
        {
            if (id != NetworkManager.Singleton.Client.Id)
            {
                player.animManager.AnimSetMove(animParameter[0], animParameter[1], animParameter[2]);
            }
                
        }
    }

    [MessageHandler((ushort)ServerToClientId.SkillReady)]
    private static void SkillAccept(Message message) // 반드시 메시지 전달순서대로 Get-- 할것, 아니면 오류남
    {
        if (list.TryGetValue(NetworkManager.Singleton.Client.Id, out Player player))
        {
            player.playerSkill.SkillReady(message.GetInt(), message.GetString());
        }
    }
    [MessageHandler((ushort)ServerToClientId.SkillStart)]
    private static void playerSkillStartAccept(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.playerSkill.Skill(message.GetInt(), message.GetString());
        }
    }
    [MessageHandler((ushort)ServerToClientId.playerUIUpdate)]
    private static void playerUIUpdateAccept(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.playerUIManager.UIUpdate(message.GetFloat(), message.GetFloat(), message.GetFloat());
        }
    }

}
