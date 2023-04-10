using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBJECT;
using OBSERVER;

// 플레이어 매니저는 인게임상에 존재하는 플레이어의 정보를 직접적으로 가져오기에 옵저버 패턴을 사용하지 않는 의존적 성향을 지닌 클래스입니다.
public class PlayerManager : SingletonTemplate<PlayerManager>, ISubject
{
    private PlayerController _inGamePlayer;
    private GameObject _inHierarchyPlayer;
    private List<IPlayerObserver> _observers = new List<IPlayerObserver>();
    private int _hp, _maxHp;
    protected override void Init()
    {
        _inHierarchyPlayer = Instantiate(ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.PLAYER, "Player"));
        _inHierarchyPlayer.transform.Find("Body").Find("Image").TryGetComponent(out _inGamePlayer);
        _inHierarchyPlayer.SetActive(false);
        _hp = _maxHp = _inGamePlayer.GetMaxHp();
    }
    private void Update()
    {
        _hp = _inGamePlayer.GetHp();
        NotifyObservers();
    }
    public void RegisterObserver(IPlayerObserver observer)
    {
        if (_observers.Contains(observer)) return; // 가입시키려는 관찰자가 이미 리스트에 들어있다면 리턴
        _observers.Add(observer);
    }
    public void RemoveObserver(IPlayerObserver observer)
    {
        if (!_observers.Contains(observer)) return;
        _observers.Remove(observer);
    }
    public void NotifyObservers()
    {
        for (int i = 0; i < _observers.Count; ++i) _observers[i].UpdateData(_hp, _maxHp);
    }
    public void SetActivePlayer(bool isActive)
    {
        _inHierarchyPlayer.SetActive(isActive);

        if (isActive)
            _inGamePlayer.ResetPlayer();
    }
    public GameObject GetInHierarchyPlayer() { return _inHierarchyPlayer; }
    public PlayerController GetInGamePlayer() { return _inGamePlayer; }
    public void RegisterObserver(IObserver observer) { }
    public void RemoveObserver(IObserver observer) { }
    private PlayerManager() {}
}

public interface IPlayerObserver : IObserver
{
    public void UpdateData(int hp, int maxHp);
}
