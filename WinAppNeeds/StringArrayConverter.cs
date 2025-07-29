using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace EasyWinFormLibrary.WinAppNeeds
{
    /// <summary>
    /// Custom type converter for string arrays in property grid
    /// </summary>
    public class StringArrayConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                return stringValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(s => s.Trim())
                                 .ToArray();
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is string[] stringArray)
            {
                return string.Join(", ", stringArray);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
