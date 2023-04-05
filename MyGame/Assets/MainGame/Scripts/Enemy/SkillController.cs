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
        public void OnAttackBox(float isOn)
        {
            bool on = isOn > 0.0f ? true : false;
            _colTransform.gameObject.SetActive(on);
        }
        protected internal override void OnCollision(ObjectBase obj, Collider2D col)
        {
            obj.TakeDamage(_atk);
        }
    }
}
