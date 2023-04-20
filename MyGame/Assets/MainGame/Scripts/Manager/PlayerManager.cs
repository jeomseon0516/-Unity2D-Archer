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
        SkillUpdate();
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

        if (Input.GetKeyDown(KeyCode.Q))
            _inGamePlayer.FromIndexToSkillAction(0);
    }
    private void SetPlayerActionTrigger(bool condition, string name, string key)
    {
        if (!condition) return;

        _inGamePlayer.StartCoroutineTrigger(name, key);
    }
}

// ..Skill..
// ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.UI, "DogUI");
// Object.Instantiate(skillData).TryGetComponent(out this.skillData)

struct InGameSkillData
{
    public SkillData inGameSkillUi;
    public int index;

    public InGameSkillData(SkillData inGameSkillUi, int index)
    {
        this.inGameSkillUi = inGameSkillUi;
        this.index = index;
    }
}

public partial class PlayerManager : SingletonTemplate<PlayerManager>
{
    // SkillData...UIData 
    Queue<SkillData> _dogSkillUIList = new Queue<SkillData>();
    List<InGameSkillData>  _inGameUIList   = new List<InGameSkillData>();
    List<UI> _skillButtonList = new List<UI>();

    private void SkillInit()
    {
        GameObject dogSkillUi = ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.UI, "DogUI");

        _skillButtonList.Add(UIManager.GetInstance().GetFromKeyToUI("Q_UI"));
        _skillButtonList.Add(UIManager.GetInstance().GetFromKeyToUI("E_UI"));
        _skillButtonList.Add(UIManager.GetInstance().GetFromKeyToUI("R_UI"));

        for (int i = 0; i < 3; ++i)
        {
            SkillData dogSkill = Instantiate(dogSkillUi).AddComponent<SkillData>();

            dogSkill.SetUIPrefab(dogSkillUi);
            _dogSkillUIList.Enqueue(dogSkill);
        }
    }
    private void SkillUpdate()
    {
        for (int i = 0; i < 3; ++i)
        {
            string skillName = _inGamePlayer.GetFromIndexToSkillListValue(i);

            if (CheckUseSkill(i, skillName))
            {
                --i;
                continue;
            }

            if (string.IsNullOrEmpty(skillName))
                continue;

            switch(skillName)
            {
                case "Dog":
                    PushInGameSkillUI(_dogSkillUIList, i);
                    break;
            }
        }
    }

    private bool CheckUseSkill(int index, string skillName)
    {
        if (_inGameUIList.Count <= index || _inGameUIList.Count == 0 || !string.IsNullOrEmpty(skillName)) return false;

        string uiName = _inGameUIList[index].inGameSkillUi.name;

        if (uiName.Contains("Dog"))
            _dogSkillUIList.Enqueue(_inGameUIList[index].inGameSkillUi);

        _inGameUIList.RemoveAt(index);
        return true;
    }
    private void PushInGameSkillUI(Queue<SkillData> skillQueue, int index)
    {
        if (index < 0 || _inGameUIList.Count <= index) return;
        // 이미 생성했다면
        if (_inGameUIList[index].index == index) return;

        print("asdf");

        SkillData dogSkill = skillQueue.Dequeue();
        dogSkill.SetUIParent(_skillButtonList[index].transform);
        dogSkill.Ready();

        InGameSkillData inGameSkillData;

        inGameSkillData.inGameSkillUi = dogSkill;
        inGameSkillData.index = index;

        _inGameUIList.Add(inGameSkillData);
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
