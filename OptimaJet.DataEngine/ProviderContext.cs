using OptimaJet.DataEngine.Exceptions;
using OptimaJet.DataEngine.Helpers;

namespace OptimaJet.DataEngine;

public class ProviderContext : IAsyncDisposable, IDisposable
{
    private ProviderContext(IProviderBuilder providerBuilder)
    {
        var providerKey = providerBuilder.GetKey();

        if (!_providers.ContainsKey(providerKey))
        {
            _providers.Add(providerKey, providerBuilder.Build());
        }

        _keys.Push(providerKey);
    }

    public static ProviderContext Use(IProviderBuilder providerBuilder)
    {
        return new ProviderContext(providerBuilder);
    }

    public static IProvider Current => CurrentOrNull ?? throw new NoAvailableProviderException();

    public static IProvider? CurrentOrNull => _keys.TryPeek(out var key) ? _providers[key] : null;

    private static readonly AsyncLocalDictionary<ProviderKey, IProvider> _providers = new();
    private static readonly AsyncLocalStack<ProviderKey> _keys = new();

    #region IDisposable implementation

    public void Dispose()
    {
        DisposeAsync().AsTask().Wait();
    }
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncInternal();
        GC.SuppressFinalize(this);
    }

    private async ValueTask DisposeAsyncInternal()
    {
        if (_disposed) return;

        if (_keys.TryPop(out var key))
        {
            if (!_keys.Contains(key))
            {
                await _providers[key].DisposeAsync();
                _providers.Remove(key);
            }
        }

        _disposed = true;
    }
    private bool _disposed;

    #endregion
}