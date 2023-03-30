using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBJECT;

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
/*            int xdir = Random.Range(0, 2) == 0 ? -1 : 1;
            int ydir = Random.Range(0, 2) == 0 ? -1 : 1;

            Vector2 offset  = new Vector2(Random.Range(0, 5), Random.Range(0.0f, 1.5f));
            Vector2 randvec = new Vector2(camera.transform.position.x + ((camera.orthographicSize * camera.aspect) + offset.x) * xdir,
                    0.0f + offset.y * ydir);

            GameObject obj = Instantiate(_prefab);
            obj.transform.position = randvec;
            obj.transform.parent = _parent.transform;*/

            yield return new WaitForSeconds(6.0f);
        }
    }

    private EnemyManager() {}
}