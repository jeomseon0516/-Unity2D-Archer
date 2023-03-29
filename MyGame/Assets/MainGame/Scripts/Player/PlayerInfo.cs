using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBSERVER
{
    public class PlayerInfo : ISubject
    {
        private List<IObserver> _observers = new List<IObserver>();

        private int _hp;

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
                _observers[i].UpdateData(_hp);
            }
        }
        public void Update(int hp)
        {
            _hp = hp;
            NotifyObservers();
        }
        public int GetHp() { return _hp; }
        public void SetHp(int hp) { _hp = hp; }

        public PlayerInfo() {}
    }
    public class HpObserver : IObserver
    {
        private PlayerInfo _plrInfo = null;

        public void Init(PlayerInfo plrInfo) { _plrInfo = plrInfo; }

        public void UpdateData(int hp)
        {
            
        }

        public HpObserver() { }
    }
}
