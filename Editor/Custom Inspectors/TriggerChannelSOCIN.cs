using UnityEditor;
using UnityEngine;

namespace Euphrates.Editor
{
    [CustomEditor(typeof(TriggerChannelSO))]
    public class TriggerChannelSOCIN : UnityEditor.Editor
    {
        TriggerChannelSO _target;

        private void OnEnable()
        {
            _target = (TriggerChannelSO)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Trigger"))
                _target.Invoke();
        }
    }
}
