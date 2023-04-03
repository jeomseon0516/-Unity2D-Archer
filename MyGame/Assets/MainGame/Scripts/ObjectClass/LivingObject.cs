using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBSERVER;

namespace OBJECT
{
    public abstract partial class LivingObject : ObjectBase, ISubject
    {
        protected GameObject _bullet;

        protected virtual void CreateBullet() {}
        protected override void Init() { _bullet = ResourcesManager.GetInstance().GetObjectToKey(_id, "Bullet"); }
        protected override void Run() 
        {
            float dirX = _direction.x;
            float dirY = _direction.y;

            dirX = dirX > 0 ? dirX : -dirX; // Abs
            dirY = dirY > 0 ? dirY : -dirY;

            float speed = dirX >= dirY ? dirX : dirY; // Max

            _animator.SetFloat("Speed", speed);
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