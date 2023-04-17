using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public class SkillController : ObjectBase
    {
        protected override void Init()
        {
            _speed = 0.0f;
            _atk = 5;
            _hp = 1;
        }
        protected override void OnCollision(ObjectBase obj, Collider2D col)
        {
            Vector2 force = Default.GetFromPostionToDirection(obj.GetPhysics().position, _physics.position);
            obj.TakeDamage(_atk, force * 2);
        }
    }
}
