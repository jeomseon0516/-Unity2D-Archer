using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public class SkillController : ObjectBase
    {
        private Collider2D _col;
        protected override void Init()
        {
            _col = GetComponent<Collider2D>();

            _speed = 0.0f;
            _atk = 5;
            _hp = 1;
        }
        public void OnAttackBox(float isOn)
        {
            bool on = isOn > 0.0f ? true : false;
            _rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
            _col.enabled = on;
        }
        protected internal override void TriggerAction(Collider2D col) { print("asdf"); TriggerCollision(col.transform.parent, col.transform.GetComponent<ObjectBase>()); }
    }
}
