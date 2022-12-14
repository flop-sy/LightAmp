#region

using System;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.Util
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum, Inherited = false)]
    internal sealed class JsonSerializableAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal sealed class JsonImmutableAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class JsonNameAttribute : Attribute
    {
        public JsonNameAttribute(params string[] names)
        {
            Names = names;
        }

        public string[] Names { get; }
    }
}