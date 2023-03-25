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

namespace OBJECT
{
    public abstract class ObjectBase : MonoBehaviour
    {
        protected Animator _animator;
        protected SpriteRenderer _sprRen;
        protected SpriteRenderer _shadowSprRen;
        protected OBJECTID _id;
        protected Vector3 _direction;
        protected Vector3 _lookAt;
        protected float _speed;
        protected int _hp;
        protected Transform _shadow;
        protected Vector3 _shadowPos;
        protected Transform _physics;
        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            _sprRen   = GetComponent<SpriteRenderer>();
            _physics = transform.parent;
            _shadow = _physics.Find("Shadow");
            _shadowSprRen = _shadow.GetComponent<SpriteRenderer>();
            _shadowPos = _shadow.localPosition;
            _hp = 10;
            _speed = 2;
            _direction = new Vector3(1.0f, 0.0f, 0.0f);
            transform.parent.gameObject.AddComponent<ObjectPhysics>();

            Init();
        }
        protected virtual void Init() {}
        protected virtual void Run() {}
        protected internal virtual void CollisionAction(Collision2D obj) {}
        protected virtual void ObjUpdate() {}
        protected virtual void Die() 
        {
            Destroy(_physics.GetComponent<Collider2D>());
            if (_animator == null) { DestroyObj(); }
            else { _animator.SetTrigger("Die"); }
        }
        protected void DestroyObj() { Destroy(_physics.gameObject); }
        private bool CheckDeadToHp()
        {
            if (_hp > 0) return false;
            Die();
            return true;
        }
        private void Update()
        {
            if (CheckDeadToHp()) return;

            _lookAt = _direction;
            Run();
            ObjUpdate();
            Move(_direction.x, _direction.y);
            CheckHeight();
            ChangeFlipXToHor(_lookAt.x);
            SettingZNode();
        }
        private void Move(float moveX, float moveY)
        {
            if (moveX == 0.0f && moveY == 0.0f) return;
            _physics.position += new Vector3(moveX * _speed, moveY * (_speed * 0.5f), 0.0f) * Time.deltaTime;
        }
        private void ChangeFlipXToHor(float hor)
        {
            Quaternion rotation = _physics.rotation;

            if      (hor < 0) rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, 180.0f, rotation.eulerAngles.z);
            else if (hor > 0) rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, 0.0f,   rotation.eulerAngles.z);

            _physics.rotation = rotation;
        }
        private void CheckHeight() { _shadow.localPosition = new Vector3(_shadowPos.x, _shadowPos.y - transform.localPosition.y * 0.5f, 0.0f); }
        private void SettingZNode() 
        { 
            _sprRen.sortingOrder = (int)((_shadow.position.y) * 10) * -1;
            _shadowSprRen.sortingOrder = _sprRen.sortingOrder - 1;
        }
    }
}