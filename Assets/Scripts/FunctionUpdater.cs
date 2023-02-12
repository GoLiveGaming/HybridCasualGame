using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utitlity
{
    public class FunctionUpdater
    {
        /*
         * Class to hook Actions into MonoBehaviour
         * */
        public class MonoBehaviourHook : MonoBehaviour
        {

            public Action OnUpdate;

            private void Update()
            {
                if (OnUpdate != null) OnUpdate();
            }
        }

        public static FunctionUpdater Create(Action onUpdate)
        {
            var t = GetFunctionTimer();
            t.Reset();
            if (t.mono != null)
            {
                t.Setup(onUpdate);
            }
            else
            {
                var timer = new GameObject("Updater", typeof(MonoBehaviourHook));
                var comp = timer.AddComponent<MonoBehaviourHook>();
                t.Setup(comp, onUpdate);
            }

            return t;
        }

        public static void Stop(FunctionUpdater updater)
        {
            updater.Reset();
        }

        private static FunctionUpdater GetFunctionTimer()
        {
            foreach(var timer in pool)
            {
                if (timer.isFree)
                    return timer;
            }
            var t = new FunctionUpdater();
            pool.Add(t);
            return t;
        }

        private static List<FunctionUpdater> pool = new List<FunctionUpdater>();

        public bool isFree = true;
        private GameObject parent;
        private MonoBehaviourHook mono;

        private void Setup(MonoBehaviourHook mono, Action onUpdate)
        {
            if (parent == null)
                parent = new GameObject("Updaters_Pool");
            mono.transform.SetParent(parent.transform, false);
            this.mono = mono;
            this.mono.OnUpdate = onUpdate;
            mono.gameObject.SetActive(true);
        }

        private void Setup(Action onUpdate)
        {
            mono.OnUpdate = onUpdate;
            mono.gameObject.SetActive(true);
        }

        public void Reset()
        {
            isFree = true;
            if (mono == null) return;
            mono.OnUpdate = null;
            mono.gameObject.SetActive(false);
        }
    }
}
