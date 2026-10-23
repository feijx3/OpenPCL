using System;
using System.Runtime.CompilerServices;

namespace MeloongCore
{
    public static class GeneralUtils
    {
        [NullableContext(1)]
        public static bool IsGenericInstanceOf([Nullable(2)] object instance, Type genericTypeDefinition)
        {
            if (instance == null)
            {
                return false;
            }
            for (Type type = instance.GetType(); type != null; type = type.BaseType)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
