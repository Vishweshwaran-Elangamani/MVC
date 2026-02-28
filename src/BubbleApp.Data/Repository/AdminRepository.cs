using BubbleApp.Common.Entities;
using BubbleApp.Data.IRepository;
using BubbleApp.Data.Mongo;
using MongoDB.Driver;
using System.Threading;

namespace BubbleApp.Data.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly MongoContext _ctx;
        public AdminRepository(MongoContext ctx) => _ctx = ctx;

        public Task<Admin?> GetByEmailAsync(string email, CancellationToken ct = default)
            => _ctx.Admins.Find(a => a.Email == email).FirstOrDefaultAsync(ct)!;

        public async Task<Admin> CreateAsync(Admin admin, CancellationToken ct = default)
        {
            await _ctx.Admins.InsertOneAsync(admin, cancellationToken: ct);
            return admin;
        }
    }
}