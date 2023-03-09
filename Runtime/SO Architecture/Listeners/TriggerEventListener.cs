using UnityEngine;
using UnityEngine.Events;

namespace Euphrates
{
    [AddComponentMenu("Event Listeners/Trigger Event Listener")]
    public class TriggerEventListener : MonoBehaviour
    {
        [SerializeField] TriggerChannelSO _event;
        public UnityEvent OnTrigger;

        private void OnEnable() => _event.AddListener(OnTrigger.Invoke);

        private void OnDisable() => _event.RemoveListener(OnTrigger.Invoke);
    }
}
