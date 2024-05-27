using CrudApplication.Core.Models;


namespace CrudApplication.Core.Abstractions
{
    public interface IUserService
    {
        Task<(Guid, string?)> CreateUser(User user);
        Task<string?> DeleteUser(User user, bool isSoft);
        Task<(bool, string?)> ExistsUser(User user);
        Task<(List<User>, string?)> GetAllUsersActive();
        Task<(List<User>, string?)> GetAllUsersOlderThat(User user);
        Task<(User, string?)> GetOneUser(User user);
        Task<(bool, string?)> IsUniqueUserLogin(User user);
        Task<string?> RecoveryUser(User user);
        Task<string?> UpdateUserLogin(User user);
        Task<string?> UpdateUserNameOrGenderOrBirthday(User user);
        Task<string?> UpdateUserPassword(User user);
    }
}