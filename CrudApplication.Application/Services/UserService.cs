using CrudApplication.Core.Abstractions;
using CrudApplication.Core.Models;
using Microsoft.Extensions.Logging;


namespace CrudApplication.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;


        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }


        public async Task<(Guid, string?)> CreateUser(User user)
        {
            var notValidFields = new List<string>();
            if (!Utils.IsValidLogin(user.Login))
            {
                notValidFields.Add($"{nameof(user.Login)}");
            }
            if (!Utils.IsValidPassword(user.Password))
            {
                notValidFields.Add($"{nameof(user.Password)}");
            }
            if (!Utils.IsValidName(user.Name))
            {
                notValidFields.Add($"{nameof(user.Name)}");
            }
            if (user.Gender != 0 && user.Gender != 1 && user.Gender != 2)
            {
                notValidFields.Add($"{nameof(user.Gender)}");
            }
            if (user.Birthday != null)
            {
                if (user.Birthday == new DateTime())
                {
                    notValidFields.Add($"{nameof(user.Birthday)}");
                }
            }
            if (string.IsNullOrEmpty(user.CreatedBy))
            {
                notValidFields.Add($"{nameof(user.CreatedBy)}");
            }
            if (user.CreatedOn == new DateTime())
            {
                notValidFields.Add($"{nameof(user.CreatedOn)}");
            }
            if (notValidFields.Count == 0)
            {
                var login = user.Login;
                var password = Utils.GetMD5Hash(user.Password);
                var name = user.Name;
                var gender = user.Gender;
                var birthday = user.Birthday;
                var admin = user.Admin;
                var createdBy = user.CreatedBy;
                var createdOn = user.CreatedOn;
                return (await _userRepository.Create(login, password, name, gender, birthday, admin, createdBy, createdOn), null);
            }
            var error = $"Not valid fields for creating user: {string.Join(", ", notValidFields)}";
            _logger.LogError(error);
            return (Guid.Empty, error);
        }

        public async Task<string?> UpdateUserNameOrGenderOrBirthday(User user)
        {
            var notValidFields = new List<string>();
            if (user.Guid == Guid.Empty)
            {
                notValidFields.Add($"{nameof(user.Guid)}");
            }
            if (user.Name != null)
            {
                if (!Utils.IsValidName(user.Name))
                {
                    notValidFields.Add($"{nameof(user.Name)}");
                }
            }
            if (user.Gender != -1 && user.Gender != 0 && user.Gender != 1 && user.Gender != 2)
            {
                notValidFields.Add($"{nameof(user.Gender)}");
            }
            if (user.Birthday != null)
            {
                if (user.Birthday == new DateTime())
                {
                    notValidFields.Add($"{nameof(user.Birthday)}");
                }
            }
            if (string.IsNullOrEmpty(user.ModifiedBy))
            {
                notValidFields.Add($"{nameof(user.ModifiedBy)}");
            }
            if (user.ModifiedOn == new DateTime())
            {
                notValidFields.Add($"{nameof(user.ModifiedOn)}");
            }
            if (notValidFields.Count == 0)
            {
                await _userRepository.UpdateNameOrGenderOrBirthday(user.Guid, user.Name, user.Gender, user.Birthday, user.ModifiedBy, user.ModifiedOn);
                return null;
            }
            var error = $"Not valid fields for updating user: {string.Join(", ", notValidFields)}";
            _logger.LogError(error);
            return error;
        }

        public async Task<string?> UpdateUserPassword(User user)
        {
            var notValidFields = new List<string>();
            if (user.Guid == Guid.Empty)
            {
                notValidFields.Add($"{nameof(user.Guid)}");
            }
            if (!Utils.IsValidPassword(user.Password))
            {
                notValidFields.Add($"{nameof(user.Password)}");
            }
            if (string.IsNullOrEmpty(user.ModifiedBy))
            {
                notValidFields.Add($"{nameof(user.ModifiedBy)}");
            }
            if (user.ModifiedOn == new DateTime())
            {
                notValidFields.Add($"{nameof(user.ModifiedOn)}");
            }
            if (notValidFields.Count == 0)
            {
                await _userRepository.UpdatePassword(user.Guid, user.Password, user.ModifiedBy, user.ModifiedOn);
                return null;
            }
            var error = $"Not valid fields for updating user: {string.Join(", ", notValidFields)}";
            _logger.LogError(error);
            return error;
        }

        public async Task<string?> UpdateUserLogin(User user)
        {
            var notValidFields = new List<string>();
            if (user.Guid == Guid.Empty)
            {
                notValidFields.Add($"{nameof(user.Guid)}");
            }
            if (!Utils.IsValidLogin(user.Login))
            {
                notValidFields.Add($"{nameof(user.Login)}");
            }
            if (string.IsNullOrEmpty(user.ModifiedBy))
            {
                notValidFields.Add($"{nameof(user.ModifiedBy)}");
            }
            if (user.ModifiedOn == new DateTime())
            {
                notValidFields.Add($"{nameof(user.ModifiedOn)}");
            }
            if (notValidFields.Count == 0)
            {
                await _userRepository.UpdateLogin(user.Guid, user.Login, user.ModifiedBy, user.ModifiedOn);
                return null;
            }
            var error = $"Not valid fields for updating user: {string.Join(", ", notValidFields)}";
            _logger.LogError(error);
            return error;
        }

        public async Task<(List<User>, string?)> GetAllUsersActive()
        {
            return (await _userRepository.GetAllActive(), null);
        }

        public async Task<(User, string?)> GetOneUser(User user)
        {
            if (!string.IsNullOrEmpty(user.Login))
            {
                return (await _userRepository.GetOneByLogin(user.Login), null);
            }
            if (user.Guid != Guid.Empty)
            {
                return (await _userRepository.GetOneByGuid(user.Guid), null);
            }
            var error = $"Not valid fields for getting user: {nameof(user.Login)}, {nameof(user.Guid)}";
            _logger.LogError(error);
            return (user, error);
        }

        public async Task<(List<User>, string?)> GetAllUsersOlderThat(User user)
        {
            if (user.Birthday != null)
            {
                return (await _userRepository.GetAllOlderThat((DateTime)user.Birthday), null);
            }
            var error = $"Not valid fields for getting user: {nameof(user.Birthday)}";
            _logger.LogError(error);
            return (new List<User>(), error);
        }

        public async Task<string?> DeleteUser(User user, bool isSoft)
        {
            var notValidFields = new List<string>();
            if (user.Guid == Guid.Empty)
            {
                notValidFields.Add($"{nameof(user.Guid)}");
            }
            if (isSoft)
            {
                if (string.IsNullOrEmpty(user.RevokedBy))
                {
                    notValidFields.Add($"{nameof(user.RevokedBy)}");
                }
                if (user.RevokedOn == new DateTime())
                {
                    notValidFields.Add($"{nameof(user.RevokedOn)}");
                }
            }
            if (notValidFields.Count == 0)
            {
                await _userRepository.Delete(user.Guid, isSoft, user.RevokedBy, user.RevokedOn);
                return null;
            }
            var error = $"Not valid fields for deleting user:  {string.Join(", ", notValidFields)}";
            _logger.LogError(error);
            return error;
        }

        public async Task<string?> RecoveryUser(User user)
        {
            var notValidFields = new List<string>();
            if (user.Guid == Guid.Empty)
            {
                notValidFields.Add($"{nameof(user.Guid)}");
            }
            if (string.IsNullOrEmpty(user.ModifiedBy))
            {
                notValidFields.Add($"{nameof(user.ModifiedBy)}");
            }
            if (user.ModifiedOn == new DateTime())
            {
                notValidFields.Add($"{nameof(user.ModifiedOn)}");
            }
            if (notValidFields.Count == 0)
            {
                await _userRepository.Recovery(user.Guid, user.ModifiedBy, user.ModifiedOn);
                return null;
            }
            var error = $"Not valid fields for recovering user:  {string.Join(", ", notValidFields)}";
            _logger.LogError(error);
            return error;
        }

        public async Task<(bool, string?)> ExistsUser(User user)
        {
            if (!string.IsNullOrEmpty(user.Login) && !string.IsNullOrEmpty(user.Password))
            {
                return (await _userRepository.Exists(user.Login, Utils.GetMD5Hash(user.Password)), null);
            }
            if (!string.IsNullOrEmpty(user.Login))
            {
                return (await _userRepository.Exists(user.Login), null);
            }
            if (user.Guid != Guid.Empty)
            {
                return (await _userRepository.Exists(user.Guid), null);
            }
            var error = $"Not valid fields for checking user: {nameof(user.Login)}, {nameof(user.Password)}, {nameof(user.Guid)}";
            _logger.LogError(error);
            return (false, error);
        }

        public async Task<(bool, string?)> IsUniqueUserLogin(User user)
        {
            if (!string.IsNullOrEmpty(user.Login))
            {
                return (!await _userRepository.Exists(user.Login), null);
            }
            var error = $"Not valid fields for checking user: {nameof(user.Login)}";
            _logger.LogError(error);
            return (false, error);
        }
    }
}