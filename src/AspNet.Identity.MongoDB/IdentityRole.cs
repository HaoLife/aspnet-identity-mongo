using global::MongoDB.Bson;
using global::MongoDB.Bson.Serialization.Attributes;
using System;


namespace Microsoft.AspNetCore.Identity.MongoDB
{
    public class IdentityRole : IdentityRole<string>
    {
        public IdentityRole()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

    }

    public class IdentityRole<TKey>
    {
        public IdentityRole()
        {

        }
        public IdentityRole(TKey key, string roleName)
        {
            Id = key;
            Name = roleName;
        }

        public TKey Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public override string ToString() => Name;
    }
}