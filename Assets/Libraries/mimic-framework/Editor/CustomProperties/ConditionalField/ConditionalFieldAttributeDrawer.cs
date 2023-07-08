using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Reflection;
using UnityEditor;

//Credits https://github.com/Deadcows/MyBox/blob/master/Attributes/ConditionalFieldAttribute.cs
namespace Mimic.Internal
{

    [CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
    public class ConditionalFieldAttributeDrawer : PropertyDrawer {
        private ConditionalFieldAttribute _attribute;

        private bool _customDrawersCached;
        private static IEnumerable<Type> _typesCache;
        private bool _multipleAttributes;
        private bool _specialType;
        private PropertyAttribute _genericAttribute;
        private PropertyDrawer _genericDrawerInstance;
        private Type _genericDrawerType;
        private Type _genericType;
        private PropertyDrawer _genericTypeDrawerInstance;
        private Type _genericTypeDrawerType;


        /// <summary>
        /// If conditional is part of type in collection, we need to link properties as in collection
        /// </summary>
        private readonly Dictionary<SerializedProperty, SerializedProperty> _conditionalToTarget =
            new Dictionary<SerializedProperty, SerializedProperty>();
        private bool _toShow = true;


        private void Initialize(SerializedProperty property) {
            if (_attribute == null) _attribute = attribute as ConditionalFieldAttribute;
            if (_attribute == null) return;

            if (!_conditionalToTarget.ContainsKey(property))
                _conditionalToTarget.Add(property, ConditionalFieldUtility.FindRelativeProperty(property, _attribute.FieldToCheck));


            if (_customDrawersCached) return;
            if (_typesCache == null) {
                _typesCache = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                    .Where(x => typeof(PropertyDrawer).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
            }

            if (HaveMultipleAttributes()) {
                _multipleAttributes = true;
                GetPropertyDrawerType(property);
            } else if (!fieldInfo.FieldType.Module.ScopeName.Equals(typeof(int).Module.ScopeName)) {
                _specialType = true;
                GetTypeDrawerType(property);
            }

            _customDrawersCached = true;
        }

        private bool HaveMultipleAttributes() {
            if (fieldInfo == null) return false;
            var genericAttributeType = typeof(PropertyAttribute);
            var attributes = fieldInfo.GetCustomAttributes(genericAttributeType, false);
            if (attributes==null || attributes.Length==0) return false;
            return attributes.Length > 1;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            Initialize(property);

            _toShow = ConditionalFieldUtility.PropertyIsVisible(_conditionalToTarget[property], _attribute.Inverse, _attribute.CompareValues);
            if (!_toShow) return 0;

            if (_genericDrawerInstance != null)
                return _genericDrawerInstance.GetPropertyHeight(property, label);
            if (_genericTypeDrawerInstance != null)
                return _genericTypeDrawerInstance.GetPropertyHeight(property, label);
            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (!_toShow) return;

            if (_multipleAttributes && _genericDrawerInstance != null) {
                try {
                    _genericDrawerInstance.OnGUI(position, property, label);
                } catch (Exception e) {
                    EditorGUI.PropertyField(position, property, label);
                    LogWarning("Unable to instantiate " + _genericAttribute.GetType() + " : " + e, property);
                }
            } else if (_specialType && _genericTypeDrawerInstance != null) {
                try {
                    _genericTypeDrawerInstance.OnGUI(position, property, label);
                } catch (Exception e) {
                    EditorGUI.PropertyField(position, property, label);
                    LogWarning("Unable to instantiate " + _genericType + " : " + e, property);
                }
            } else {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        private void LogWarning(string log, SerializedProperty property) {
            var warning = "Property <color=brown>" + fieldInfo.Name + "</color>";
            if (fieldInfo.DeclaringType != null) warning += " on behaviour <color=brown>" + fieldInfo.DeclaringType.Name + "</color>";
            warning += " caused: " + log;

            Debug.LogWarning(warning, property.serializedObject.targetObject);
        }


        #region Get Custom Property/Type drawers

        private void GetPropertyDrawerType(SerializedProperty property) {
            if (_genericDrawerInstance != null) return;

            //Get the second attribute flag
            try {
                _genericAttribute = (PropertyAttribute)fieldInfo.GetCustomAttributes(typeof(PropertyAttribute), false)[1];

                if (_genericAttribute is ContextMenuItemAttribute ||
                    _genericAttribute is SeparatorAttribute) {
                    LogWarning("[ConditionalField] does not work with " + _genericAttribute.GetType(), property);
                    return;
                }

                if (_genericAttribute is TooltipAttribute) return;
            } catch (Exception e) {
                LogWarning("Can't find stacked propertyAttribute after ConditionalProperty: " + e, property);
                return;
            }

            //Get the associated attribute drawer
            try {
                _genericDrawerType = _typesCache.First(x =>
                    (Type)CustomAttributeData.GetCustomAttributes(x).First().ConstructorArguments.First().Value == _genericAttribute.GetType());
            } catch (Exception e) {
                LogWarning("Can't find property drawer from CustomPropertyAttribute of " + _genericAttribute.GetType() + " : " + e, property);
                return;
            }

            //Create instances of each (including the arguments)
            try {
                _genericDrawerInstance = (PropertyDrawer)Activator.CreateInstance(_genericDrawerType);
                //Get arguments
                IList<CustomAttributeTypedArgument> attributeParams = fieldInfo.GetCustomAttributesData()[1].ConstructorArguments;
                IList<CustomAttributeTypedArgument> unpackedParams = new List<CustomAttributeTypedArgument>();
                //Unpack any params object[] args
                foreach (CustomAttributeTypedArgument singleParam in attributeParams) {
                    if (singleParam.Value.GetType() == typeof(ReadOnlyCollection<CustomAttributeTypedArgument>)) {
                        foreach (CustomAttributeTypedArgument unpackedSingleParam in (ReadOnlyCollection<CustomAttributeTypedArgument>)singleParam
                            .Value) {
                            unpackedParams.Add(unpackedSingleParam);
                        }
                    } else {
                        unpackedParams.Add(singleParam);
                    }
                }

                object[] attributeParamsObj = unpackedParams.Select(x => x.Value).ToArray();

                if (attributeParamsObj.Any()) {
                    _genericAttribute = (PropertyAttribute)Activator.CreateInstance(_genericAttribute.GetType(), attributeParamsObj);
                } else {
                    _genericAttribute = (PropertyAttribute)Activator.CreateInstance(_genericAttribute.GetType());
                }
            } catch (Exception e) {
                LogWarning("No constructor available in " + _genericAttribute.GetType() + " : " + e, property);
                return;
            }

            //Reassign the attribute field in the drawer so it can access the argument values
            try {
                _genericDrawerType.GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(_genericDrawerInstance, _genericAttribute);
            } catch (Exception e) {
                LogWarning("Unable to assign attribute to " + _genericDrawerInstance.GetType() + " : " + e, property);
            }
        }


        private void GetTypeDrawerType(SerializedProperty property) {
            if (_genericTypeDrawerInstance != null) return;

            //Get the type
            _genericType = fieldInfo.FieldType;

            var _genericObject = fieldInfo;

            //Get the associated attribute drawer
            try {
                _genericTypeDrawerType = _typesCache.First(x =>
                    (Type)CustomAttributeData.GetCustomAttributes(x).First().ConstructorArguments.First().Value == _genericType);
            } catch (Exception) {
                // Commented out because of multiple false warnings on Behaviour types
                //LogWarning("[ConditionalField] does not work with "+_genericType+". Unable to find property drawer from the Type", property);
                return;
            }

            //Create instances of each (including the arguments)
            try {
                _genericTypeDrawerInstance = (PropertyDrawer)Activator.CreateInstance(_genericTypeDrawerType);
            } catch (Exception e) {
                LogWarning("no constructor available in " + _genericType + " : " + e, property);
                return;
            }

            //Reassign the attribute field in the drawer so it can access the argument values
            try {
                _genericTypeDrawerType.GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(_genericTypeDrawerInstance, _genericObject);
            } catch (Exception) {
                //LogWarning("Unable to assign attribute to " + _genericTypeDrawerInstance.GetType() + " : " + e, property);
            }
        }

        #endregion
    }
}