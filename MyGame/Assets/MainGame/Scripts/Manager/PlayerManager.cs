using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBJECT;
using PUB_SUB;

// 플레이어 매니저는 인게임상에 존재하는 플레이어의 정보를 직접적으로 가져오기에 옵저버 패턴을 사용하지 않는 의존적 성향을 지닌 클래스입니다.
public class PlayerManager : SingletonTemplate<PlayerManager>
{
    private PlayerController _inGamePlayer;
    private GameObject _inHierarchyPlayer;
    private PlayerPublisher _playerPublisher;
    private int _hp, _maxHp;
    private int _stamina, _maxStamina;

    protected override void Init()
    {
        _inHierarchyPlayer = Instantiate(ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.PLAYER, "Player"));
        _inHierarchyPlayer.transform.Find("Body").Find("Image").TryGetComponent(out _inGamePlayer);
        _inHierarchyPlayer.SetActive(false);

        _hp = _maxHp = _inGamePlayer.GetMaxHp();
        _stamina = _maxStamina = _inGamePlayer.GetMaxStamina();

        _playerPublisher = new PlayerPublisher();
    }
    private void Start()
    {
        _playerPublisher.UpdateMaxHp(_maxHp);
        _playerPublisher.UpdateHp(_hp);
        _playerPublisher.UpdateMaxStamina(_maxStamina);
        _playerPublisher.UpdateStamina(_stamina);
    }
    private void Update()
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
    private PlayerManager() {}
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
