namespace BubbleApp.Data.Mongo
{
    public class MongoSettings
    {
        public string ConnectionString { get; set; } = default!;
        public string Database        { get; set; } = default!;

        // Collection names (override in appsettings.json if needed)
        public string AdminsCollection      { get; set; } = "admins";
        public string WorkspacesCollection  { get; set; } = "workspaces";
        public string NotesCollection       { get; set; } = "notes";
    }
}