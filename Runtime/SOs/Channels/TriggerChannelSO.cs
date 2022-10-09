using UnityEngine;
using UnityEngine.Events;

namespace Euphrates
{
    [CreateAssetMenu(fileName = "New Trigger Channel", menuName = "SO Channels/Trigger")]
    public class TriggerChannelSO : ScriptableObject
    {
        [SerializeField] UnityEvent OnTrigger;

        [SerializeField] bool _silent = false;
        public bool Silent { get => _silent; set => _silent = value; }

        public void AddListener(UnityAction listener) => OnTrigger.AddListener(listener);
        public void RemoveListener(UnityAction listener) => OnTrigger.RemoveListener(listener);

        public void Invoke()
        {
            if (_silent)
                return;

            OnTrigger?.Invoke();
        }
    }
}
