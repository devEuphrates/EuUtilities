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

        #region Create
        static TweenData CreateTween<T>(string functionName, T val, T endVal, float duration, Ease easeFunction = Ease.Lerp, Action<T> onStep = null, Action onFinish = null)
        {
            try
            {
                Func<float, float> ease = TweenOps.GetEaseFunction(easeFunction);
                MethodInfo method = typeof(TweenOps).GetMethod(functionName);
                T Operation(TweenData<T> data, float t) => (T)method.Invoke(null, new object[] { data, t, ease });

                TweenData<T> data = new TweenData<T>(
                    UnityEngine.Random.Range(100000, 999999),
                    val,
                    endVal,
                    Time.realtimeSinceStartup,
                    duration,
                    onStep,
                    onFinish,
                    Operation);

                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static TweenData CreateTween(float val, float endVal, float duration, Ease easeFunction = Ease.Lerp, Action<float> onStep = null, Action onFinish = null)
            => CreateTween("FloatOperation", val, endVal, duration, easeFunction, onStep, onFinish);

        public static TweenData CreateTween(double val, double endVal, float duration, Ease easeFunction = Ease.Lerp, Action<double> onStep = null, Action onFinish = null)
            => CreateTween("DoubleOperation", val, endVal, duration, easeFunction, onStep, onFinish);

        public static TweenData CreateTween(int val, int endVal, float duration, Ease easeFunction = Ease.Lerp, Action<int> onStep = null, Action onFinish = null)
            => CreateTween("IntegerOperation", val, endVal, duration, easeFunction, onStep, onFinish);

        public static TweenData CreateTween(Color val, Color endVal, float duration, Ease easeFunction = Ease.Lerp, Action<Color> onStep = null, Action onFinish = null)
            => CreateTween("ColorOperation", val, endVal, duration, easeFunction, onStep, onFinish);

        public static TweenData CreateTween(Vector3 val, Vector3 endVal, float duration, Ease easeFunction = Ease.Lerp, Action<Vector3> onStep = null, Action onFinish = null)
            => CreateTween("Vector3Operation", val, endVal, duration, easeFunction, onStep, onFinish);

        public static TweenData CreateTween(Vector2 val, Vector2 endVal, float duration, Ease easeFunction = Ease.Lerp, Action<Vector2> onStep = null, Action onFinish = null)
            => CreateTween("Vector2Operation", val, endVal, duration, easeFunction, onStep, onFinish);

        public static TweenData CreateTween(Quaternion val, Quaternion endVal, float duration, Ease easeFunction = Ease.Lerp, Action<Quaternion> onStep = null, Action onFinish = null)
            => CreateTween("QuaternionOperation", val, endVal, duration, easeFunction, onStep, onFinish);
        #endregion

        #region Do
        static TweenData DoTween<T>(string functionName, T val, T endVal, float duration, Ease easeFunction = Ease.Lerp, Action<T> onStep = null, Action onFinish = null)
        {
            try
            {
                TweenData data = CreateTween(functionName, val, endVal, duration, easeFunction, onStep, onFinish);
                StartTween(data);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static TweenData DoTween<T>(T val, T endVal, float duration, Ease easeFunction = Ease.Lerp, Action<T> onStep = null, Action onFinish = null)
        => DoTween<T>($"{typeof(T).Name}Operation", val, endVal, duration, easeFunction, onStep, onFinish);

        public static TweenData DoTween(float val, float endVal, float duration, Ease easeFunction = Ease.Lerp, Action<float> onStep = null, Action onFinish = null)
            => DoTween("FloatOperation", val, endVal, duration, easeFunction, onStep, onFinish);

        public static TweenData DoTween(double val, double endVal, float duration, Ease easeFunction = Ease.Lerp, Action<double> onStep = null, Action onFinish = null)
            => DoTween("DoubleOperation", val, endVal, duration, easeFunction, onStep, onFinish);

        public static TweenData DoTween(int val, int endVal, float duration, Ease easeFunction = Ease.Lerp, Action<int> onStep = null, Action onFinish = null)
            => DoTween("IntegerOperation", val, endVal, duration, easeFunction, onStep, onFinish);

        public static TweenData DoTween(Color val, Color endVal, float duration, Ease easeFunction = Ease.Lerp, Action<Color> onStep = null, Action onFinish = null)
            => DoTween("ColorOperation", val, endVal, duration, easeFunction, onStep, onFinish);

        public static TweenData DoTween(Vector3 val, Vector3 endVal, float duration, Ease easeFunction = Ease.Lerp, Action<Vector3> onStep = null, Action onFinish = null)
            => DoTween("Vector3Operation", val, endVal, duration, easeFunction, onStep, onFinish);

        public static TweenData DoTween(Vector2 val, Vector2 endVal, float duration, Ease easeFunction = Ease.Lerp, Action<Vector2> onStep = null, Action onFinish = null)
            => DoTween("Vector2Operation", val, endVal, duration, easeFunction, onStep, onFinish);

        public static TweenData DoTween(Quaternion val, Quaternion endVal, float duration, Ease easeFunction = Ease.Lerp, Action<Quaternion> onStep = null, Action onFinish = null)
            => DoTween("QuaternionOperation", val, endVal, duration, easeFunction, onStep, onFinish);
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

        public static void StartTween(TweenData data)
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

                    //Type tVal = tw.GetType().GetProperty("From").PropertyType;
                    //Type tBase = typeof(TweenData<>);
                    //Type tSpesific = tBase.MakeGenericType(tVal);

                    //float start = (float)tSpesific.GetProperty("Start").GetValue(tw, null);
                    //float duration = (float)tSpesific.GetProperty("Duration").GetValue(tw, null);

                    //MethodInfo onStep = tSpesific.GetMethod("InvokeStep");
                    //MethodInfo onFinish = tSpesific.GetMethod("InvokeFinish");

                    float start = tw.Start;
                    float duration = tw.Duration;

                    if (start + duration < Time.realtimeSinceStartup)
                    {
                        //onStep.Invoke(tw, new object[] { start + duration });
                        //onFinish.Invoke(tw, null);
                        tw.InvokeStep(start + duration);
                        tw.InvokeFinish();
                        _tweens.RemoveAt(i);
                        continue;
                    }

                    //onStep.Invoke(tw, new object[] { Time.realtimeSinceStartup });
                    tw.InvokeStep(Time.realtimeSinceStartup);
                }

                await Task.Yield();
            }

            _working = false;
        }

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

        public static void DoMove(this Transform transform, Vector3 move, float duration, Ease easeFunction = Ease.Lerp)
            => DoPosition(transform, transform.position + move, duration, easeFunction);

        public static void DoRotation(this Transform transform, Quaternion rotation, float duration, Ease easeFunction = Ease.Lerp, Action<Quaternion> onStep = null, Action onFinish = null)
        {
            Quaternion start = transform.rotation;
            Quaternion end = rotation;

            void Step(Quaternion val)
            {
                transform.rotation = val;
                onStep?.Invoke(val);
            }

            DoTween(start, end, duration, easeFunction, Step, onFinish);
        }

        public static void DoRotate(this Transform transform, Quaternion rotate, float duration, Ease easeFunction = Ease.Lerp)
           => DoRotation(transform, transform.rotation * rotate, duration, easeFunction);

        public static void DoScale(this Transform transform, Vector3 scale, float duration, Ease easeFunction = Ease.Lerp, Action<Vector3> onStep = null, Action onFinish = null)
        {
            Vector3 start = transform.localScale;
            Vector3 end = scale;

            void Step(Vector3 val)
            {
                transform.localScale = val;
                onStep?.Invoke(val);
            }

            DoTween(start, end, duration, easeFunction, Step, onFinish);
        }

        public static void DoAlpha(this CanvasGroup canvasGroup, float alpha, float duration, Ease easeFunction = Ease.Lerp, Action<float> onStep = null, Action onFinish = null)
        {
            float start = canvasGroup.alpha;
            float end = alpha;

            void Step(float val)
            {
                canvasGroup.alpha = val;
                onStep?.Invoke(val);
            }

            DoTween(start, end, duration, easeFunction, Step, onFinish);
        }

        public static void DoColor(this MeshRenderer renderer, Color color, float duration, Ease easeFunction = Ease.Lerp, Action<Color> onStep = null, Action onFinish = null)
        {
            Color start = renderer.material.color;
            Color end = color;

            void Step(Color val)
            {
                renderer.material.color = val;
                onStep?.Invoke(val);
            }

            DoTween(start, end, duration, easeFunction, Step, onFinish);
        }

        public static void DoColor(this MeshRenderer renderer, string colorName, Color color, float duration, Ease easeFunction = Ease.Lerp, Action<Color> onStep = null, Action onFinish = null)
        {
            Color start = renderer.material.GetColor(colorName);
            Color end = color;

            void Step(Color val)
            {
                renderer.material.SetColor(colorName, val);
                onStep?.Invoke(val);
            }

            DoTween(start, end, duration, easeFunction, Step, onFinish);
        }
        #endregion

        #region Animations

        public static void DoJump()
        {

        }

        #endregion
    }

    public interface TweenData
    {
        public int ID { get; set; }

        public float Start { get; set; }
        public float Duration { get; set; }
        public float End => Start + Duration;

        public void InvokeStep(float t);
        public void InvokeFinish();

        public void Stop();
        public void Then(TweenData tween);

        public void AddFinishListener(Action listener);
        public void RemoveFinishListener(Action listener);
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

        public void Stop() => Tween.StopTween(ID);

        public void Then(TweenData tween) => OnFinish += () => Tween.StartTween(tween);

        public void AddFinishListener(Action listener) => OnFinish += listener;

        public void RemoveFinishListener(Action listener) => OnFinish -= listener;

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

    public class TweenQueue
    {
        public event Action OnFinish;
        public event Action<TweenData> OnFinishStep;

        List<TweenData> _tweens;
        int _currentIndex;

        bool _working = false;
        public bool Working => _working;

        public TweenQueue(TweenData first, Action onFinish, Action<TweenData> onFinishStep)
        {
            OnFinish = onFinish;
            OnFinishStep = onFinishStep;

            _tweens = new List<TweenData>();
            _tweens.Add(first);

            _currentIndex = 0;
        }

        public TweenQueue(TweenData first, Action onFinish)
        {
            OnFinish = onFinish;
            OnFinishStep = null;

            _tweens = new List<TweenData>();
            _tweens.Add(first);

            _currentIndex = 0;
        }

        public TweenQueue(TweenData first)
        {
            OnFinish = null;
            OnFinishStep = null;

            _tweens = new List<TweenData>();
            _tweens.Add(first);

            _currentIndex = 0;
        }

        public void AddTween(TweenData tween)
        {
            TweenData last = _tweens[_tweens.Count - 1];
            last.Then(tween);
            last.AddFinishListener(() => _currentIndex++);

            _tweens[_tweens.Count - 1] = last;
            _tweens.Add(tween);
        }

        void ResetWorkState()
        {
            _working = false;
            _currentIndex = 0;
        }

        public void Start()
        {
            if (_working)
                return;

            _working = true;

            TweenData last = _tweens[_tweens.Count - 1];
            last.AddFinishListener(ResetWorkState);
            _tweens[_tweens.Count - 1] = last;

            Tween.StartTween(_tweens[0]);
        }

        public void End()
        {
            if (!_working)
                return;

            Tween.StopTween(_tweens[_currentIndex].ID);
            ResetWorkState();
        }
    }
}
