using UnityEngine;
using UnityEngine.Events;

namespace Euphrates
{
    [CreateAssetMenu(fileName = "New Object Channel", menuName = "SO Channels/Object")]
    public abstract class ObjectEventSO : ScriptableObject
    {
        public UnityEvent<Object> OnTrigger;
        public bool Silent = false;

        public void AddListener(UnityAction<Object> listener) => OnTrigger.AddListener(listener);
        public void RemoveListener(UnityAction<Object> listener) => OnTrigger.RemoveListener(listener);
        public void RemoveAllListeners() => OnTrigger.RemoveAllListeners();

        public void Invoke(Object obj)
        {
            if (Silent)
                return;

            OnTrigger?.Invoke(obj);
        }
    }
}
