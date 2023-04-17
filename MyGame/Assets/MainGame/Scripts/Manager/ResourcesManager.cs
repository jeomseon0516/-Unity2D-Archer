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
        AddObject(OBJECTID.PLAYER,  "Player",      "Prefabs/Player/Player");
        AddObject(OBJECTID.PLAYER,  "GroundSmoke", "Prefabs/Player/GroundSmoke");
        AddObject(OBJECTID.PLAYER,  "Bullet",      "Prefabs/Player/Bullet");
        AddObject(OBJECTID.PLAYER,  "DogSkill",    "Prefabs/Player/DogSkill");
        AddObject(OBJECTID.ENEMY,   "Prefab",      "Prefabs/Enemy/Enemy");
        AddObject(OBJECTID.ENEMY,   "Bullet",      "Prefabs/Enemy/EnemyBullet");
        AddObject(OBJECTID.PENGUIN, "Bullet",      "Prefabs/Enemy/Penguin/SnowBall");
        AddObject(OBJECTID.FX,      "Smoke",       "Prefabs/FX/Smoke");
        AddObject(OBJECTID.FX,      "HitEffect",   "Prefabs/FX/HitEffect");
    }
    private void AddObject(OBJECTID id, string key, string path)
    {
        // 해당 키가 등록되어 있지 않다면?
        if (!_resourceDic.TryGetValue(id, out Dictionary<string, GameObject> fromStringToObjList))
            _resourceDic.Add(id, fromStringToObjList = new Dictionary<string, GameObject>());
  
        if (!fromStringToObjList.TryGetValue(key, out GameObject obj))
            fromStringToObjList.Add(key, Resources.Load(path) as GameObject);
    }
    public GameObject GetObjectToKey(OBJECTID id, string key)    
    {
        Init();
        return _resourceDic[id][key];
    }
    private ResourcesManager() {}
}