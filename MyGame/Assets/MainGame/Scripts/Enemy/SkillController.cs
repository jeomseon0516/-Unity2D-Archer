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
            _bodyCollider.enabled = on;
        }
        protected internal override void TriggerAction(Collider2D col) 
        {
            ObjectBase obj = col.transform.Find("Image").GetComponent<ObjectBase>();

            if (TriggerCollision(col.transform.parent, obj))
                obj.TakeDamage(_atk);
        }
    }
}
