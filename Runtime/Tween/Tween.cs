using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Euphrates
{
    public static class Tween
    {
        static List<TweenData> _tweens = new List<TweenData>();
        static bool _working;

        #region Extensions
        public static void DoPosition(this Transform transform, Vector3 position, float duration, Ease easeFunction = Ease.Lerp, Action<Vector3> onStep = null, Action onFinish = null)
        {
            Vector3 start = transform.position;
            Vector3 end = position;

            void Step(Vector3 val)
            {
                transform.position = val;
                onStep?.Invoke(val);
            }

            DoTween(start, end, duration, easeFunction, Step, onFinish);
        }

        public static void DoMove(this Transform transform, Vector3 move, float duration, Ease easeFunction = Ease.Lerp) => DoPosition(transform, transform.position + move, duration, easeFunction);
        #endregion

        #region Do
        public static int DoTween(float var, float endVal, float duration, Ease easeFunction = Ease.Lerp, Action<float> onStep = null, Action onFinish = null)
        {
            Func<float, float> ease = TweenOps.GetEaseFunction(easeFunction);
            float Operation(TweenData<float> data, float t) => TweenOps.FloatOperation(data, t, ease);

            TweenData<float> data = new TweenData<float>(
                UnityEngine.Random.Range(100000, 999999),
                var,
                endVal,
                Time.realtimeSinceStartup,
                duration,
                onStep,
                onFinish,
                Operation);

            AddTween(data);
            return data.ID;
        }

        public static int DoTween(Color var, Color endVal, float duration, Action<Color> onStep = null, Action onFinish = null)
        {
            TweenData<Color> data = new TweenData<Color>(
                UnityEngine.Random.Range(100000, 999999),
                var,
                endVal,
                Time.realtimeSinceStartup,
                duration,
                onStep,
                onFinish,
                TweenOps.ColorOperation);

            AddTween(data);
            return data.ID;
        }

        public static int DoTween(Vector3 var, Vector3 endVal, float duration, Ease easeFunction = Ease.Lerp, Action<Vector3> onStep = null, Action onFinish = null)
        {
            Func<float, float> ease = TweenOps.GetEaseFunction(easeFunction);
            Vector3 Operation(TweenData<Vector3> data, float t) => TweenOps.Vector3Operation(data, t, ease);

            TweenData<Vector3> data = new TweenData<Vector3>(
                UnityEngine.Random.Range(100000, 999999),
                var,
                endVal,
                Time.realtimeSinceStartup,
                duration,
                onStep,
                onFinish,
                Operation);

            AddTween(data);
            return data.ID;
        }

        public static int DoTween(Vector2 var, Vector2 endVal, float duration, Ease easeFunction = Ease.Lerp, Action<Vector2> onStep = null, Action onFinish = null)
        {
            Func<float, float> ease = TweenOps.GetEaseFunction(easeFunction);
            Vector2 Operation(TweenData<Vector2> data, float t) => TweenOps.Vector2Operation(data, t, ease);

            TweenData<Vector2> data = new TweenData<Vector2>(
                UnityEngine.Random.Range(100000, 999999),
                var,
                endVal,
                Time.realtimeSinceStartup,
                duration,
                onStep,
                onFinish,
                Operation);

            AddTween(data);
            return data.ID;
        }
        #endregion

        public static bool StopTween(int ID)
        {
            foreach (var tw in _tweens)
            {
                if (tw.ID != ID)
                    continue;

                _tweens.Remove(tw);
                return true;
            }

            return false;
        }

        static void AddTween(TweenData data)
        {
            _tweens.Add(data);

            if (!_working)
                WorkTweens();
        }

        async static void WorkTweens()
        {
            if (_working)
                return;

            _working = true;

            while (_tweens.Count > 0)
            {
                for (int i = _tweens.Count - 1; i >= 0; i--)
                {
                    var tw = _tweens[i];

                    Type tVal = tw.GetType().GetProperty("From").PropertyType;
                    Type tBase = typeof(TweenData<>);
                    Type tSpesific = tBase.MakeGenericType(tVal);

                    float start = (float)tSpesific.GetProperty("Start").GetValue(tw, null);
                    float duration = (float)tSpesific.GetProperty("Duration").GetValue(tw, null);

                    MethodInfo onStep = tSpesific.GetMethod("InvokeStep");
                    MethodInfo onFinish = tSpesific.GetMethod("InvokeFinish");

                    if (start + duration < Time.realtimeSinceStartup)
                    {
                        onStep.Invoke(tw, new object[] { start + duration });
                        onFinish.Invoke(tw, null);
                        _tweens.RemoveAt(i);
                        continue;
                    }

                    onStep.Invoke(tw, new object[] { Time.realtimeSinceStartup });
                }

                await Task.Yield();
            }

            _working = false;
        }
    }

    public interface TweenData
    {
        public int ID { get; set; }
        public Type ValueType { get; }
    }

    public struct TweenData<T> : TweenData
    {
        private int _id;
        public int ID { get => _id; set => _id = value; }

        public T From { get; set; }
        public T To { get; set; }

        public float Start { get; set; }
        public float Duration { get; set; }
        public float End => Start + Duration;

        public Type ValueType { get => typeof(T); }

        public void InvokeStep(float t)
        {
            if (OnStep == null)
                return;

            T val = Operation(this, t);
            OnStep(val);
        }

        public void InvokeFinish() => OnFinish?.Invoke();

        public Action<T> OnStep;
        public Action OnFinish;

        public Func<TweenData<T>, float, T> Operation;

        public TweenData(int id,
                         T from,
                         T to,
                         float start,
                         float duration,
                         Action<T> step,
                         Action finish,
                         Func<TweenData<T>, float, T> operation)
        {
            _id = id;
            From = from;
            To = to;
            Start = start;
            Duration = duration;
            OnStep = step;
            OnFinish = finish;
            Operation = operation;
        }
    }
}
