using System;
using UnityEngine;

namespace Euphrates
{
    public static class TweenOps
    {
        static readonly float PI = Mathf.PI;
        static float Clp(float x) => Mathf.Clamp01(x);
        static float Cos(float x) => Mathf.Cos(x);
        static float Sin(float x) => Mathf.Sin(x);
        static float Pow(float x, float p) => Mathf.Pow(x, p);
        static float Sqrt(float x) => Mathf.Sqrt(x);

        #region Operations
        public static float FloatOperation(TweenData<float> data, float time, Func<float, float> ease)
        {
            float step = (time - data.Start) / data.Duration;
            float eased = ease(step);

            return data.From + data.To * eased;
        }

        public static Color ColorOperation(TweenData<Color> data, float time)
        {
            float step = (time - data.Start) / data.Duration;
            return Color.Lerp(data.From, data.To, step);
        }

        public static Vector3 Vector3Operation(TweenData<Vector3> data, float time, Func<float, float> ease)
        {
            float step = (time - data.Start) / data.Duration;
            float eased = ease(step);

            Vector3 dir = data.To - data.From;

            return data.From + dir * eased;
        }

        public static Vector2 Vector2Operation(TweenData<Vector2> data, float time, Func<float, float> ease)
        {
            float step = (time - data.Start) / data.Duration;
            float eased = ease(step);

            Vector2 dir = data.To - data.From;

            return data.From + dir * eased;
        }
        #endregion

        #region Ease Functions
        public static Func<float, float> GetEaseFunction(Ease easeFunction) => easeFunction switch
        {
            Ease.Lerp => (t) => Mathf.Lerp(0f, 1f, t),
            Ease.InSine => EaseInSine,
            Ease.OutSine => EaseOutSine,
            Ease.InOutSine => EaseInOutSine,
            Ease.InQuad => EaseInQuad,
            Ease.OutQuad => EaseOutQuad,
            Ease.InOutQuad => EaseInOutQuad,
            Ease.InCubic => EaseInCubic,
            Ease.OutCubic => EaseOutCubic,
            Ease.InOutCubic => EaseInOutCubic,
            Ease.InQuart => EaseInQuart,
            Ease.OutQuart => EaseOutQuart,
            Ease.InOutQuart => EaseInOutQuart,
            Ease.InQuint => EaseInQuint,
            Ease.OutQuint => EaseOutQuint,
            Ease.InOutQuint => EaseInOutQuint,
            Ease.InExpo => EaseInExpo,
            Ease.OutExpo => EaseOutExpo,
            Ease.InOutExpo => EaseInOutExpo,
            Ease.InCirc => EaseInCirc,
            Ease.OutCirc => EaseOutCirc,
            Ease.InOutCirc => EaseInOutCirc,
            Ease.InBack => EaseInBack,
            Ease.OutBack => EaseOutBack,
            Ease.InOutBack => EaseInOutBack,
            Ease.InElastic => EaseInElastic,
            Ease.OutElastic => EaseOutElastic,
            Ease.InOutElastic => EaseInOutElastic,
            Ease.InBounce => EaseInBounce,
            Ease.OutBounce => EaseOutBounce,
            Ease.InOutBounce => EaseInOutBounce,
            _ => (t) => Mathf.Lerp(0f, 1f, t),
        };

        static float EaseInSine(float time)
        {
            time = Clp(time);
            return 1f - Cos((time * PI) * .5f);
        }

        static float EaseOutSine(float time)
        {
            time = Clp(time);
            return 1f - Sin((time * PI) * .5f);
        }

        static float EaseInOutSine(float time)
        {
            time = Clp(time);
            return -(Cos(PI * time) - 1f) * .5f;
        }

        static float EaseInQuad(float time)
        {
            time = Clp(time);
            return Pow(time, 2);
        }

        static float EaseOutQuad(float time)
        {
            time = Clp(time);
            return 1f - Pow(1f - time, 2);
        }

        static float EaseInOutQuad(float time)
        {
            time = Clp(time);
            return time < .5f ? 2f * time * time : 1f - Pow(-2f * time + 2f, 2) * .5f;
        }

        static float EaseInCubic(float time)
        {
            time = Clp(time);
            return Pow(time, 3);
        }

        static float EaseOutCubic(float time)
        {
            time = Clp(time);
            return 1f - Pow(1f - time, 3);
        }

        static float EaseInOutCubic(float time)
        {
            time = Clp(time);
            return time < .5f ? 4 * Pow(time, 3) : 1f - Pow(-2f * time + 2f, 3) * .5f;
        }

        static float EaseInQuart(float time)
        {
            time = Clp(time);
            return Pow(time, 4);
        }

        static float EaseOutQuart(float time)
        {
            time = Clp(time);
            return 1f - Pow(1f - time, 4);
        }

        static float EaseInOutQuart(float time)
        {
            time = Clp(time);
            return time < .5f ? 8 * Pow(time, 4) : 1f - Pow(-2f * time + 2f, 4) * .5f;
        }

