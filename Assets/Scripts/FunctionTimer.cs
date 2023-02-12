using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utitlity
{
    public class FunctionTimerDecreased
    {
        public static FunctionTimerDecreased Create(long time, Action onUpdate, Action<int> onSecond, Action onComplete, bool useUnscaledTime = false)
        {
            Time.timeScale = 1.0f;
            var t = GetFunctionTimer();

            if (t.mono != null)
            {
                t.Reset();
                t.Setup(time, onUpdate, onSecond, onComplete, false);
            }
            else
            {
                t.Reset();
                var timer = new GameObject("Timer");
                var comp = timer.AddComponent<MonoBehaviourHook>();
                t.Setup(time, comp, onUpdate, onSecond, onComplete, true);
            }

            return t;
        }

        public static void Stop(FunctionTimerDecreased timer)
        {
            if (timer == null) return;
            if (timer.mono != null)
            {
                timer.mono.gameObject.SetActive(false);
                timer.mono.OnUpdate = null;
            }
            timer.Reset();
        }

        public static void StopAll()
        {
            foreach (var t in pool)
                t.Reset();
        }
        public static void Destroy(FunctionTimerDecreased timer)
        {
            if (timer == null) return;


            pool.Remove(timer);
            UnityEngine.Object.Destroy(timer.parent.gameObject);
        }

        public static void DestroyAll()
        {
            foreach (var item in pool)
                UnityEngine.Object.Destroy(item.mono);
            pool.Clear();
        }

        private static FunctionTimerDecreased GetFunctionTimer()
        {
            foreach(var timer in pool)
            {
                if (timer.isFree)
                    return timer;
            }
            var t = new FunctionTimerDecreased();
            pool.Add(t);
            return t;
        }

        private static List<FunctionTimerDecreased> pool = new List<FunctionTimerDecreased>();

        private long time = 0;
        private float currentTime = 0f;
        private float currentSec = 1f;
        private bool useUnscaledTime;
        public bool isFree = true;
        private GameObject parent;
        private MonoBehaviourHook mono;
        private Action onUpdate, onComplete;
        private Action<int> onSecond;

        private void Setup(long time, MonoBehaviourHook mono, Action onUpdate, Action<int> onSecond, Action onComplete, bool useUnscaledTime = false)
        {
            if (parent == null)
                parent = new GameObject("Timers_Pool");
            mono.transform.SetParent(parent.transform, false);
            this.onUpdate = onUpdate;
            this.onComplete = onComplete;
            this.onSecond = onSecond;
            this.useUnscaledTime = useUnscaledTime;
            this.time = time;
            this.currentTime = time;
            this.currentSec = time;
            this.isFree = false;
            this.mono = mono;
            this.mono.OnUpdate = OnUpdate;
            mono.gameObject.SetActive(true);
        }

        private void Setup(long time, Action onUpdate, Action<int> onSecond, Action onComplete, bool useUnscaledTime)
        {
            this.onUpdate = onUpdate;
            this.onComplete = onComplete;
            this.onSecond = onSecond;
            this.useUnscaledTime = useUnscaledTime;
            this.time = time;
            this.currentTime = time;
            this.currentSec = time;
            this.isFree = false;
            this.mono.OnUpdate = OnUpdate;
            mono.gameObject.SetActive(true);
        }

        public void Reset()
        {
            time = 0;
            currentTime = 0f;
            useUnscaledTime = false;
            isFree = true;
            if (mono == null) return;
            mono.OnUpdate = null;
            mono.gameObject.SetActive(false);
        }

        private void OnUpdate()
        {
            currentTime -= Time.deltaTime;
            if (onUpdate != null) onUpdate();
            if(currentSec - 1f > currentTime)
            {
                currentSec -= 1f;
                if (onSecond != null) onSecond(Mathf.CeilToInt(currentTime));//sends current second
            }
            if(currentTime <= 0f)
            {
                Stop(this);
                if (onComplete != null) onComplete();
            }
        }
    }
    

    public class FunctionTimerIncreased
    {
        public static FunctionTimerIncreased Create(float time,float maxTime, Action onUpdate, Action<int> onSecond, Action onComplete, bool useUnscaledTime = false)
        {
            Time.timeScale = 1.0f;
            var t = GetFunctionTimer();

            if (t.mono != null)
            {
                t.Reset();
                t.Setup(time, maxTime, onUpdate, onSecond, onComplete, false);
            }
            else
            {
                t.Reset();
                var timer = new GameObject("Timer");
                var comp = timer.AddComponent<MonoBehaviourHook>();
                t.Setup(time, maxTime, comp, onUpdate, onSecond, onComplete, true);
            }

            return t;
        }

        public static void Stop(FunctionTimerIncreased timer)
        {
            if (timer == null) return;
            if (timer.mono != null)
            {
                timer.mono.gameObject.SetActive(false);
                timer.mono.OnUpdate = null;
            }
            timer.Reset();
        }

        public static void StopAll()
        {
            foreach (var t in pool)
                t.Reset();
        }

        //public static void DestroyAll()
        //{
        //    foreach (var item in pool)
        //    {
        //        if(item.mono.gameObject != null)
        //        {
        //            pool.Remove(item);
        //            UnityEngine.Object.Destroy(item.mono.gameObject.transform.parent.gameObject);
        //        }
        //    }
           
        //}

        public static void DestroyAll()
        {
            foreach (var items in pool)
            {
                UnityEngine.Object.Destroy(items.parent.gameObject);
            }

            pool.Clear();
        }

        public static void Destroy(FunctionTimerIncreased timers)
        {
            if (timers == null) return;

            pool.Remove(timers);

            UnityEngine.Object.Destroy(timers.parent.gameObject);
        }

        private static FunctionTimerIncreased GetFunctionTimer()
        {
            foreach(var timer in pool)
            {
                if (timer.isFree)
                    return timer;
            }
            var t = new FunctionTimerIncreased();
            pool.Add(t);
            return t;
        }

        private static List<FunctionTimerIncreased> pool = new List<FunctionTimerIncreased>();

        private float time = 0f;
        private float maxTime = 0f;
        private float currentTime = 0f;
        private float currentSec = 1f;
        private bool useUnscaledTime;
        public bool isFree = true;
        private GameObject parent;
        private MonoBehaviourHook mono;
        private Action onUpdate, onComplete;
        private Action<int> onSecond;

        private void Setup(float time, float maxTime, MonoBehaviourHook mono, Action onUpdate, Action<int> onSecond, Action onComplete, bool useUnscaledTime = false)
        {
            if (parent == null)
                parent = new GameObject("Timers_Pool");
            mono.transform.SetParent(parent.transform, false);
            this.onUpdate = onUpdate;
            this.onComplete = onComplete;
            this.onSecond = onSecond;
            this.useUnscaledTime = useUnscaledTime;
            this.time = time;
            this.maxTime = maxTime;
            this.currentTime = time;
            this.currentSec = time;
            this.isFree = false;
            this.mono = mono;
            this.mono.OnUpdate = OnUpdate;
            mono.gameObject.SetActive(true);
        }

        private void Setup(float time, float maxTime, Action onUpdate, Action<int> onSecond, Action onComplete, bool useUnscaledTime)
        {
            this.onUpdate = onUpdate;
            this.onComplete = onComplete;
            this.onSecond = onSecond;
            this.useUnscaledTime = useUnscaledTime;
            this.time = time;
            this.maxTime = maxTime;
            this.currentTime = time;
            this.currentSec = time;
            this.isFree = false;
            this.mono.OnUpdate = OnUpdate;
            mono.gameObject.SetActive(true);
        }

        public void Reset()
        {
            time = 0f;
            maxTime = 0f;
            currentTime = 0f;
            useUnscaledTime = false;
            isFree = true;
            if (mono == null) return;
            mono.OnUpdate = null;
            mono.gameObject.SetActive(false);
            
        }

        private void OnUpdate()
        {
            currentTime += Time.deltaTime;
            if (onUpdate != null) onUpdate();
            if(currentSec + 1f > currentTime)
            {
                currentSec += 1f;
                if (onSecond != null) onSecond(Mathf.FloorToInt(currentTime)); //sends current second
            }
            if(currentTime >= maxTime)
            {
                Stop(this);
                if (onComplete != null) onComplete();
            }
        }
    }  
    public class FunctionTimerDeciSecond
    {
        public static FunctionTimerDeciSecond Create(float time,float maxTime, Action onUpdate, Action<int> onSecond, Action onComplete, bool useUnscaledTime = false)
        {
            Time.timeScale = 1.0f;
            var t = GetFunctionTimer();

            if (t.mono != null)
            {
                t.Reset();
                t.Setup(time, maxTime, onUpdate, onSecond, onComplete, false);
            }
            else
            {
                t.Reset();
                var timer = new GameObject("Timer");
                var comp = timer.AddComponent<MonoBehaviourHook>();
                t.Setup(time, maxTime, comp, onUpdate, onSecond, onComplete, true);
            }

            return t;
        }

        public static void Stop(FunctionTimerDeciSecond timer)
        {
            if (timer == null) return;
            if (timer.mono != null)
            {
                timer.mono.gameObject.SetActive(false);
                timer.mono.OnUpdate = null;
            }
            timer.Reset();
        }

        public static void StopAll()
        {
            foreach (var t in pool)
                t.Reset();
        }

        //public static void DestroyAll()
        //{
        //    foreach (var item in pool)
        //    {
        //        if(item.mono.gameObject != null)
        //        {
        //            pool.Remove(item);
        //            UnityEngine.Object.Destroy(item.mono.gameObject.transform.parent.gameObject);
        //        }
        //    }
           
        //}

        public static void DestroyAll()
        {
            foreach (var items in pool)
            {
                UnityEngine.Object.Destroy(items.parent.gameObject);
            }

            pool.Clear();
        }

        public static void Destroy(FunctionTimerDeciSecond timer)
        {
            if (timer == null) return;

            
            pool.Remove(timer);
            UnityEngine.Object.Destroy(timer.parent.gameObject);
        }

        private static FunctionTimerDeciSecond GetFunctionTimer()
        {
            //foreach(var timer in pool)
            //{
            //    if (timer.isFree)
            //        return timer;
            //}
            var t = new FunctionTimerDeciSecond();
            pool.Add(t);
            return t;
        }

        private static List<FunctionTimerDeciSecond> pool = new List<FunctionTimerDeciSecond>();

        private float time = 0f;
        private float maxTime = 0f;
        private float currentTime = 0f;
        private float currentSec = 1f;
        private bool useUnscaledTime;
        public bool isFree = true;
        private GameObject parent;
        private MonoBehaviourHook mono;
        private Action onUpdate, onComplete;
        private Action<int> onSecond;

        private void Setup(float time, float maxTime, MonoBehaviourHook mono, Action onUpdate, Action<int> onSecond, Action onComplete, bool useUnscaledTime = false)
        {
            if (parent == null)
                parent = new GameObject("Timers_Pool");
            mono.transform.SetParent(parent.transform, false);
            this.onUpdate = onUpdate;
            this.onComplete = onComplete;
            this.onSecond = onSecond;
            this.useUnscaledTime = useUnscaledTime;
            this.time = time;
            this.maxTime = maxTime;
            this.currentTime = time;
            this.currentSec = time;
            this.isFree = false;
            this.mono = mono;
            this.mono.OnUpdate = OnUpdate;
            mono.gameObject.SetActive(true);
        }

        private void Setup(float time, float maxTime, Action onUpdate, Action<int> onSecond, Action onComplete, bool useUnscaledTime)
        {
            this.onUpdate = onUpdate;
            this.onComplete = onComplete;
            this.onSecond = onSecond;
            this.useUnscaledTime = useUnscaledTime;
            this.time = time;
            this.maxTime = maxTime;
            this.currentTime = time;
            this.currentSec = time;
            this.isFree = false;
            this.mono.OnUpdate = OnUpdate;
            mono.gameObject.SetActive(true);
        }

        public void Reset()
        {
            time = 0f;
            maxTime = 0f;
            currentTime = 0f;
            useUnscaledTime = false;
            isFree = true;
            if (mono == null) return;
            mono.OnUpdate = null;
            mono.gameObject.SetActive(false);
            
        }

        private void OnUpdate()
        {
            currentTime += Time.deltaTime * 20f;
            if (onUpdate != null) onUpdate();
            if(currentSec + 1f > currentTime)
            {
                currentSec += 1f;
                if (onSecond != null) onSecond(Mathf.FloorToInt(currentTime)); //sends current second
            }
            if(currentTime >= maxTime)
            {
                Stop(this);
                if (onComplete != null) onComplete();
            }
        }
    }

    public class FunctionTimerUnlimited
    {
        public static FunctionTimerUnlimited Create(float time, Action onUpdate, Action<int> onSecond, bool useUnscaledTime = false)
        {
            Time.timeScale = 1.0f;
            var t = GetFunctionTimer();

            if (t.mono != null)
            {
                t.Reset();
                t.Setup(time, onUpdate, onSecond, false);
            }
            else
            {
                t.Reset();
                var timer = new GameObject("Timer");
                var comp = timer.AddComponent<MonoBehaviourHook>();
                t.Setup(time,comp, onUpdate, onSecond, true);
            }

            return t;
        }

        public static void Stop(FunctionTimerUnlimited timer)
        {
            if (timer == null) return;
            if (timer.mono != null)
            {
                timer.mono.gameObject.SetActive(false);
                timer.mono.OnUpdate = null;
            }
            timer.Reset();
        }

        public static void StopAll()
        {
            foreach (var t in pool)
                t.Reset();
        }


        public static void DestroyAll()
        {
            foreach (var items in pool)
            {
                UnityEngine.Object.Destroy(items.parent.gameObject);
            }

            pool.Clear();
        }

        public static void Destroy(FunctionTimerUnlimited timer)
        {
            if (timer == null) return;


            pool.Remove(timer);
            UnityEngine.Object.Destroy(timer.parent.gameObject);
        }

        private static FunctionTimerUnlimited GetFunctionTimer()
        {
            //foreach(var timer in pool)
            //{
            //    if (timer.isFree)
            //        return timer;
            //}
            var t = new FunctionTimerUnlimited();
            pool.Add(t);
            return t;
        }

        private static List<FunctionTimerUnlimited> pool = new List<FunctionTimerUnlimited>();

        private float time = 0f;
        private float maxTime = 0f;
        private float currentTime = 0f;
        private float currentSec = 1f;
        private bool useUnscaledTime;
        public bool isFree = true;
        private GameObject parent;
        private MonoBehaviourHook mono;
        private Action onUpdate, onComplete;
        private Action<int> onSecond;

        private void Setup(float time, MonoBehaviourHook mono, Action onUpdate, Action<int> onSecond, bool useUnscaledTime = false)
        {
            if (parent == null)
                parent = new GameObject("Timers_Pool");
            mono.transform.SetParent(parent.transform, false);
            this.onUpdate = onUpdate;
            this.onSecond = onSecond;
            this.useUnscaledTime = useUnscaledTime;
            this.time = time;
            this.currentTime = time;
            this.currentSec = time;
            this.isFree = false;
            this.mono = mono;
            this.mono.OnUpdate = OnUpdate;
            mono.gameObject.SetActive(true);
        }

        private void Setup(float time, Action onUpdate, Action<int> onSecond, bool useUnscaledTime)
        {
            this.onUpdate = onUpdate;
            this.onSecond = onSecond;
            this.useUnscaledTime = useUnscaledTime;
            this.time = time;
            this.currentTime = time;
            this.currentSec = time;
            this.isFree = false;
            this.mono.OnUpdate = OnUpdate;
            mono.gameObject.SetActive(true);
        }

        public void Reset()
        {
            time = 0f;
            maxTime = 0f;
            currentTime = 0f;
            useUnscaledTime = false;
            isFree = true;
            if (mono == null) return;
            mono.OnUpdate = null;
            mono.gameObject.SetActive(false);

        }

        private void OnUpdate()
        {
            currentTime += Time.deltaTime;
            if (onUpdate != null) onUpdate();
            if (currentSec + 1f > currentTime)
            {
                currentSec += 1f;
              //  if (onSecond != null) onSecond(Mathf.FloorToInt(currentTime)); //sends current second
                if (onSecond != null) onSecond(Mathf.FloorToInt(currentTime)); //sends current second
            }
            if (currentTime >= maxTime)
            {
               // Stop(this);
               // if (onComplete != null) onComplete();
            }
        }
    }
    public class ToolTipTimer
    {
        public static ToolTipTimer Create(float time, Action onUpdate, Action<int> onSecond, bool useUnscaledTime = false)
        {
            Time.timeScale = 1.0f;
            var t = GetFunctionTimer();

            if (t.mono != null)
            {
                t.Reset();
                t.Setup(time, onUpdate, onSecond, false);
            }
            else
            {
                t.Reset();
                var timer = new GameObject("Timer");
                var comp = timer.AddComponent<MonoBehaviourHook>();
                t.Setup(time,comp, onUpdate, onSecond, true);
            }

            return t;
        }

        public static void Stop(ToolTipTimer timer)
        {
            if (timer == null) return;
            if (timer.mono != null)
            {
                timer.mono.gameObject.SetActive(false);
                timer.mono.OnUpdate = null;
            }
            timer.Reset();
        }

        public static void StopAll()
        {
            foreach (var t in pool)
                t.Reset();
        }


        public static void DestroyAll()
        {
            foreach (var items in pool)
            {
                UnityEngine.Object.Destroy(items.parent.gameObject);
            }

            pool.Clear();
        }

        public static void Destroy(ToolTipTimer timer)
        {
            if (timer == null) return;


            pool.Remove(timer);
            UnityEngine.Object.Destroy(timer.parent.gameObject);
        }

        private static ToolTipTimer GetFunctionTimer()
        {
            foreach (var timer in pool)
            {
                if (timer.isFree)
                    return timer;
            }
            var t = new ToolTipTimer();
            pool.Add(t);
            return t;
        }

        private static List<ToolTipTimer> pool = new List<ToolTipTimer>();

        private float time = 0f;
        private float maxTime = 0f;
        private float currentTime = 0f;
        private float currentSec = 1f;
        private bool useUnscaledTime;
        public bool isFree = true;
        private GameObject parent;
        private MonoBehaviourHook mono;
        private Action onUpdate, onComplete;
        private Action<int> onSecond;

        private void Setup(float time, MonoBehaviourHook mono, Action onUpdate, Action<int> onSecond, bool useUnscaledTime = false)
        {
            if (parent == null)
                parent = new GameObject("Timers_Pool");
            mono.transform.SetParent(parent.transform, false);
            this.onUpdate = onUpdate;
            this.onSecond = onSecond;
            this.useUnscaledTime = useUnscaledTime;
            this.time = time;
            this.currentTime = time;
            this.currentSec = time;
            this.isFree = false;
            this.mono = mono;
            this.mono.OnUpdate = OnUpdate;
            mono.gameObject.SetActive(true);
        }

        private void Setup(float time, Action onUpdate, Action<int> onSecond, bool useUnscaledTime)
        {
            this.onUpdate = onUpdate;
            this.onSecond = onSecond;
            this.useUnscaledTime = useUnscaledTime;
            this.time = time;
            this.currentTime = time;
            this.currentSec = time;
            this.isFree = false;
            this.mono.OnUpdate = OnUpdate;
            mono.gameObject.SetActive(true);
        }

        public void Reset()
        {
            time = 0f;
            maxTime = 0f;
            currentTime = 0f;
            useUnscaledTime = false;
            isFree = true;
            if (mono == null) return;
            mono.OnUpdate = null;
            mono.gameObject.SetActive(false);

        }

        private void OnUpdate()
        {
            currentTime += Time.deltaTime;
            if (onUpdate != null) onUpdate();
            if (currentSec + 1f > currentTime)
            {
                currentSec += 1f;
              //  if (onSecond != null) onSecond(Mathf.FloorToInt(currentTime)); //sends current second
                if (onSecond != null) onSecond(Mathf.FloorToInt(currentTime)); //sends current second
            }
            if (currentTime >= maxTime)
            {
               // Stop(this);
               // if (onComplete != null) onComplete();
            }
        }
    }


}
