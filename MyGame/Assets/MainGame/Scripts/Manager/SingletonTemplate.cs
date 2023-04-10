using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SINGLETON;
/*
 * TODO : 해당 클래스는 Awake로 초기화 할 수 없습니다.
 */
namespace SINGLETON
{
    public abstract class Singleton : MonoBehaviour { protected abstract void Awake(); }
}

public abstract class SingletonTemplate<T> : Singleton where T : Singleton
{
    private static T _instance = null;
    private static object _obj = new object();

    // 겟 인스턴스와 어웨이크에서 모두 예외처리 해주어야함 하이어라키에 이미 싱글톤이 있는 경우와 없는 경우가 있기때문
    /*
     * 하이어라키에 먼저 들어가있는경우나 하이어라키에 없는 경우를 모두 체크해줘야함
     */
    public static T GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<T>();

            lock (_obj)
            {
                _instance = GetInstanceFindObject();
            }
        }

        return _instance;
    }
    protected sealed override void Awake()
    {
        if (FindObjectOfType<T>() != this)
        {
            Destroy(gameObject);
            return;
        }
        if (_instance == null)
            TryGetComponent(out _instance);

        DontDestroyOnLoad(gameObject);
        Init();
    }
    private static T GetInstanceFindObject() { return _instance == null ? new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>() : _instance; }
    protected abstract void Init();
    protected SingletonTemplate() {}
}
