using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBJECT;

namespace OBJECT
{
    public class AttackBox : MonoBehaviour
    {
        ObjectBase _objBase;
        private void Awake() { _objBase = transform.parent.GetComponent<ObjectBase>(); }
        private void OnTriggerEnter2D(Collider2D col) { _objBase.TriggerAction(col); }
    }
}