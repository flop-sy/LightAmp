#region

using System;

#endregion

namespace BardMusicPlayer.Siren.AlphaTab.Util;

internal sealed class Lazy<T>
{
    private readonly Func<T> _factory;
    private bool _created;
    private T _value;

    public Lazy(Func<T> factory)
    {
        _factory = factory;
    }

    public T Value
    {
        get
        {
            if (_created) return _value;

            _value = _factory();
            _created = true;

            return _value;
        }
    }
}