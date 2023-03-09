using UnityEngine;
using UnityEngine.Events;

namespace Euphrates
{
    [AddComponentMenu("Event Listeners/Object Event Listener")]
    public class ObjectEventListener : MonoBehaviour
    {
        [SerializeField] ObjectEventSO _event;
        public UnityEvent<Object> OnTrigger;

        private void OnEnable() => _event.AddListener(OnTrigger.Invoke);

        private void OnDisable() => _event.RemoveListener(OnTrigger.Invoke);
    }
}
