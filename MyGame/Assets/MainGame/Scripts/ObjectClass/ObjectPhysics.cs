using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBJECT;

namespace OBJECT
{
    public sealed class ObjectPhysics : MonoBehaviour
    {
        private ObjectBase _child;
        private void Awake()
        {
            name = Default.GetRemoveSelectString(name, "(Clone)");
            _child = transform.parent.Find("Image").GetComponent<ObjectBase>();
        }
        private void OnCollisionEnter2D(Collision2D collision) { _child.CollisionAction(collision); }
        private void OnTriggerEnter2D(Collider2D collision) { _child.TriggerAction(collision); }
    }
}

