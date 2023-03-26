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
            int index = name.IndexOf("(Clone)");

            if (index > 0)
                name = name.Substring(0, index);

            _child = transform.Find(name).GetComponent<ObjectBase>();
        }
        private void OnCollisionEnter2D(Collision2D collision) { _child.CollisionAction(collision); }
    }
}

