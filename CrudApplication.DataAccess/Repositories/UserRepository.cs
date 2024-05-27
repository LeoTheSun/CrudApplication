using CrudApplication.Core.Abstractions;
using CrudApplication.Core.Models;
using CrudApplication.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace CrudApplication.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;
        private readonly ILogger<UserRepository> _logger;


        public UserRepository(UserDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<Guid> Create(string login, string password, string name, int gender, DateTime? birthday, bool admin, string createdBy, DateTime createdOn)
        {
            var userEntity = new UserEntity()
            {
                Login = login,
                Password = password,
                Name = name,
                Gender = gender,
                Birthday = birthday,
                Admin = admin,
                CreatedOn = createdOn,
                CreatedBy = createdBy,
                ModifiedOn = createdOn,
                ModifiedBy = createdBy
            };

            await _context.Users.AddAsync(userEntity);
            _logger.LogInformation("SQL Query:");
            await _context.SaveChangesAsync();
            return userEntity.Guid;
        }

        public async Task UpdateNameOrGenderOrBirthday(Guid guid, string name, int gender, DateTime? birthday, string modifiedBy, DateTime modifiedOn)
        {
            if (!string.IsNullOrEmpty(name) && gender != -1 && birthday != null)
            {
                _logger.LogInformation("SQL Query:");
                await _context.Users
                    .Where(u => u.Guid == guid)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(u => u.Name, u => name)
                        .SetProperty(u => u.Gender, u => gender)
                        .SetProperty(u => u.Birthday, u => birthday)
                        .SetProperty(u => u.ModifiedBy, u => modifiedBy)
                        .SetProperty(u => u.ModifiedOn, u => modifiedOn)
                    );
                return;
            }
            if (!string.IsNullOrEmpty(name) && gender != -1)
            {
                _logger.LogInformation("SQL Query:");
                await _context.Users
                    .Where(u => u.Guid == guid)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(u => u.Name, u => name)
                        .SetProperty(u => u.Gender, u => gender)
                        .SetProperty(u => u.ModifiedBy, u => modifiedBy)
                        .SetProperty(u => u.ModifiedOn, u => modifiedOn)
                    );
                return;
            }
            if (!string.IsNullOrEmpty(name) && birthday != null)
            {
                _logger.LogInformation("SQL Query:");
                await _context.Users
                    .Where(u => u.Guid == guid)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(u => u.Gender, u => gender)
                        .SetProperty(u => u.Birthday, u => birthday)
                        .SetProperty(u => u.ModifiedBy, u => modifiedBy)
                        .SetProperty(u => u.ModifiedOn, u => modifiedOn)
                    );
                return;
            }
            if (gender != -1 && birthday != null)
            {
                _logger.LogInformation("SQL Query:");
                await _context.Users
                    .Where(u => u.Guid == guid)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(u => u.Gender, u => gender)
                        .SetProperty(u => u.Birthday, u => birthday)
                        .SetProperty(u => u.ModifiedBy, u => modifiedBy)
                        .SetProperty(u => u.ModifiedOn, u => modifiedOn)
                    );
                return;
            }
            if (!string.IsNullOrEmpty(name))
            {
                _logger.LogInformation("SQL Query:");
                await _context.Users
                    .Where(u => u.Guid == guid)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(u => u.Name, u => name)
                        .SetProperty(u => u.ModifiedBy, u => modifiedBy)
                        .SetProperty(u => u.ModifiedOn, u => modifiedOn)
                    );
                return;
            }
            if (gender != -1)
            {
                _logger.LogInformation("SQL Query:");
                await _context.Users
                    .Where(u => u.Guid == guid)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(u => u.Gender, u => gender)
                        .SetProperty(u => u.ModifiedBy, u => modifiedBy)
                        .SetProperty(u => u.ModifiedOn, u => modifiedOn)
                    );
                return;
            }
            if (birthday != null)
            {
                _logger.LogInformation("SQL Query:");
                await _context.Users
                    .Where(u => u.Guid == guid)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(u => u.Birthday, u => birthday)
                        .SetProperty(u => u.ModifiedBy, u => modifiedBy)
                        .SetProperty(u => u.ModifiedOn, u => modifiedOn)
                    );
                return;
            }
        }

        public async Task UpdatePassword(Guid guid, string password, string modifiedBy, DateTime modifiedOn)
        {
            _logger.LogInformation("SQL Query:");
            await _context.Users
                   .Where(u => u.Guid == guid)
                   .ExecuteUpdateAsync(s => s
                       .SetProperty(u => u.Password, u => password)
                       .SetProperty(u => u.ModifiedBy, u => modifiedBy)
                        .SetProperty(u => u.ModifiedOn, u => modifiedOn)
                   );
        }

        public async Task UpdateLogin(Guid guid, string login, string modifiedBy, DateTime modifiedOn)
        {
            _logger.LogInformation("SQL Query:");
            await _context.Users
                   .Where(u => u.Guid == guid)
                   .ExecuteUpdateAsync(s => s
                       .SetProperty(u => u.Login, u => login)
                       .SetProperty(u => u.ModifiedBy, u => modifiedBy)
                       .SetProperty(u => u.ModifiedOn, u => modifiedOn)
                   );
        }

        public async Task<List<User>> GetAllActive()
        {
            _logger.LogInformation("SQL Query:");
            var userEntities = await _context.Users
                .AsNoTracking()
                .Where(u => u.RevokedOn == null)
                .OrderBy(u => u.CreatedOn)
                .ToListAsync();
            var users = userEntities
                .Select(u => new User(
                    u.Guid,
                    u.Login,
                    string.Empty,
                    u.Name,
                    u.Gender,
                    u.Birthday,
                    u.Admin,
                    u.CreatedOn,
                    u.CreatedBy,
                    u.ModifiedOn,
                    u.ModifiedBy,
                    u.RevokedOn,
                    u.RevokedBy
                )).ToList();
            return users;
        }

        public async Task<User> GetOneByLogin(string login)
        {
            _logger.LogInformation("SQL Query:");
            var userEntity = await _context.Users
                .AsNoTracking()
                .FirstAsync(u => u.Login == login);
            var user = new User(
                userEntity.Guid,
                userEntity.Login,
                string.Empty,
                userEntity.Name,
                userEntity.Gender,
                userEntity.Birthday,
                userEntity.Admin,
                userEntity.CreatedOn,
                userEntity.CreatedBy,
                userEntity.ModifiedOn,
                userEntity.ModifiedBy,
                userEntity.RevokedOn,
                userEntity.RevokedBy
            );
            return user;
        }

        public async Task<List<User>> GetAllOlderThat(DateTime date)
        {
            _logger.LogInformation("SQL Query:");
            var userEntities = await _context.Users
                .AsNoTracking()
                .Where(u => u.Birthday < date)
                .ToListAsync();
            var users = userEntities
                .Select(u => new User(
                    u.Guid,
                    u.Login,
                    string.Empty,
                    u.Name,
                    u.Gender,
                    u.Birthday,
                    u.Admin,
                    u.CreatedOn,
                    u.CreatedBy,
                    u.ModifiedOn,
                    u.ModifiedBy,
                    u.RevokedOn,
                    u.RevokedBy
                )).ToList();
            return users;
        }

        public async Task Delete(Guid guid, bool isSoft, string? revokedBy, DateTime? revokedOn)
        {
            if (isSoft)
            {
                _logger.LogInformation("SQL Query:");
                await _context.Users
                   .Where(u => u.Guid == guid)
                   .ExecuteUpdateAsync(s => s
                       .SetProperty(u => u.RevokedBy, u => revokedBy)
                       .SetProperty(u => u.RevokedOn, u => revokedOn)
                       .SetProperty(u => u.ModifiedBy, u => revokedBy)
                       .SetProperty(u => u.ModifiedOn, u => revokedOn)
                   );
                return;
            }
            _logger.LogInformation("SQL Query:");
            await _context.Users
                .Where(u => u.Guid == guid)
                .ExecuteDeleteAsync();
        }

        public async Task Recovery(Guid guid, string modifiedBy, DateTime modifiedOn)
        {
            _logger.LogInformation("SQL Query:");
            await _context.Users
                   .Where(u => u.Guid == guid)
                   .ExecuteUpdateAsync(s => s
                       .SetProperty(u => u.RevokedBy, u => null)
                       .SetProperty(u => u.RevokedOn, u => null)
                       .SetProperty(u => u.ModifiedBy, u => modifiedBy)
                       .SetProperty(u => u.ModifiedOn, u => modifiedOn)
                   );
        }

        public async Task<bool> Exists(Guid guid)
        {
            _logger.LogInformation("SQL Query:");
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Guid == guid);
        }

        public async Task<bool> Exists(string login)
        {
            _logger.LogInformation("SQL Query:");
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Login == login);
        }

        public async Task<bool> Exists(string login, string password)
        {
            _logger.LogInformation("SQL Query:");
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Login == login && u.Password == password);
        }

        public async Task<User> GetOneByGuid(Guid guid)
        {
            _logger.LogInformation("SQL Query:");
            var userEntity = await _context.Users
                .AsNoTracking()
                .FirstAsync(u => u.Guid == guid);
            var user = new User(
                userEntity.Guid,
                userEntity.Login,
                string.Empty,
                userEntity.Name,
                userEntity.Gender,
                userEntity.Birthday,
                userEntity.Admin,
                userEntity.CreatedOn,
                userEntity.CreatedBy,
                userEntity.ModifiedOn,
                userEntity.ModifiedBy,
                userEntity.RevokedOn,
                userEntity.RevokedBy
            );
            return user;
        }
    }
}