﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AGS.API;
using AGS.Engine;

namespace AGS.Editor
{
    public class ObjectTypeDescriptor : ITypeDescriptor
    {
        private readonly object _obj;
        private readonly Dictionary<InspectorCategory, List<IProperty>> _props;

        public ObjectTypeDescriptor(object obj)
        {
            _obj = obj;
            _props = new Dictionary<InspectorCategory, List<IProperty>>();
        }

        public Dictionary<InspectorCategory, List<IProperty>> GetProperties()
        {
            InspectorCategory cat = new InspectorCategory("General");
            AddProperties(cat, _obj, _props);
            return _props;
        }

        public static void AddProperties(InspectorCategory defaultCategory, object obj, Dictionary<InspectorCategory, List<IProperty>> properties)
        {
            var props = getProperties(obj.GetType());
            foreach (var prop in props)
            {
                var cat = defaultCategory;
                InspectorProperty property = AddProperty(obj, prop, ref cat);
                if (property == null) continue;
                properties.GetOrAdd(cat, () => new List<IProperty>(props.Length)).Add(property);
            }
        }

        public static InspectorProperty AddProperty(object obj, PropertyInfo prop, ref InspectorCategory cat)
        {
            var attr = prop.GetCustomAttribute<PropertyAttribute>();
            if (attr == null && prop.PropertyType.FullName.StartsWith("AGS.API.IEvent", StringComparison.Ordinal)) return null; //filtering all events from the inspector by default
            if (attr == null && prop.PropertyType.FullName.StartsWith("AGS.API.IBlockingEvent", StringComparison.Ordinal)) return null; //filtering all events from the inspector by default
            string name = prop.Name;
            if (attr != null)
            {
                if (!attr.Browsable) return null;
                if (attr.Category != null) cat = new InspectorCategory(attr.Category, attr.CategoryZ, attr.CategoryExpand);
                if (attr.DisplayName != null) name = attr.DisplayName;
            }
            InspectorProperty property = new InspectorProperty(obj, name, prop);
            RefreshChildrenProperties(property);
            return property;
        }

        public static void RefreshChildrenProperties(IProperty property)
        {
            property.Children.Clear();
            var val = property.GetValue();
            if (val == null) return;
            var objType = val.GetType();
            if (objType.GetTypeInfo().GetCustomAttribute<PropertyFolderAttribute>(true) != null)
            {
                var props = getProperties(objType);
                foreach (var childProp in props)
                {
                    InspectorCategory dummyCat = null;
                    var childProperty = AddProperty(val, childProp, ref dummyCat);
                    if (childProperty == null) continue;
                    property.Children.Add(childProperty);
                }
            }
        }

        private static PropertyInfo[] getProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }
    }
}