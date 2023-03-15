using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonTemplate<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T _instance = null;
    private static object _obj = new object();

    public static T GetInstance() 
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType(typeof(T)) as T;
            /*
             * 추상 클래스로 싱글톤 생성시 생기는 문제점 new 키워드로 생성시 생성자를 public으로 선언해야 한다.
             * 그것을 방지하기 위해 인스턴스를 Activator클래스의 CreateInstance를 사용한다 데이터 타입을 받아와 해당 데이터 타입과 같은 사이즈의 인스턴스를 생성하여 반환해준다.
             */

            // 멀티쓰레드 방지 instance가 null일때 쓰레드가 해당 함수를 동시에 여러번 호출하면 싱글톤 객체가 두개 이상이 되기 때문에
            lock (_obj)
            { 
                _instance = _instance == null ? new GameObject(typeof(T).ToString(), typeof(T)).AddComponent<T>() : _instance;
                print("aa");
            }
        }
        return _instance;
    }
    protected virtual void Awake()
    {
        // 하이어라키에서 T형 데이터 타입을 갖고있는 오브젝트를 가져온다.
        // 해당 타입이 이미 존재하면 디스트로이
        if (FindObjectsOfType<T>().Length > 0)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    protected SingletonTemplate() {}
}