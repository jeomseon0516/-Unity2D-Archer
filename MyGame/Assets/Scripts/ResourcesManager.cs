using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RM = ResourcesManager;

public enum OBJECTID
{
    PLAYER,
    ENEMY,
    BACKGROUND,
    FX
}
/*
 * "Prefabs/Enemy/Enemy"
 * "Prefabs/Bullet"
 */
public sealed class ResourcesManager : SingletonTemplate<ResourcesManager>
{
    private Dictionary<OBJECTID,
        Dictionary<string, GameObject>> _resourceDic = new Dictionary<OBJECTID, Dictionary<string, GameObject>>();
    private bool _isCreate = false; 
    /*
     * 분기 나눠 줘야 함
     */
    protected override void Awake() 
    {
        base.Awake();
        Init();
    }
    private void Init()
    {
        if (_isCreate) return;
        _isCreate = true;
        print("aa");
        AddObject(OBJECTID.ENEMY,  "Prefab", "Prefabs/Enemy/Enemy");
        AddObject(OBJECTID.ENEMY,  "Bullet", "Prefabs/Enemy/EnemyBullet");
        AddObject(OBJECTID.PLAYER, "Bullet", "Prefabs/Bullet");
        AddObject(OBJECTID.FX,     "Smoke",  "Prefabs/FX/Smoke");
    }

    private void AddObject(OBJECTID id, string key, string path)
    {
        // 해당 키가 이마 등록되어있다면?
        if (_resourceDic.ContainsKey(id))
        {
            _resourceDic[id].Add(key, Resources.Load(path) as GameObject);
        }
        else
        {
            _resourceDic.Add(id, CreateDicGameObjectToString(key, path));
        }
    }

    private Dictionary<string, GameObject> CreateDicGameObjectToString(string key, string path)
    {
        var dic = new Dictionary<string, GameObject>();
        dic.Add(key, Resources.Load(path) as GameObject);
        return dic;
    }

    public GameObject GetObjectToKey(OBJECTID id, string key)    
    {
        Init();
        return Instantiate(_resourceDic[id][key]);
    }

    private ResourcesManager() {}
}
