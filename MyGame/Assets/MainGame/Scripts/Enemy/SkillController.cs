using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public class SkillController : ObjectBase
    {
        private GameObject _attackBox;
        private Collider2D _col;
        protected override void Init()
        {
            _attackBox = transform.parent.gameObject;
            _attackBox.gameObject.AddComponent<AttackBox>().SetObjectBase(this);

            _col = _attackBox.GetComponent<Collider2D>();

            _speed = 0.0f;
            _atk = 5;
            _hp = 1;
        }
        public void OnAttackBox(float isOn)
        {
            bool on = isOn > 0.0f ? true : false;
            _col.enabled = on;
        }
        protected internal override void TriggerAction(Collider2D col) { TriggerCollision(col.transform.parent.gameObject.transform, col.transform.GetComponent<ObjectBase>()); }
    }
}
