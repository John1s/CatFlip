using Catflip.Api.DataAccess;
using Catflip.Api.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Catflip.Api.Services
{
    public interface IUserRepository
    {
        public Task CreateUser(string username, string password);
        public Task<UserEntity> ValidatedUser(string username, string password);
        public Task DeleteUser(string username);
        public Task ChangePassword(string username, string oldPassword, string newPassword);
    }

    class UserRepository: IUserRepository
    {
        private static RNGCryptoServiceProvider rngCrypto = new RNGCryptoServiceProvider();
        CatflipDbContext Context { get; }
        public UserRepository(CatflipDbContext context)
        {
            Context = context;
        }

        string HashedPassword(string password, string salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password + salt);
            return Convert.ToBase64String(MD5.HashData(passwordBytes));
        }

        public async Task CreateUser(string username, string password)
        {            
            if(Context.Users.Any(u => u.Username == username))
            {
                throw new DuplicateUsernameException(username);
            }
            var bytes = new byte[16];
            rngCrypto.GetBytes(bytes);
            var salt = Convert.ToBase64String(bytes);
            var hashedPassword = HashedPassword(password, salt);

            Context.Add(new UserEntity
            {
                Username = username,
                HashedPassword = hashedPassword,
                Salt = salt
            });
            await Context.SaveChangesAsync();
        }

        public async Task DeleteUser(string username)
        {
            var user = await Context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                Context.Users.Remove(user);
                await Context.SaveChangesAsync();
            }
        }

        public async Task<UserEntity> ValidatedUser(string username, string password)
        {
            var user = await Context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                throw new InvalidLoginException();
            }
            var hashedPassword = HashedPassword(password, user.Salt);
            if (hashedPassword != user.HashedPassword)
            {
                throw new InvalidLoginException();
            }
            return user;
        }

        public async Task ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user = await ValidatedUser(username, oldPassword);

            var newPasswordHash = HashedPassword(newPassword, user.Salt);
            user.HashedPassword = newPasswordHash;
            await Context.SaveChangesAsync();
        }
    }
}