        static float EaseInQuint(float time)
        {
            time = Clp(time);
            return Pow(time, 5);
        }

        static float EaseOutQuint(float time)
        {
            time = Clp(time);
            return 1f - Pow(1f - time, 5);
        }

        static float EaseInOutQuint(float time)
        {
            time = Clp(time);
            return time < .5f ? 16 * Pow(time, 5) : 1f - Pow(-2f * time + 2f, 5) * .5f;
        }

        static float EaseInExpo(float time)
        {
            time = Clp(time);
            return time == 0f ? 0 : Pow(2f, 10f * time - 10f);
        }

        static float EaseOutExpo(float time)
        {
            time = Clp(time);
            return time == 1f ? 1f : 1f - Pow(2f, -10f * time);
        }

        static float EaseInOutExpo(float time)
        {
            time = Clp(time);
            return time == 0f ? 0f
                : time == 1f ? 1f
                : time < .5f ? Pow(2f, 20f * time - 10f) * .5f
                : (2f - Pow(2f, -20f * time + 10f)) * .5f;
        }

        static float EaseInCirc(float time)
        {
            time = Clp(time);
            return 1f - Sqrt(1f - Pow(time, 2));
        }

        static float EaseOutCirc(float time)
        {
            time = Clp(time);
            return Sqrt(1f - Pow(time - 1f, 2));
        }

        static float EaseInOutCirc(float time)
        {
            time = Clp(time);
            return time < .5f ? (1f - Sqrt(1f - Pow(2f * time, 2))) * .5f
                : (Sqrt(1f - Pow(-2f * time + 2f, 2)) + 1f) * .5f;
        }

        static float EaseInBack(float time)
        {
            time = Clp(time);

            float const1 = 1.70158f;
            float const2 = const1 + 1f;

            return const2 * Pow(time, 3) - const1 * Pow(time, 2);
        }

        static float EaseOutBack(float time)
        {
            time = Clp(time);

            float const1 = 1.70158f;
            float const2 = const1 + 1f;

            return 1f + const2 * Pow(time - 1f, 3) + const1 * Pow(time - 1f, 2);
        }

        static float EaseInOutBack(float time)
        {
            time = Clp(time);

            float const1 = 1.70158f;
            float const2 = const1 * 1.525f;

            return time < .5f ? (Pow(2f * time, 2) * ((const2 + 1f) * 2f * time - const2)) * .5f
                : (Pow(2f * time - 2f, 2) * ((const2 + 1f) * (time * 2f - 2f) + const2) + 2f) * .5f;
        }

        static float EaseInElastic(float time)
        {
            time = Clp(time);

            float const1 = (2f * PI) / 3f;

            return time == 0 ? 0
                : time == 1 ? 1
                : -Pow(2f, 10f * time - 10f) * Sin((time * 10f - 10.75f) * const1);
        }

        static float EaseOutElastic(float time)
        {
            time = Clp(time);

            float const1 = (2f * PI) / 3f;

            return time == 0f ? 0f
                : time == 1f ? 1f
                : Pow(2f, -10f * time) * Sin((time * 10f - .75f) * const1) + 1f;
        }

        static float EaseInOutElastic(float time)
        {
            time = Clp(time);

            float const1 = (2f * PI) / 4.5f;
            const float const2 = 11.125f;

            return time == 0f ? 0f
                : time == 1f ? 1f
                : time < .5f ? -(Pow(2f, 20f * time - 10f) * Sin((20f * time - const2) * const1)) * .5f
                : (Pow(2f, -20f * time + 10f) * Sin((20f * time - const2) * const1)) * .5f + 1f;
        }

        static float EaseInBounce(float time) => 1f - EaseOutBounce(1 - time);

        static float EaseOutBounce(float time)
        {
            time = Clp(time);

            float div = 2.75f;
            float mult = 7.5625f;

            if (time < 1 / div)
            {
                return mult * time * time;
            }
            else if (time < 2 / div)
            {
                time -= 1.5f / div;
                return mult * time * time + 0.75f;
            }
            else if (time < 2.5 / div)
            {
                time -= 2.25f / div;
                return mult * time * time + 0.9375f;
            }
            else
            {
                time -= 2.625f / div;
                return mult * time * time + 0.984375f;
            }
        }

        static float EaseInOutBounce(float time)
        {
            time = Clp(time);

            if (time < 0.5) return EaseInBounce(time * 2) / 2;
            return 1 - EaseInBounce((1 - time) * 2) / 2;
        }
        #endregion
    }

    public enum Ease
    {
        Lerp, InSine, OutSine, InOutSine, InQuad, OutQuad, InOutQuad, InCubic, OutCubic, InOutCubic, InQuart, OutQuart, InOutQuart, InQuint, OutQuint, InOutQuint,
        InExpo, OutExpo, InOutExpo, InCirc, OutCirc, InOutCirc, InBack, OutBack, InOutBack, InElastic, OutElastic, InOutElastic, InBounce, OutBounce, InOutBounce
    }
}
