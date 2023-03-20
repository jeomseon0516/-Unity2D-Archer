using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : LivingObject
{
    protected override void Init()
    {
        _hp = 5;
    }
    protected override void Run()
    {
        base.Run();
    }
    protected override void CreateBullet()
    {

    }
    protected override void Die()
    {
        base.Die();
    }
    protected override void ObjUpdate()
    {
        base.ObjUpdate();
    }
    // TODO : HitAnimation재생
    protected override void CollisionAction(Collision2D obj)
    {
        if (LayerMask.LayerToName(obj.gameObject.layer) != "Bullet") return;

        --_hp;
    }
}
