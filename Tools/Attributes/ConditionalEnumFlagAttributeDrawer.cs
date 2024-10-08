#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalEnumFlagAttribute))]
public class ConditionalEnumFlagAttributeDrawer : PropertyDrawer
{
    #region Members

    private static IEnumerable<Type> _allPropertyDrawerAttributeTypes;
    private readonly Dictionary<SerializedProperty, SerializedProperty> _conditionalToTarget = new Dictionary<SerializedProperty, SerializedProperty>();
    private bool _customDrawersCached;
    private bool _multipleAttributes;
    private bool _specialType;
    private PropertyAttribute _genericAttribute;
    private PropertyDrawer _genericAttributeDrawerInstance;
    private Type _genericAttributeDrawerType;
    private Type _genericType;
    private PropertyDrawer _genericTypeDrawerInstance;
    private Type _genericTypeDrawerType;
    private bool _toShow = true;

    #endregion Members

    #region Properties

    private ConditionalEnumFlagAttribute ConditionalEnumFlagAttribute => attribute as ConditionalEnumFlagAttribute;

    #endregion Properties

    #region API Methods
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Initialize(property);

        _toShow = ConditionalFieldUtility.PropertyIsVisible(_conditionalToTarget[property], ConditionalEnumFlagAttribute.inverse, ConditionalEnumFlagAttribute.compareValue);
        if (!_toShow)
            return 0;

        if (_genericAttributeDrawerInstance != null)
            return _genericAttributeDrawerInstance.GetPropertyHeight(property, label);

        if (_genericTypeDrawerInstance != null)
            return _genericTypeDrawerInstance.GetPropertyHeight(property, label);

