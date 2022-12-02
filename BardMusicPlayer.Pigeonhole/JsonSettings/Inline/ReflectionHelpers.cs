#region

using System;
using System.Reflection;

#endregion

namespace BardMusicPlayer.Pigeonhole.JsonSettings.Inline
{
    internal static class ReflectionHelpers
    {
        public static bool IsValueType(Type targetType)
        {
            if (targetType == null) throw new NullReferenceException("Must supply the targetType parameter");
            return targetType.GetTypeInfo().IsValueType;
        }
    }
}