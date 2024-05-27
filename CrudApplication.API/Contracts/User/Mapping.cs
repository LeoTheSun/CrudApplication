using CrudApplication.API.Contracts.User.In;
using CrudApplication.API.Contracts.User.Out;


namespace CrudApplication.API.Contracts.User
{
    public static class Mapping
    {
        private static Core.Models.User CreateUser(
            Guid guid = new Guid(),
            string login = "",
            string password = "",
            string name = "",
            int gender = -1,
            DateTime? birthday = null,
            bool admin = false,
            DateTime createdOn = new DateTime(),
            string createdBy = "",
            DateTime modifiedOn = new DateTime(),
            string modifiedBy = "",
            DateTime? revokedOn = null,
            string? revokedBy = null
        )
        {
            var user = new Core.Models.User(
                guid,
                login,
                password,
                name,
                gender,
                birthday,
                admin,
                createdOn,
                createdBy,
                modifiedOn,
                modifiedBy,
                revokedOn,
                revokedBy
            );
            return user;
        }


        public static Core.Models.User UserFromLoginAndPassword(string login, string password)
        {
            var user = CreateUser(
                login: login,
                password: password
            );
            return user;
        }

        public static Core.Models.User UserFromCreateUserData(CreateUserData createUserData, string createdBy, DateTime createdOn)
        {
            var user = CreateUser(
                login: createUserData.Login,
                password: createUserData.Password,
                name: createUserData.Name,
                gender: createUserData.Gender,
                birthday: createUserData.Birthday,
                admin: createUserData.Admin,
                createdOn: createdOn,
                createdBy: createdBy
            );
            return user;
        }

        public static Core.Models.User UserFromUpdateUserData(Guid guid, UpdateUserData updateUserData, string modifiedBy, DateTime modifiedOn)
        {
            if (updateUserData.Gender == null)
            {
                updateUserData.Gender = -1;
            }
            var user = CreateUser(
                guid: guid,
                login: updateUserData.Login,
                password: updateUserData.Password,
                name: updateUserData.Name,
                gender: updateUserData.Gender.Value,
                birthday: updateUserData.Birthday,
                modifiedOn: modifiedOn,
                modifiedBy: modifiedBy
            );
            return user;
        }

        public static Core.Models.User UserFromGuid(Guid guid)
        {
            var user = CreateUser(
                guid: guid
            );
            return user;
        }

        public static Core.Models.User UserFromLogin(string login)
        {
            var user = CreateUser(
                login: login
            );
            return user;
        }

        public static Core.Models.User UserFromDateTime(DateTime birthday)
        {
            var user = CreateUser(
                birthday: birthday
            );
            return user;
        }

        public static Core.Models.User UserFromGuid(Guid guid, string modifiedBy, DateTime modifiedOn)
        {
            var user = CreateUser(
                guid: guid,
                modifiedOn: modifiedOn,
                modifiedBy: modifiedBy,
                revokedOn: modifiedOn,
                revokedBy: modifiedBy
            );
            return user;
        }


        public static FullUserData FullUserDataFromUser(Core.Models.User user)
        {
            var fullUserData = new FullUserData()
            {
                Guid = user.Guid,
                Login = user.Login,
                Name = user.Name,
                Gender = user.Gender,
                Birthday = user.Birthday,
                Admin = user.Admin,
                CreatedOn = user.CreatedOn,
                CreatedBy = user.CreatedBy,
                ModifiedOn = user.ModifiedOn,
                ModifiedBy = user.ModifiedBy,
                RevokedOn = user.RevokedOn,
                RevokedBy = user.RevokedBy
            };
            return fullUserData;
        }

        public static ShortUserData ShortUserDataFromUser(Core.Models.User user)
        {
            var shortUserData = new ShortUserData()
            {
                Name = user.Name,
                Gender = user.Gender,
                Birthday = user.Birthday,
                Active = (user.RevokedBy == null && user.RevokedOn == null)
            };
            return shortUserData;
        }
    }
}