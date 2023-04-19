using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBJECT;
using PUB_SUB;

// 플레이어 매니저는 인게임상에 존재하는 플레이어의 정보를 직접적으로 가져오기에 옵저버 패턴을 사용하지 않는 의존적 성향을 지닌 클래스입니다.
public partial class PlayerManager : SingletonTemplate<PlayerManager>
{
    protected override void Init()
    {
        StatusInit();
        SkillInit();
    }
    private void Start()
    {
        StatusStart();
    }
    private void Update()
    {
        StatusUpdate();
    }
    private PlayerManager() {}
}
// ..Skill..
public partial class PlayerManager : SingletonTemplate<PlayerManager>
{
    private PlayerController _inGamePlayer;
    private GameObject _inHierarchyPlayer;
    private PlayerPublisher _playerPublisher;

    private int _hp, _maxHp, _stamina, _maxStamina;

    private void StatusStart()
    {
        _playerPublisher.UpdateMaxHp(_maxHp);
        _playerPublisher.UpdateHp(_hp);
        _playerPublisher.UpdateMaxStamina(_maxStamina);
        _playerPublisher.UpdateStamina(_stamina);
    }
    private void StatusInit()
    {
        _inHierarchyPlayer = Instantiate(ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.PLAYER, "Player"));
        _inHierarchyPlayer.transform.Find("Body").Find("Image").TryGetComponent(out _inGamePlayer);
        _inHierarchyPlayer.SetActive(false);

        _hp = _maxHp = _inGamePlayer.GetMaxHp();
        _stamina = _maxStamina = _inGamePlayer.GetMaxStamina();

        _playerPublisher = new PlayerPublisher();
    }
    private void StatusUpdate()
    {
        _playerPublisher.UpdateHp(_hp = _inGamePlayer.GetHp());
        _playerPublisher.UpdateStamina(_stamina = _inGamePlayer.GetStamina());
    }
    public void SetActivePlayer(bool isActive)
    {
        _inHierarchyPlayer.SetActive(isActive);

        if (isActive)
            _inGamePlayer.ResetPlayer();
    }
    public GameObject GetInHierarchyPlayer() { return _inHierarchyPlayer; }
    public PlayerController GetInGamePlayer() { return _inGamePlayer; }
    public PlayerPublisher GetPlayerPublisher() { return _playerPublisher; }
}
// ..Input..
public partial class PlayerManager : SingletonTemplate<PlayerManager>
{
    private void FixedUpdate()
    {
        Vector2 moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 arrow         = new Vector2(Input.GetAxisRaw("FireHorizontal"), Input.GetAxisRaw("FireVertical"));

        _inGamePlayer.SetDirection(moveDirection);
        _inGamePlayer.SetFireDirection(arrow);

        SetPlayerActionTrigger(Input.GetKey(KeyCode.Space), "OnJump", "Jump");
        SetPlayerActionTrigger(Input.GetKeyDown(KeyCode.LeftShift), "Dash", "Dash");
        SetPlayerActionTrigger(Input.GetKeyDown(KeyCode.Q), "Dog", "Dog");
    }
    private void SetPlayerActionTrigger(bool condition, string name, string key)
    {
        if (!condition) return;

        _inGamePlayer.StartCoroutineTrigger(name, key);
    }
}

// ..Skill..
struct SkillInventory
{
    public KeyCode key;
    public SkillData skillData;
    public PlayerController.PLR_STATE _checkState;
    public string skillName;

    public SkillInventory(KeyCode key)
    {
        this.key = key;
        skillName = "";
        skillData = null;
        _checkState = PlayerController.PLR_STATE.RUN;
    }
}
// ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.UI, "DogUI");
// Object.Instantiate(skillData).TryGetComponent(out this.skillData)
public partial class PlayerManager : SingletonTemplate<PlayerManager>
{
    public enum SKILL_KIND
    { 
        DOG
    }

    // SkillData...UIData, skillArray 
    SkillData[] _dogSkill;

    private SkillInventory[] _skillInventory;
    private int _index;

