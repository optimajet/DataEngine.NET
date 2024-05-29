namespace OptimaJet.DataEngine;

public class OptionsRestorer : IDisposable
{
    internal OptionsRestorer(IOptions options, Action<IOptions> setOptions)
    {
        _options = options;
        _setOptions = setOptions;
    }

    private readonly IOptions _options;
    private Action<IOptions>? _setOptions;

    #region IDisposable implementation

    public void Dispose()
    {
        DisposeInternal();
        GC.SuppressFinalize(this);
    }

    private void DisposeInternal()
    {
        if (_setOptions != null)
        {
            _setOptions(_options);
        }

        _setOptions = null;
    }

    #endregion
}