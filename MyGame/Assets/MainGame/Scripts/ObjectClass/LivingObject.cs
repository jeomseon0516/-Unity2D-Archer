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
        protected override void Run() { _animator.SetFloat("Speed", GetFromDirectionToSpeed(_direction)); }
        protected float GetFromDirectionToSpeed(Vector2 direction)
        {
            float dirX = direction.x;
            float dirY = direction.y;

            dirX = dirX > 0 ? dirX : -dirX; // Abs
            dirY = dirY > 0 ? dirY : -dirY;

            return dirX >= dirY ? dirX : dirY; // Max
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