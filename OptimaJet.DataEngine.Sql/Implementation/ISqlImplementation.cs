using System.Data.Common;
using OptimaJet.DataEngine.Metadata;
using SqlKata.Compilers;

namespace OptimaJet.DataEngine.Sql.Implementation;

internal interface ISqlImplementation
{
    string Name { get; }
    Compiler Compiler { get; }
    Dialect Dialect { get; }
    DbConnection CreateConnection(string connectionString);
    void ConfigureMetadata(EntityMetadata metadata);
}