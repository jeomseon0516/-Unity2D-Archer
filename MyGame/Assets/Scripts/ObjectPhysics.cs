using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBJECT;

public sealed class ObjectPhysics : MonoBehaviour
{
    private ObjectBase _child;
    private void Awake() { _child = transform.Find(name).GetComponent<ObjectBase>(); }
    private void OnCollisionEnter2D(Collision2D collision) { _child.CollisionAction(collision); }
}