    private void SkillInit()
    {
        _skillInventory = new SkillInventory[4];
        _dogSkill = new SkillData[4];

        GameObject skillList = GameObject.Find("SkillList");

        for (int i = 0; i < 3; ++i)
        {
            GameObject obj = new GameObject();
            _dogSkill[i] = obj.AddComponent<SkillData>();
        }

        _skillInventory[0] = new SkillInventory(KeyCode.Q);
        _skillInventory[1] = new SkillInventory(KeyCode.E);
        _skillInventory[2] = new SkillInventory(KeyCode.R);
        _index = 0;
    }
    private void SkillUpdate()
    {

    }
    private IEnumerator SelectRandomSkill(SKILL_KIND kind)
    {
        while (true)
        {
            yield return YieldCache.WaitForSeconds(Random.Range(2.0f, 4.0f));

            switch (kind)
            {
                case SKILL_KIND.DOG:
                    /*
                        ..구현..
                        GC에 수집되지 않게 미리 적당한 양의 스킬들을 캐싱해둔다.
                        사용이 가능한 스킬들은 _skillInventory로 옮겨둔다.
                        UI에 신호를 주고 UI에서 스킬의 사용준비가 되면 SkillData에서 true값을 반환해서 받아온다 또는 Queue로 구현해서 가져온다. 
                        SkillData는 자신이 어떤 스킬인지 변수로 가지고 있는다.
                        스킬이 사
                    */
                    break;
            }
        }
    }
}
public class PlayerPublisher
{
    int _hp, _maxHp, _stamina, _maxStamina;

    private List<IHpSubscriber> _hpSubscribers = new List<IHpSubscriber>();
    private List<IStaminaSubscriber> _staminaSubscribers = new List<IStaminaSubscriber>();

    public void RegisterIHpSubscriber(IHpSubscriber hpSubscriber)
    {
        if (_hpSubscribers.Contains(hpSubscriber)) return; // 가입시키려는 관찰자가 이미 리스트에 들어있다면 리턴
        _hpSubscribers.Add(hpSubscriber);
    }
    public void RemoveIHpSubscriber(IHpSubscriber hpSubscriber)
    {
        if (!_hpSubscribers.Contains(hpSubscriber)) return;
        _hpSubscribers.Remove(hpSubscriber);
    }
    public void RegisterIStaminaSubscriber(IStaminaSubscriber staminaSubscriber)
    {
        if (_staminaSubscribers.Contains(staminaSubscriber)) return; // 가입시키려는 관찰자가 이미 리스트에 들어있다면 리턴
        _staminaSubscribers.Add(staminaSubscriber);
    }
    public void RemoveIStaminaSubscriber(IStaminaSubscriber staminaSubscriber)
    {
        if (!_staminaSubscribers.Contains(staminaSubscriber)) return;
        _staminaSubscribers.Remove(staminaSubscriber);
    }
    public void UpdateHp(int hp)
    {
        if (_hp == hp) return;

        _hp = hp;

        for (int i = 0; i < _hpSubscribers.Count; ++i) 
            _hpSubscribers[i].OnUpdateHp(_hp);
    }
    public void UpdateMaxHp(int maxHp)
    {
        _maxHp = maxHp;

        for (int i = 0; i < _hpSubscribers.Count; ++i) 
            _hpSubscribers[i].OnUpdateMaxHp(_maxHp);
    }
    public void UpdateStamina(int stamina)
    {
        if (_stamina == stamina) return;

        _stamina = stamina;

        for (int i = 0; i < _staminaSubscribers.Count; ++i)
            _staminaSubscribers[i].OnUpdateStamina(_stamina);
    }
    public void UpdateMaxStamina(int maxStamina)
    {
        _maxStamina = maxStamina;

        for (int i = 0; i < _staminaSubscribers.Count; ++i)
            _staminaSubscribers[i].OnUpdateMaxStamina(_maxStamina);
    }
}
public interface IStaminaSubscriber
{
    void OnUpdateStamina(int stamina);
    void OnUpdateMaxStamina(int maxStamina);
}
