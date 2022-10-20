using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using System;
#endif

namespace SurvivalTemplatePro
{
    /// <summary>
    /// TODO: Hash the event names
    /// </summary>
    [AddComponentMenu("Wieldables/Event Handler")]
    [DisallowMultipleComponent()]
    public class STPEventHandler : MonoBehaviour, ISTPEventHandler
    {
        #region Internal
        [System.Serializable]
        public class STPEventSenderPair
        {
            public STPEvent Event;

#if UNITY_EDITOR
            public Component Source;
#endif


            public STPEventSenderPair(string eventName, Component source)
            {
                this.Event = new STPEvent(eventName);

#if UNITY_EDITOR
                this.Source = source;
#endif
            }
        }
        #endregion

        public List<STPEventSenderPair> EventSenderPairs => m_Events;

        [SerializeField, HideInInspector]
        private List<STPEventSenderPair> m_Events = new List<STPEventSenderPair>();

        private Dictionary<string, STPEvent> m_EventsNameDictionary;


        public void TriggerAction(string name, float value)
        {
            if (m_EventsNameDictionary == null)
                GenerateDictionary();

            if (m_EventsNameDictionary.TryGetValue(name, out STPEvent stpEvent))
                stpEvent.TriggerAction(value);
        }

        public void TriggerAction(STPEventReference eventReference, float value = 1f)
        {
            if (m_EventsNameDictionary == null)
                GenerateDictionary();

            if (m_EventsNameDictionary.TryGetValue(eventReference.Name, out STPEvent stpEvent))
                stpEvent.TriggerAction(value);
        }

        public bool TryGetEventWithName(string name, out STPEvent stpEvent)
        {
            if (m_EventsNameDictionary == null)
                GenerateDictionary();

            if (m_EventsNameDictionary.TryGetValue(name, out stpEvent))
                return true;

            return false;
        }

        private void GenerateDictionary() 
        {
            m_EventsNameDictionary = new Dictionary<string, STPEvent>();

            foreach (var stpEvent in m_Events)
                m_EventsNameDictionary.Add(stpEvent.Event.EventName, stpEvent.Event);
        }

#if UNITY_EDITOR
        public void UpdateEvents()
        {
            ISTPEventReferenceHandler[] referenceHandlers = GetComponentsInChildren<ISTPEventReferenceHandler>();

            foreach (var referenceHandler in referenceHandlers)
            {
                foreach (var eventRef in GetAllEventReferencesFromHandler(referenceHandler.GetEventReferencesSource()))
                    AddAction(eventRef, referenceHandler.GetEventReferencesSource());
            }

            CleanEvents();

            EditorUtility.SetDirty(this);
        }

        public void ClearEvents()
        {
            m_Events.Clear();
            EditorUtility.SetDirty(this);
        }

        public string[] GetAllEventNames()
        {
            List<string> eventNames = new List<string>();

            foreach (var eventPair in m_Events)
                eventNames.Add(eventPair.Event.EventName);

            return eventNames.ToArray();
        }

        private void AddAction(STPEventReference eventRef, Component source)
        {
            string newEventName = GetNewActionName(eventRef.DefaultName, source, eventRef.SetToExistingIfAvailable);
            eventRef.SetName(newEventName);

            if (!ContainsEventWithName(newEventName))
                m_Events.Add(new STPEventSenderPair(newEventName, source));

            EditorUtility.SetDirty(source);
        }

        private string GetNewActionName(string eventName, Component source, bool setToExistingIfAvailable)
        {
            eventName = GetEventNameWithParent(eventName, source.GetType());

            bool containsEventWithName = ContainsEventWithName(eventName);
            bool containsEventWithSameParams = ContainsEventWithParams(eventName, source);

            if (!setToExistingIfAvailable && containsEventWithName && !containsEventWithSameParams)
                eventName = GetEventNameWithNextIndex(eventName);

            return eventName;
        }

        private string GetEventNameWithParent(string eventName, System.Type type)
        {
            return string.Format(eventName, type.Name.ToUnityLikeNameFormat());
        }

        private string GetEventNameWithNextIndex(string eventName)
        {
            string newName = eventName;

            int currentIndex = GetAllEventsWithName(eventName).Count;

            if (eventName.Contains("-"))
            {
                int symbolIndex = newName.LastIndexOf("-") + 1;
                int newIndex = int.Parse(newName.Substring(symbolIndex));
                newName = newName.Remove(symbolIndex);
                newName += (newIndex + 1).ToString();
            }
            else
                newName += $"-{currentIndex}";

            return newName;
        }

        private List<string> GetAllEventsWithName(string eventName) 
        {
            List<string> names = new List<string>();

            foreach (var eventPair in m_Events)
            {
                if (eventPair.Event.EventName.Contains(eventName))
                    names.Add(eventPair.Event.EventName);
            }

            return names;
        }

        private bool ContainsEventWithName(string name)
        {
            bool contains = false;

            foreach (var stpEvent in m_Events)
            {
                if (stpEvent.Event.EventName == name)
                    return true;
            }

            return contains;
        }

        private bool ContainsEventWithParams(string name, Component component)
        {
            bool contains = false;

            foreach (var stpEvent in m_Events)
            {
                if (stpEvent.Event.EventName.Contains(name) && stpEvent.Source == component)
                    return true;
            }

            return contains;
        }

        private void CleanEvents() 
        {
            foreach (var eventPair in m_Events)
            {
                if (eventPair.Source == null)
                {
                    m_Events.Remove(eventPair);
                    break;
                }
            }
        }

        private static List<STPEventReference> GetAllEventReferencesFromHandler(Component component)
        {
            List<STPEventReference> eventReferences = new List<STPEventReference>();

            Type compType = component.GetType();
            BindingFlags bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic;
            FieldInfo[] privateFields = GetFieldInfosIncludingBaseClasses(compType, bindingFlags);

            foreach (FieldInfo fieldInfo in privateFields)
            {
                if (fieldInfo.FieldType == typeof(STPEventReference))
                    eventReferences.Add(fieldInfo.GetValue(component) as STPEventReference);
            }

            return eventReferences;
        }

        private static FieldInfo[] GetFieldInfosIncludingBaseClasses(Type type, BindingFlags bindingFlags)
        {
            FieldInfo[] fieldInfos = type.GetFields(bindingFlags);

            // If this class doesn't have a base, don't waste any time
            if (type.BaseType == typeof(object))
            {
                return fieldInfos;
            }
            else
            { // Otherwise, collect all types up to the furthest base class
                var fieldInfoList = new List<FieldInfo>(fieldInfos);
                while (type.BaseType != typeof(object))
                {
                    type = type.BaseType;
                    fieldInfos = type.GetFields(bindingFlags);

                    // Look for fields we do not have listed yet and merge them into the main list
                    for (int index = 0; index < fieldInfos.Length; ++index)
                    {
                        bool found = false;

                        for (int searchIndex = 0; searchIndex < fieldInfoList.Count; ++searchIndex)
                        {
                            bool match =
                                (fieldInfoList[searchIndex].DeclaringType == fieldInfos[index].DeclaringType) &&
                                (fieldInfoList[searchIndex].Name == fieldInfos[index].Name);

                            if (match)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            fieldInfoList.Add(fieldInfos[index]);
                        }
                    }
                }

                return fieldInfoList.ToArray();
            }
        }
#endif
    }
}