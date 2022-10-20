using UnityEditor;
using UnityEngine;
using Toolbox.Editor;

namespace SurvivalTemplatePro
{
    [CustomEditor(typeof(STPEventHandler))]
    public class STPEventsHandlerEditor : ToolboxEditor
    {
        private STPEventHandler m_EventsHandler;

        private bool m_FoldoutActive = false;
        private static Color m_StandardColor = new Color(0.8f, 0.8f, 0.8f, 0.9f);


        public override void DrawCustomInspector()
        {
            base.DrawCustomInspector();

            if (m_EventsHandler == null || Application.isPlaying)
                return;

            DrawEventsFoldout();

            if (m_FoldoutActive)
                EditorGUILayout.Space();

            DrawUpdateEventsButton();
        }

        private void OnEnable()
        {
            m_EventsHandler = target as STPEventHandler;
            m_FoldoutActive = false;
        }

        private void DrawUpdateEventsButton()
        {
            if (GUILayout.Button("Update Events"))
                m_EventsHandler.UpdateEvents();
        }

        private void DrawEventsFoldout() 
        {
            var eventSenderPairs = m_EventsHandler.EventSenderPairs;

            if (eventSenderPairs != null && eventSenderPairs.Count > 0)
            {
                EditorGUILayout.Space();

                GUI.color = m_StandardColor;
                GUILayout.BeginVertical(EditorStyles.helpBox);

                m_FoldoutActive = EditorGUILayout.Foldout(m_FoldoutActive, m_FoldoutActive ? "Hide Events" : "Show Events" + "...", true, STPEditorGUI.FoldOutStyle);

                if (m_FoldoutActive)
                {
                    STPEditorGUI.Separator();

                    int index = 1;

                    foreach (var eventPair in eventSenderPairs)
                    {
                        GUI.color = m_StandardColor;
                        GUILayout.BeginHorizontal();
                        DrawEvent(eventPair, index);
                        GUILayout.EndHorizontal();

                        index++;
                    }

                    EditorGUILayout.Space();

                    if (GUILayout.Button("Clear Events List"))
                        m_EventsHandler.ClearEvents();
                }

                GUILayout.EndVertical();
            }
        }

        private void DrawEvent(STPEventHandler.STPEventSenderPair eventSenderPair, int index)
        {
            GUILayout.Toggle(true, $"  {index}: {eventSenderPair.Event.EventName}", EditorStyles.radioButton);
            EditorGUILayout.ObjectField(eventSenderPair.Source, typeof(Component), false); 
        }
    }
}