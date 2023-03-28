using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBJECT;

namespace OBJECT
{
    public class AttackBox : MonoBehaviour
    {
        private ObjectBase _objBase;
        private void OnTriggerEnter2D(Collider2D col) { _objBase.TriggerAction(col); }
        public void SetObjectBase(ObjectBase obj) { _objBase = obj; }
    }
}