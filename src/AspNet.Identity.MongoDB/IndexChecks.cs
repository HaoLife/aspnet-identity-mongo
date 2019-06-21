using global::MongoDB.Driver;


namespace Microsoft.AspNetCore.Identity.MongoDB
{
    public static class IndexChecks
    {
        public static void EnsureUniqueIndexOnNormalizedUserName<TUser, Tkey>(IMongoCollection<TUser> users)
            where TUser : IdentityUser<Tkey>
        {
            var userName = Builders<TUser>.IndexKeys.Ascending(t => t.NormalizedUserName);
            var unique = new CreateIndexOptions { Unique = true };
            users.Indexes.CreateOneAsync(userName, unique);
        }

        public static void EnsureUniqueIndexOnNormalizedRoleName<TRole, Tkey>(IMongoCollection<TRole> roles)
            where TRole : IdentityRole<Tkey>
        {
            var roleName = Builders<TRole>.IndexKeys.Ascending(t => t.NormalizedName);
            var unique = new CreateIndexOptions { Unique = true };
            roles.Indexes.CreateOneAsync(roleName, unique);
        }

        public static void EnsureUniqueIndexOnNormalizedEmail<TUser, Tkey>(IMongoCollection<TUser> users)
            where TUser : IdentityUser<Tkey>
        {
            var email = Builders<TUser>.IndexKeys.Ascending(t => t.NormalizedEmail);
            var unique = new CreateIndexOptions { Unique = true };
            users.Indexes.CreateOneAsync(email, unique);
        }

        /// <summary>
        ///     ASP.NET Core Identity now searches on normalized fields so these indexes are no longer required, replace with
        ///     normalized checks.
        /// </summary>
        public static class OptionalIndexChecks
        {
            public static void EnsureUniqueIndexOnUserName<TUser, Tkey>(IMongoCollection<TUser> users)
                where TUser : IdentityUser<Tkey>
            {
                var userName = Builders<TUser>.IndexKeys.Ascending(t => t.UserName);
                var unique = new CreateIndexOptions { Unique = true };
                users.Indexes.CreateOneAsync(userName, unique);
            }

            public static void EnsureUniqueIndexOnRoleName<TRole, Tkey>(IMongoCollection<TRole> roles)
                where TRole : IdentityRole<Tkey>
            {
                var roleName = Builders<TRole>.IndexKeys.Ascending(t => t.Name);
                var unique = new CreateIndexOptions { Unique = true };
                roles.Indexes.CreateOneAsync(roleName, unique);
            }

            public static void EnsureUniqueIndexOnEmail<TUser, Tkey>(IMongoCollection<TUser> users)
                where TUser : IdentityUser<Tkey>
            {
                var email = Builders<TUser>.IndexKeys.Ascending(t => t.Email);
                var unique = new CreateIndexOptions { Unique = true };
                users.Indexes.CreateOneAsync(email, unique);
            }
        }
    }
}