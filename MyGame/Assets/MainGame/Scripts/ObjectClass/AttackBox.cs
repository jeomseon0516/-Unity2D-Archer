using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBJECT;

namespace OBJECT
{
    public class AttackBox : MonoBehaviour
    {
        private ObjectBase _objBase;
        private void OnTriggerEnter2D(Collider2D col) 
        {
            col.transform.parent.Find("Image").TryGetComponent(out ObjectBase obj);

            if (_objBase.TriggerCollision(obj.GetPhysics(), obj))
                obj.TakeDamage(_objBase.GetAtk());
        }
        public void SetObjectBase(ObjectBase obj) { _objBase = obj; }
    }
}