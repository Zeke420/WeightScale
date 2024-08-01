using System;
using System.Windows.Markup;

namespace WeightScale.Presentation.Converters
{
    public class EnumBindingSourceExtension : MarkupExtension
    {
        public EnumBindingSourceExtension(Type enumType)
        {
            if (enumType is null || !enumType.IsEnum)
            {
                throw new Exception("EnumType must not be null and of type Enum");
            }

            EnumType = enumType;
        }

        private Type EnumType { get; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }
    }
}