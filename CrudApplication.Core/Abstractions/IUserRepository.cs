using CrudApplication.Core.Models;


namespace CrudApplication.Core.Abstractions
{
    public interface IUserRepository
    {
        Task<Guid> Create(string login, string password, string name, int gender, DateTime? birthday, bool admin, string createdBy, DateTime createdOn);
        Task Delete(Guid guid, bool isSoft, string? revokedBy, DateTime? revokedOn);
        Task<bool> Exists(Guid guid);
        Task<bool> Exists(string login);
        Task<bool> Exists(string login, string password);
        Task<List<User>> GetAllActive();
        Task<List<User>> GetAllOlderThat(DateTime date);
        Task<User> GetOneByLogin(string login);
        Task<User> GetOneByGuid(Guid guid);
        Task Recovery(Guid guid, string modifiedBy, DateTime modifiedOn);
        Task UpdateLogin(Guid guid, string login, string modifiedBy, DateTime modifiedOn);
        Task UpdateNameOrGenderOrBirthday(Guid guid, string name, int gender, DateTime? birthday, string modifiedBy, DateTime modifiedOn);
        Task UpdatePassword(Guid guid, string password, string modifiedBy, DateTime modifiedOn);
    }
}