using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OBJECTID
{
    PLAYER,
    ENEMY,
    BACKGROUND,
    FX
}
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
        _animator = GetComponent<Animator>();
        _sprRen   = GetComponent<SpriteRenderer>();
        _shadow   = transform.Find("Shadow");
        _hp = 10;
        _speed = 2;
        _direction = new Vector3(1.0f, 0.0f, 0.0f);

        Init();
    }
    protected virtual void Init() {}
    protected virtual void Run() {}
    protected virtual void CollisionAction(Collision2D obj) {}
    protected virtual void ObjUpdate() {}
    protected virtual void Die() { Destroy(gameObject); }
    private bool CheckDeadToHp() 
    { 
        if (_hp > 0) return false;
        Die();
        return true;
    }
    private void Update()
    {
        if (CheckDeadToHp()) return;

        Run();
        ObjUpdate();
        Move(_direction.x, _direction.y);
        SettingZNode();
        ChangeFlipXToHor(_direction.x);
    }
    private void Move(float moveX, float moveY)
    {
        if (moveX == 0.0f && moveY == 0.0f) return;
        transform.position += new Vector3(moveX * _speed, moveY * (_speed * 0.5f), 0.0f) * Time.deltaTime;
    }
    private void ChangeFlipXToHor(float hor)
    {
        Quaternion rotation = transform.rotation;

        if      (hor < 0) rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, 180.0f, rotation.eulerAngles.z);
        else if (hor > 0) rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, 0.0f,   rotation.eulerAngles.z);

        transform.rotation = rotation;
    }
    private void OnCollisionEnter2D(Collision2D collision) { CollisionAction(collision); }
    private void SettingZNode() { _sprRen.sortingOrder = (int)((_shadow.position.y) * 10) * -1; }
}