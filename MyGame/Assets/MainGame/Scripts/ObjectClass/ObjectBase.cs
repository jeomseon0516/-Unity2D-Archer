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
        protected OBJECTID       _id;
        protected Animator       _animator;
        protected SpriteRenderer _sprRen;
        protected SpriteRenderer _shadowSprRen;
        protected Rigidbody2D    _rigidbody;
        protected Transform      _shadow;
        protected Transform      _physics;
        protected Vector2        _direction;
        protected Vector2        _lookAt;
        protected Vector2        _shadowPos;
        protected float          _heightOffset;
        protected float          _offsetY;
        protected float          _speed;
        protected int            _hp;
        protected int            _beforeHp;
        protected int            _atk;
        protected bool           _isDie;

        // default 값 세팅
        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            _sprRen   = GetComponent<SpriteRenderer>();

            _physics = transform.parent;
            _physics.gameObject.AddComponent<ObjectPhysics>();

            _rigidbody = _physics.GetComponent<Rigidbody2D>();

            _shadow       = _physics.Find("Shadow");
            _shadowSprRen = _shadow.GetComponent<SpriteRenderer>();

            _shadowPos = _shadow.localPosition;
            _lookAt    = new Vector2(0.0f, 0.0f);

            _offsetY = transform.position.y - _shadow.transform.position.y;
            _heightOffset = _shadow.gameObject.GetComponent<RectTransform>().rect.height * _shadow.transform.localScale.y;

            _isDie = false;
            _hp    = 10;
            _atk   = 2;
            _speed = 2;

            Init();
        }
        private void OnTriggerEnter2D(Collider2D col) { TriggerAction(col); }
        protected internal virtual void TriggerAction(Collider2D col) {}
        protected internal virtual void CollisionAction(Collision2D obj) {}
        protected virtual void GetDamageAction(int damage) {}
        protected virtual void ObjFixedUpdate() {}
        protected virtual void ObjUpdate() {}
        protected virtual void Init() {}
        protected virtual void Run() {}
        protected virtual void Die() 
        {
            Destroy(_physics.GetComponent<Collider2D>());
            _direction = Vector2.zero;
            if   (_animator == null) { DestroyObj(); }
            else { _animator.SetTrigger("Die"); }
        }
        private void FixedUpdate()
        {
            if (_hp < _beforeHp)
                GetDamageAction(_beforeHp - _hp);
                _beforeHp = _hp;

            if (CheckDeadToHp())
                return;

            _lookAt = _direction;
            Run();
            ObjFixedUpdate();
            Move(_direction.x, _direction.y);
            CheckHeight();
            ChangeFlipXToHor(_lookAt.x);
            SettingZNode();
        }
        protected bool TriggerCollision(Transform targetPhysics, ObjectBase obj)
        {
            float targetPosY = targetPhysics.position.y - obj.GetOffSetY();
            float myPosY     = _rigidbody.position.y - _offsetY;

            float targetOffsetY = obj.GetHeightOffSet();

            if (myPosY + _heightOffset > targetPosY - targetOffsetY && myPosY + _heightOffset < targetPosY + targetOffsetY ||
                myPosY - _heightOffset < targetPosY + targetOffsetY && myPosY - _heightOffset > targetPosY - targetOffsetY)
            {
                obj.TakeDamage(_atk);
                return true;
            }
            return false;
        }
        private bool CheckDeadToHp()
        {
            if (_isDie)  return true;
            if (_hp > 0) return false;
            _isDie = true;
            Die();
            return true;
        }
        private void Move(float moveX, float moveY)
        {
            if (moveX == 0.0f && moveY == 0.0f) return;
            _rigidbody.MovePosition(_rigidbody.position + new Vector2(moveX * _speed, moveY * (_speed * 0.5f)) * Time.deltaTime);
        }
        private void ChangeFlipXToHor(float hor)
        {
            Quaternion rotation = _physics.rotation;

            if      (hor < 0.0f) rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, 180.0f, rotation.eulerAngles.z);
            else if (hor > 0.0f) rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, 0.0f,   rotation.eulerAngles.z);
                                                                                                     
            _physics.rotation = rotation;
        }
        private void SettingZNode() 
        { 
            _sprRen.sortingOrder = (int)((_shadow.position.y) * 10) * -1;
            _shadowSprRen.sortingOrder = _sprRen.sortingOrder - 1;
        }
        private void Update() { ObjUpdate(); }
        protected void DestroyObj() { Destroy(_physics.gameObject); }
        public void TakeDamage(int damage) { _hp -= damage; }
        private void CheckHeight() { _shadow.localPosition = new Vector2(_shadowPos.x, _shadowPos.y - transform.localPosition.y * 0.5f); }
        public float GetHeightOffSet() { return _heightOffset; }
        public float GetOffSetY() { return _offsetY; }
        public int GetHp() { return _hp; }
    }
}