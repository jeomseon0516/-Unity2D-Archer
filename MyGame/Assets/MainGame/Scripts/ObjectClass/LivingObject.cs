using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBSERVER;

namespace OBJECT
{
    public abstract partial class LivingObject : ObjectBase, ISubject
    {
        protected GameObject _bullet;
        protected float _beforeLocalY; // 이전 프레임의 로컬 Y 
        protected float _jumpValue;

        private Dictionary<string, IEnumerator> _coroutineList = new Dictionary<string, IEnumerator>();
        protected virtual void CreateBullet() { }
        protected override void Init() { _bullet = ResourcesManager.GetInstance().GetObjectToKey(_id, "Bullet"); }
        protected override void Run() { _animator.SetFloat("Speed", Mathf.Max(Mathf.Abs(_direction.x), Mathf.Abs(_direction.y))); }
        protected IEnumerator Jumping(float jump, float gravity = Constants.GRAVITY)
        {
            _bodyCollider.isTrigger = true;

            while (_body.localPosition.y >= 0)
            {
                yield return YieldCache.WaitForFixedUpdate;
                _body.localPosition += new Vector3(0.0f, jump, 0.0f) * Time.deltaTime;
                jump -= gravity * Time.deltaTime;
            }

            _bodyCollider.isTrigger = false;
            _body.localPosition = new Vector2(0.0f, 0.0f);
        }
        /* 점프를 하지 않는 객체가 있으니 객체별로 따로 호출 해줍니다. */
        protected IEnumerator CheckFallingOrJumping()
        {
            while (true)
            {
                yield return YieldCache.WaitForFixedUpdate;
                _jumpValue = _body.localPosition.y - _beforeLocalY;

                _animator.SetFloat("JumpSpeed", _jumpValue);
                _beforeLocalY = _body.localPosition.y;
            }
        }
        protected void AddAfterResetCoroutine(string key, IEnumerator coroutine)
        {
            if (_coroutineList.ContainsKey(key))
            {
                StopCoroutine(_coroutineList[key]);
                _coroutineList[key] = coroutine;
            }
            else
                _coroutineList.Add(key, coroutine);

            StartCoroutine(_coroutineList[key]);
        }
    }
    public abstract partial class LivingObject : ObjectBase, ISubject
    {
        private List<IObserver> _observers = new List<IObserver>();

        public void RegisterObserver(IObserver observer) { _observers.Add(observer); }
        public void RemoveObserver(IObserver observer) { _observers.Remove(observer); }
        public void NotifyObservers()
        {
            for (int i = 0; i < _observers.Count; i++) { _observers[i].UpdateData(this); }
        }
    }
}