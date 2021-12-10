using System;
using System.Collections.Generic;
using UnityEngine;

namespace DemoApp.Utils
{
    public class AsyncManager : MonoBehaviour
    {
        private static AsyncManager _instance;

        private readonly Queue<Action> _execOnMainThreadQueue = new Queue<Action>();

        public static void ExecuteOnMainThread(Action action) =>
            _instance._execOnMainThreadQueue.Enqueue(action);
        
        private void Start()
        {
            _instance = this;
        }

        private void Update()
        {
            while (_execOnMainThreadQueue.Count > 0)
            {
                var nextAction = _execOnMainThreadQueue.Dequeue();
                nextAction?.Invoke();
            }
        }
    }
}