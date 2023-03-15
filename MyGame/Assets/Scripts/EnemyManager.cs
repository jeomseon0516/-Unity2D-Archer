using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EM = EnemyManager; // EnemyManager
public sealed class EnemyManager : SingletonTemplate<EnemyManager>
{
    private GameObject _prefab;
    public  GameObject _parent;
    protected override void Awake()
    { 
        _parent = new GameObject("EnemyList");
        _prefab = ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.ENEMY, "Enemy");
    }

    private IEnumerator Start()
    {
        while(true)
        {
            GameObject obj = Instantiate(_prefab);

        }
    }
    void Update()
    {
        
    }

    private EnemyManager() {}
}