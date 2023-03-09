using UnityEngine;
using UnityEngine.Events;

namespace Euphrates
{
    public abstract class SOVariableChangeListener<T> : MonoBehaviour
    {
        [SerializeField] SOVariable<T> _variable;
        public UnityEvent<T> OnValueChanged;

        protected virtual void OnEnable() => _variable.OnChange += OnValueChanged.Invoke;

        protected virtual void OnDisable() => _variable.OnChange -= OnValueChanged.Invoke;
    }
}
