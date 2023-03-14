using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using ENEMYMANAGER = EnemyManager.GetInstance();
public class EnemyManager : MonoBehaviour
{
    private static EnemyManager _instance = null;
    public static EnemyManager GetInstance()
    {
        return _instance = _instance == null ? new EnemyManager() : _instance;   
    }

    GameObject _prefab;
    private void Awake()
    { 
        DontDestroyOnLoad(gameObject);

        _prefab = Resources.Load("Prefabs/Enemy/Enemy") as GameObject;
    }

    void Start()
    {
        
    }
    void Update()
    {
        
    }
}