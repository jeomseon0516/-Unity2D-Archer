using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyManager : SingletonTemplate<EnemyManager>
{
    private GameObject _prefab;
    private GameObject _parent;
    protected override void Init()
    {
        _parent = new GameObject("EnemyList");
        _prefab = ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.ENEMY, "Prefab");
    }
    private IEnumerator Start()
    {
        Camera camera = Camera.main;
        while (true)
        {
            yield return new WaitForSeconds(3.0f);

            int xDir = Random.Range(0, 2) == 0 ? -1 : 1;
            int yDir = Random.Range(0, 2) == 0 ? -1 : 1;

            Vector3 offset = new Vector3(Random.Range(0, 5), Random.Range(0.0f, 1.5f), 0.0f);
            Vector3 randVec = new Vector3(
                    camera.transform.position.x + ((camera.orthographicSize * camera.aspect) + offset.x) * xDir,
                    0.0f + offset.y * yDir, 0.0f);

            GameObject obj = Instantiate(_prefab);
            obj.transform.position = randVec;
            obj.transform.parent   = _parent.transform;
        }
    }

    private EnemyManager() {}
}