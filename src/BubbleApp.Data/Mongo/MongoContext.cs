using BubbleApp.Common.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BubbleApp.Data.Mongo
{
    public class MongoContext
    {
        public IMongoDatabase Db { get; }
        public IMongoCollection<Admin>     Admins     { get; }
        public IMongoCollection<Workspace> Workspaces { get; }
        public IMongoCollection<Note>      Notes      { get; }

        public MongoContext(IOptions<MongoSettings> options)
        {
            var s = options.Value;

            var client = new MongoClient(s.ConnectionString);
            Db = client.GetDatabase(s.Database);

            Admins     = Db.GetCollection<Admin>(s.AdminsCollection);
            Workspaces = Db.GetCollection<Workspace>(s.WorkspacesCollection);
            Notes      = Db.GetCollection<Note>(s.NotesCollection);

            // Indexes
            Workspaces.Indexes.CreateOne(
                new CreateIndexModel<Workspace>(
                    Builders<Workspace>.IndexKeys.Ascending(w => w.Slug),
                    new CreateIndexOptions { Unique = true }));

            var noteKeys = Builders<Note>.IndexKeys
                .Ascending(n => n.Workspace)
                .Ascending(n => n.UserId)
                .Descending(n => n.CreatedAt);
            Notes.Indexes.CreateOne(new CreateIndexModel<Note>(noteKeys));
        }
    }
}