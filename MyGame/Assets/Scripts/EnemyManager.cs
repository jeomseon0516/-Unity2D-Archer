using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyManager : SingletonTemplate<EnemyManager>
{
    private GameObject _prefab;
    public  GameObject _parent;

    protected override void Init()
    {
        _parent = new GameObject("EnemyList");
        _prefab = ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.ENEMY, "Prefab");
    }
    private IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);
            GameObject obj = Instantiate(_prefab);
            obj.transform.position = new Vector3(Camera.main.siz)
        }
    }

    private EnemyManager() {}
}