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
            ObjectBase obj = col.transform.parent.Find("Image").GetComponent<ObjectBase>();

            if (_objBase.TriggerCollision(obj.GetPhysics(), obj))
                obj.TakeDamage(_objBase.GetAtk());
        }
        public void SetObjectBase(ObjectBase obj) { _objBase = obj; }
    }
}