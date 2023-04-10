using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBJECT;

public sealed class EnemyManager : SingletonTemplate<EnemyManager>
{
    private GameObject _prefab;
    private Transform _parent;
    private bool _isMake;
    protected override void Init()
    {
        _parent = GameObject.Find("EnemyList").transform;
        _prefab = ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.ENEMY, "Prefab");
        _isMake = false;
    }
    private IEnumerator Start()
    {
        Camera camera = Camera.main;

        while (true)
        {
            MakingEnemy(camera);
            yield return YieldCache.WaitForSeconds(6.0f);
        }
    }
    void MakingEnemy(Camera camera)
    {
        if (!_isMake) return;

        int xDir = Random.Range(0, 2) == 0 ? -1 : 1;
        int yDir = Random.Range(0, 2) == 0 ? -1 : 1;

        Vector2 offset  = new Vector2(Random.Range(0, 5), Random.Range(0.0f, 1.5f));
        Vector2 randvec = new Vector2(camera.transform.position.x + ((camera.orthographicSize * camera.aspect) + offset.x) * xDir,
                0.0f + offset.y * yDir);

        Transform obj = Instantiate(_prefab).transform;
        obj.position = randvec;
        obj.parent = _parent;
    }
    public void SetIsMakeEnemy(bool isMake)
    {
        _isMake = isMake;

        if (!isMake)
            return;

        for (int i = 0; i < _parent.childCount; ++i)
        {
            _parent.GetChild(i).gameObject.SetActive(true);
        }
    }
    private EnemyManager() {}
}