﻿using System;
using System.Collections.Generic;

namespace AGS.Editor
{
    public interface IProperty
    {
        string Name { get; }
        object Object { get; }
        void SetValue(object value);
        object GetValue();
        string ValueString { get; }
        Type PropertyType { get; }
        TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute;
        List<IProperty> Children { get; }
        bool IsReadonly { get; }
        void Refresh();
    }
}
