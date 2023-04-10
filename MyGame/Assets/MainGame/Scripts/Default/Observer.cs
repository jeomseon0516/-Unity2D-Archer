using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBJECT;

namespace OBSERVER
{
    public interface ISubject
    {
        public void RegisterObserver(IObserver observer);
        public void RemoveObserver(IObserver observer);
        public void NotifyObservers();
    }
    public interface IObserver
    {
        public void UpdateData(ISubject obj);
    }

}