using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectBase : MonoBehaviour
{
    protected Animator       _animator;
    protected SpriteRenderer _sprRen;
    protected OBJECTID       _id;
    protected Vector3        _direction;
    protected float          _speed;
    protected int            _hp;
    protected Transform      _shadow;

    protected virtual void Awake()
    {
        Init();
        _animator = GetComponent<Animator>();
        _sprRen   = GetComponent<SpriteRenderer>();
        _shadow   = transform.Find("Shadow");
    }
    protected virtual void Init() {}
    protected virtual void Run() {}
    protected virtual void ObjUpdate() { SettingZNode(); }
    private void Update()
    {
        Run();
        ObjUpdate();
    }
    private void SettingZNode() { _sprRen.sortingOrder = (int)((_shadow.position.y) * 10) * -1; }
}
