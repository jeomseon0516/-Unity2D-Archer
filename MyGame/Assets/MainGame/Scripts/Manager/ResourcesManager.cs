using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ResourcesManager : SingletonTemplate<ResourcesManager>
{
    private Dictionary<OBJECTID,
        Dictionary<string, GameObject>> _resourceDic = new Dictionary<OBJECTID, Dictionary<string, GameObject>>();
    private bool _isCreate = false;

    /*
     * Awake() 함수 사용 금지
     */
    protected override void Init()
    {
        if (_isCreate) return;
        _isCreate = true;
        AddObject(OBJECTID.ENEMY,  "Prefab",    "Prefabs/Enemy/Enemy");
        AddObject(OBJECTID.ENEMY,  "Bullet",    "Prefabs/Enemy/EnemyBullet");
        AddObject(OBJECTID.PLAYER, "Bullet",    "Prefabs/Bullet");
        AddObject(OBJECTID.FX,     "Smoke",     "Prefabs/FX/Smoke");
        AddObject(OBJECTID.FX,     "HitEffect", "Prefabs/FX/HitEffect");
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
        return _resourceDic[id][key];
    }
    private ResourcesManager() {}
}