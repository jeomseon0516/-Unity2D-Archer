using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBSERVER;

namespace OBJECT
{
    public abstract partial class LivingObject : ObjectBase, ISubject
    {
        protected GameObject _bullet;
        protected int _maxHp;
        protected virtual void CreateBullet() { }
        protected override void Init() { _bullet = ResourcesManager.GetInstance().GetObjectToKey(_id, "Bullet"); }
        protected override void Run() { _animator.SetFloat("Speed", Mathf.Max(Mathf.Abs(_direction.x), Mathf.Abs(_direction.y))); }
        protected Vector2 RandomMovePosition()
        {
            int xDir = Random.Range(0, 2) == 0 ? -1 : 1;
            int yDir = Random.Range(0, 2) == 0 ? -1 : 1;

            Vector3 offset = new Vector2(Random.Range(0, 5), Random.Range(0.0f, 1.5f));

            return new Vector2(_rigidbody.position.x + offset.x * xDir, 0.0f + offset.y * yDir);
        }
        public int GetMaxHp() { return _maxHp; }

    }

    public abstract partial class LivingObject : ObjectBase, ISubject
    {
        private List<IObserver> _observers = new List<IObserver>();

        public void RegisterObserver(IObserver observer)
        {
            _observers.Add(observer);
        }
        public void RemoveObserver(IObserver observer)
        {
            _observers.Remove(observer);
        }
        public void NotifyObservers()
        {
            for (int i = 0; i < _observers.Count; i++)
            {
                _observers[i].UpdateData(_hp, _maxHp);
            }
        }
        public void UpdateHp(int hp, int maxHp)
        {
            _hp = hp;
            _maxHp = maxHp;
            NotifyObservers();
        }
    }
}
