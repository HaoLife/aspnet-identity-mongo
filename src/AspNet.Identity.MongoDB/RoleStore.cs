﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using global::MongoDB.Driver;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
// I'm using async methods to leverage implicit Task wrapping of results from expression bodied functions.

namespace Microsoft.AspNetCore.Identity.MongoDB
{

    /// <summary>
    ///     Note: Deleting and updating do not modify the roles stored on a user document. If you desire this dynamic
    ///     capability, override the appropriate operations on RoleStore as desired for your application. For example you could
    ///     perform a document modification on the users collection before a delete or a rename.
    ///     When passing a cancellation token, it will only be used if the operation requires a database interaction.
    /// </summary>
    /// <typeparam name="TRole">Needs to extend the provided IdentityRole type.</typeparam>
    public class RoleStore<TRole, Tkey> : IQueryableRoleStore<TRole>
        // todo IRoleClaimStore<TRole>
        where TRole : IdentityRole<Tkey>
    {
        private readonly IMongoCollection<TRole> _Roles;

        public RoleStore(IMongoCollection<TRole> roles)
        {
            _Roles = roles;
        }

        public virtual void Dispose()
        {
            // no need to dispose of anything, mongodb handles connection pooling automatically
        }

        public virtual async Task<IdentityResult> CreateAsync(TRole role, CancellationToken token)
        {
            await _Roles.InsertOneAsync(role, cancellationToken: token);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken token)
        {
            var result = await _Roles.ReplaceOneAsync(r => r.Id.Equals(To(role.Id)), role, cancellationToken: token);
            // todo low priority result based on replace result
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken token)
        {
            var result = await _Roles.DeleteOneAsync(r => r.Id.Equals(To(role.Id)), token);
            // todo low priority result based on delete result
            return IdentityResult.Success;
        }

        public virtual async Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
            => role.Id.ToString();

        public virtual async Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
            => role.Name;

        public virtual async Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
            => role.Name = roleName;

        // note: can't test as of yet through integration testing because the Identity framework doesn't use this method internally anywhere
        public virtual async Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
            => role.NormalizedName;

        public virtual async Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
            => role.NormalizedName = normalizedName;

        public virtual Task<TRole> FindByIdAsync(string roleId, CancellationToken token)
            => _Roles.Find(r => r.Id.Equals(To(roleId)))
                .FirstOrDefaultAsync(token);

        public virtual Task<TRole> FindByNameAsync(string normalizedName, CancellationToken token)
            => _Roles.Find(r => r.NormalizedName == normalizedName)
                .FirstOrDefaultAsync(token);

        public virtual IQueryable<TRole> Roles
            => _Roles.AsQueryable();


        public bool IsKey(object value)
        {
            if (value != null)
            {
                Type destinationType = typeof(Tkey);
                var sourceType = value.GetType();

                var destinationConverter = TypeDescriptor.GetConverter(destinationType);
                if (destinationConverter != null)
                    return destinationConverter.CanConvertFrom(sourceType);

                var sourceConverter = TypeDescriptor.GetConverter(sourceType);
                if (sourceConverter != null)
                    return sourceConverter.CanConvertTo(destinationType);

                if (destinationType.IsEnum)
                    return value is int;

            }
            return false;
        }



        public object To(object value)
        {
            if (value != null)
            {
                Type destinationType = typeof(Tkey);
                var sourceType = value.GetType();

                var destinationConverter = TypeDescriptor.GetConverter(destinationType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(value);

                var sourceConverter = TypeDescriptor.GetConverter(sourceType);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(value, destinationType);

                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int)value);

                if (!destinationType.IsInstanceOfType(value))
                    return Convert.ChangeType(value, destinationType);
            }
            return value;
        }

    }
}