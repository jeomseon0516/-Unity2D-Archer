using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBJECT;

namespace PUB_SUB
{
    public interface IHpSubscriber
    {
        void OnUpdateHp(int hp);
        void OnUpdateMaxHp(int maxHp);
    }
}