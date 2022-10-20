using System;
using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [CustomPropertyDrawer(typeof(STPEventAttribute))]
    public class STPEventAttributeDrawer : PropertyDrawer
    {
        private static ISTPEventHandler m_EventHandler;
        private string[] m_EventNames;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_EventNames == null || m_EventNames.Length == 0)
                NoEventsMessage(position);

            if (property.propertyType == SerializedPropertyType.String)
            {
                if (TryGetEventHandler(property, out m_EventHandler) && m_EventHandler != null)
                {
                    m_EventNames = m_EventHandler.GetAllEventNames();
                    DrawEventsPopup(position, property, label);

                    return;
                }
            }

            ErrorMessage(position);
        }

        private void DrawEventsPopup(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_EventNames == null || m_EventNames.Length == 0)
                return;

            int selectedItem = STPEditorHelper.IndexOfString(property.stringValue, m_EventNames);
            selectedItem = EditorGUI.Popup(position, label.text, selectedItem, m_EventNames);
            property.stringValue = STPEditorHelper.StringAtIndex(selectedItem, m_EventNames);
        }

        private bool TryGetEventHandler(SerializedProperty property, out ISTPEventHandler manager)
        {
            var component = property.serializedObject.targetObject as Component;

            if (component == null)
                throw new InvalidCastException("Couldn't cast targetObject");

            manager = component.GetComponentInParent<ISTPEventHandler>();

            return manager != null;
        }

        private void ErrorMessage(Rect position)
        {
            EditorGUI.HelpBox(position, "The 'STP Event' attribute runs just on strings.", MessageType.Error);
        }

        private void NoEventsMessage(Rect position)
        {
            EditorGUI.HelpBox(position, "No events to hook to found.", MessageType.Info);
        }
    }
}