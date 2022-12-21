using LiteDB;
using LiteDB.Engine;

namespace Strem.Plugins.Analytics.Services.Database;

public class AnalyticsDatabase : LiteDatabase, IAnalyticsDatabase
{
    public AnalyticsDatabase(string connectionString, BsonMapper mapper = null) : base(connectionString, mapper)
    {}

    public AnalyticsDatabase(ConnectionString connectionString, BsonMapper mapper = null) : base(connectionString, mapper)
    {}

    public AnalyticsDatabase(Stream stream, BsonMapper mapper = null, Stream logStream = null) : base(stream, mapper, logStream)
    {}

    public AnalyticsDatabase(ILiteEngine engine, BsonMapper mapper = null, bool disposeOnClose = true) : base(engine, mapper, disposeOnClose)
    {}
}