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
    protected override void Awake()
    {
        _resourceDic.Add(OBJECTID.ENEMY,  CreateDicGameObjectToString("Prefab", "Prefabs/Enemy/Enemy"));
        _resourceDic.Add(OBJECTID.ENEMY,  CreateDicGameObjectToString("Skill",  "Prefabs/Enemy/EnemyBullet"));
        _resourceDic.Add(OBJECTID.PLAYER, CreateDicGameObjectToString("Bullet", "Prefabs/Bullet"));
        _resourceDic.Add(OBJECTID.FX,     CreateDicGameObjectToString("Smoke",  "Prefabs/FX/Smoke"));
    }

    private Dictionary<string, GameObject> CreateDicGameObjectToString(string key, string path)
    {
        var dic = new Dictionary<string, GameObject>();
        GameObject obj = Resources.Load(path) as GameObject;
        dic.Add(key, obj);
        return dic;
    }

    public GameObject GetObjectToKey(OBJECTID id, string key) { return Instantiate(_resourceDic[id][key]); }
    private ResourcesManager() { }
}