        return EditorGUI.GetPropertyHeight(property);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!_toShow)
            return;

        if (_multipleAttributes && _genericAttributeDrawerInstance != null)
        {
            try
            {
                _genericAttributeDrawerInstance.OnGUI(position, property, label);
            }
            catch (Exception e)
            {
                EditorGUI.PropertyField(position, property, label);
                LogWarning("Unable to instantiate " + _genericAttribute.GetType() + " : " + e, property);
            }
        }
        else if (_specialType && _genericTypeDrawerInstance != null)
        {
            try
            {
                _genericTypeDrawerInstance.OnGUI(position, property, label);
            }
            catch (Exception e)
            {
                EditorGUI.PropertyField(position, property, label);
                LogWarning("Unable to instantiate " + _genericType + " : " + e, property);
            }
        }
        else
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    #endregion API Methods

    #region Class Methods

    private void Initialize(SerializedProperty property)
    {
        if (!_conditionalToTarget.ContainsKey(property))
            _conditionalToTarget.Add(property, ConditionalFieldUtility.FindRelativeProperty(property, ConditionalEnumFlagAttribute.fieldToCheck));

        if (_customDrawersCached)
            return;

        if (_allPropertyDrawerAttributeTypes == null)
        {
            _allPropertyDrawerAttributeTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(PropertyDrawer).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
        }

        if (HaveMultipleAttributes())
        {
            _multipleAttributes = true;
            GetPropertyDrawerType(property);
        }
        else if (fieldInfo != null && !fieldInfo.FieldType.Module.ScopeName.Equals(typeof(int).Module.ScopeName))
        {
            _specialType = true;
            GetTypeDrawerType(property);
        }

        _customDrawersCached = true;
    }

    private bool HaveMultipleAttributes()
    {
        if (fieldInfo == null)
            return false;

        var genericAttributeType = typeof(PropertyAttribute);
        var attributes = fieldInfo.GetCustomAttributes(genericAttributeType, false);

        if (attributes != null || attributes.Length == 0)
            return false;

        return attributes.Length > 1;
    }

    private void LogWarning(string log, SerializedProperty property)
    {
        var warning = "Property <color=brown>" + fieldInfo.Name + "</color>";
        if (fieldInfo != null && fieldInfo.DeclaringType != null)
            warning += " on behaviour <color=brown>" + fieldInfo.DeclaringType.Name + "</color>";
        warning += " caused: " + log;
    }

    private void GetPropertyDrawerType(SerializedProperty property)
    {
        if (_genericAttributeDrawerInstance != null)
            return;

        // Get the second attribute flag.
        try
        {
            _genericAttribute = (PropertyAttribute)fieldInfo.GetCustomAttributes(typeof(PropertyAttribute), false)
                                                            .FirstOrDefault(a => !(a is ConditionalEnumFlagAttribute));

            if (_genericAttribute is ContextMenuItemAttribute || _genericAttribute is TooltipAttribute)
            {
                LogWarning("[ConditionalField] does not work with " + _genericAttribute.GetType(), property);
                return;
            }
        }
        catch (Exception e)
        {
            LogWarning("Can't find stacked propertyAttribute after ConditionalProperty: " + e, property);
            return;
        }

        // Get the associated attribute drawer.
        try
        {
            _genericAttributeDrawerType = _allPropertyDrawerAttributeTypes.First(x =>
                (Type)CustomAttributeData.GetCustomAttributes(x).First().ConstructorArguments.First().Value == _genericAttribute.GetType());
        }
        catch (Exception e)
        {
            LogWarning("Can't find property drawer from CustomPropertyAttribute of " + _genericAttribute.GetType() + " : " + e, property);
            return;
        }

        // Create instances of each (including the arguments).
        try
        {
            _genericAttributeDrawerInstance = (PropertyDrawer)Activator.CreateInstance(_genericAttributeDrawerType);
            IList<CustomAttributeTypedArgument> attributeParams = fieldInfo.GetCustomAttributesData()
            .First(a => a.AttributeType == _genericAttribute.GetType()).ConstructorArguments;
            IList<CustomAttributeTypedArgument> unpackedParams = new List<CustomAttributeTypedArgument>();
            foreach (CustomAttributeTypedArgument singleParam in attributeParams)
            {
                if (singleParam.Value.GetType() == typeof(ReadOnlyCollection<CustomAttributeTypedArgument>))
                {
                    foreach (CustomAttributeTypedArgument unpackedSingleParam in (ReadOnlyCollection<CustomAttributeTypedArgument>)singleParam.Value)
                    {
                        unpackedParams.Add(unpackedSingleParam);
                    }
                }
                else
                {
                    unpackedParams.Add(singleParam);
                }
            }

            object[] attributeParamsObj = unpackedParams.Select(x => x.Value).ToArray();

            if (attributeParamsObj.Any())
            {
                _genericAttribute = (PropertyAttribute)Activator.CreateInstance(_genericAttribute.GetType(), attributeParamsObj);
            }
            else
            {
                _genericAttribute = (PropertyAttribute)Activator.CreateInstance(_genericAttribute.GetType());
            }
        }
        catch (Exception e)
        {
            LogWarning("No constructor available in " + _genericAttribute.GetType() + " : " + e, property);
            return;
        }

        // Reassign the attribute field in the drawer so it can access the argument values.
        try
        {
            var genericDrawerAttributeField = _genericAttributeDrawerType.GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic);
            genericDrawerAttributeField.SetValue(_genericAttributeDrawerInstance, _genericAttribute);
        }
        catch (Exception e)
        {
            LogWarning("Unable to assign attribute to " + _genericAttributeDrawerInstance.GetType() + " : " + e, property);
        }
    }

    private void GetTypeDrawerType(SerializedProperty property)
    {
        if (_genericTypeDrawerInstance != null) return;

        // Get the associated attribute drawer.
        try
        {
            // Of all property drawers in the assembly we need to find one that affects target type
            // or one of the base types of target type.
            foreach (Type propertyDrawerType in _allPropertyDrawerAttributeTypes)
            {
                _genericType = fieldInfo.FieldType;
                var affectedType = (Type)CustomAttributeData.GetCustomAttributes(propertyDrawerType).First().ConstructorArguments.First().Value;
                while (_genericType != null)
                {
                    if (_genericTypeDrawerType != null)
                        break;

                    if (affectedType == _genericType)
                        _genericTypeDrawerType = propertyDrawerType;
                    else
                        _genericType = _genericType.BaseType;
                }

                if (_genericTypeDrawerType != null)
                    break;
            }
        }
        catch (Exception)
        {
            // Commented out because of multiple false warnings on Behaviour types.
            return;
        }

        if (_genericTypeDrawerType == null)
            return;

        // Create instances of each (including the arguments).
        try
        {
            _genericTypeDrawerInstance = (PropertyDrawer)Activator.CreateInstance(_genericTypeDrawerType);
        }
        catch (Exception e)
        {
            LogWarning("no constructor available in " + _genericType + " : " + e, property);
            return;
        }

        // Reassign the attribute field in the drawer so it can access the argument values.
        try
        {
            _genericTypeDrawerType.GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(_genericTypeDrawerInstance, fieldInfo);
        }
        catch (Exception)
        {
            // LogWarning("Unable to assign attribute to " + _genericTypeDrawerInstance.GetType() + " : " + e, property);
        }
    }

    #endregion Class Methods
}

#endif