using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public abstract partial class LivingObject : ObjectBase
    {
        protected GameObject _bullet;

        protected override void Init() 
        { 
            _bullet = ResourcesManager.GetInstance().GetObjectToKey(_id, "Bullet");
        }
        protected float GetFromDirectionToSpeed(Vector2 direction)
        {
            float dirX = direction.x;
            float dirY = direction.y;

            dirX = dirX > 0 ? dirX : -dirX; // Abs
            dirY = dirY > 0 ? dirY : -dirY;

            return dirX >= dirY ? dirX : dirY; // Max
        }
        protected virtual void CreateBullet() {}
        protected override void Run() { _animator.SetFloat("Speed", GetFromDirectionToSpeed(_direction)); }
    }
}