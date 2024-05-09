namespace OptimaJet.DataEngine;

public interface IProviderBuilder
{
    ProviderKey GetKey();
    IProvider Build();
}