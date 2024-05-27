using CrudApplication.API.Contracts.User.In;
using CrudApplication.API.Contracts.User.Out;
using CrudApplication.API.Contracts.User;
using CrudApplication.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using CrudApplication.API.Contracts;


namespace CrudApplication.API.Controllers
{
    [ApiController()]
    [Tags("Users")]
    [Route("users")]
    public class UserContoller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserContoller> _logger;


        public UserContoller(IUserService userService, ILogger<UserContoller> logger)
        {
            _userService = userService;
            _logger = logger;
        }


        [HttpPost("create")]
        [ProducesResponseType(typeof(CreatedUserData), 201)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 403)]
        [ProducesResponseType(typeof(Error), 409)]
        [ProducesResponseType(typeof(Error), 422)]
        public async Task<IActionResult> CreateUser([FromHeader] string? login, [FromHeader] string? password, [FromBody] CreateUserData? createUserData)
        {
            var user = Mapping.UserFromLoginAndPassword(login, password);
            var (exists, errorMessage) = await _userService.ExistsUser(user);
            if (!string.IsNullOrEmpty(errorMessage) || !exists)
            {
                var error = new Error(401, "Unauthorized", "Invalid login and/or password");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 401
                };
                return ErrorResponse;
            }
            (user, _) = await _userService.GetOneUser(user);
            if (!user.Admin)
            {
                var error = new Error(403, "Forbidden", "User is not admin");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 403
                };
                return ErrorResponse;
            }
            if (createUserData == null)
            {
                var error = new Error(422, "Unprocessable Entity", "Input json is null");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 422
                };
                return ErrorResponse;
            }
            user = Mapping.UserFromCreateUserData(createUserData, login, DateTime.Now);
            (var uniqueLogin, errorMessage) = await _userService.IsUniqueUserLogin(user);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                var error = new Error(422, "Unprocessable Entity", errorMessage);
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 422
                };
                return ErrorResponse;
            }
            if (!uniqueLogin)
            {
                var error = new Error(409, "Conflict", "Login already exists");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 409
                };
                return ErrorResponse;
            }
            (var guid, errorMessage) = await _userService.CreateUser(user);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                var error = new Error(422, "Unprocessable Entity", errorMessage);
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 422
                };
                return ErrorResponse;
            }
            var created = new CreatedUserData(201, "Created", guid);
            var createdResponse = new JsonResult(created)
            {
                ContentType = "application/json",
                StatusCode = 201
            };
            return createdResponse;
        }


        [HttpPut("update-name-gender-birthday/{guid}")]
        [ProducesResponseType(typeof(UpdatedUserData), 200)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 403)]
        [ProducesResponseType(typeof(Error), 404)]
        [ProducesResponseType(typeof(Error), 422)]
        public async Task<IActionResult> UpdateNameOrGenderOrBirthday([FromHeader] string? login, [FromHeader] string? password, Guid guid, [FromBody] UpdateUserData? updateUserData)
        {
            var user = Mapping.UserFromLoginAndPassword(login, password);
            var (exists, errorMessage) = await _userService.ExistsUser(user);
            if (!string.IsNullOrEmpty(errorMessage) || !exists)
            {
                var error = new Error(401, "Unauthorized", "Invalid login and/or password");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 401
                };
                return ErrorResponse;
            }
            user = Mapping.UserFromGuid(guid);
            (exists, errorMessage) = await _userService.ExistsUser(user);
            if (!string.IsNullOrEmpty(errorMessage) || !exists)
            {
                var error = new Error(404, "Not found", "User with selected guid not found");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 404
                };
                return ErrorResponse;
            }
            (user, _) = await _userService.GetOneUser(user);
            if (!user.Admin || user.Guid != guid || user.RevokedBy != null || user.RevokedOn != null)
            {
                var error = new Error(403, "Forbidden", "User has not permisions for updating");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 403
                };
                return ErrorResponse;
            }
            if (updateUserData == null || (updateUserData.Name == null && updateUserData.Gender == null && updateUserData.Birthday == null))
            {
                var error = new Error(422, "Unprocessable Entity", "Input json is null or all of fields in json are null");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 422
                };
                return ErrorResponse;
            }
            user = Mapping.UserFromUpdateUserData(guid, updateUserData, login, DateTime.Now);
            errorMessage = await _userService.UpdateUserNameOrGenderOrBirthday(user);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                var error = new Error(422, "Unprocessable Entity", errorMessage);
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 422
                };
                return ErrorResponse;
            }
            var ok = new UpdatedUserData(200, "Ok", "User has been sucessfuly updated");
            var okResponse = new JsonResult(ok)
            {
                ContentType = "application/json",
                StatusCode = 200
            };
            return okResponse;
        }


        [HttpPut("update-password/{guid}")]
        [ProducesResponseType(typeof(UpdatedUserData), 200)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 403)]
        [ProducesResponseType(typeof(Error), 404)]
        [ProducesResponseType(typeof(Error), 422)]
        public async Task<IActionResult> UpdatePassword([FromHeader] string? login, [FromHeader] string? password, Guid guid, [FromBody] UpdateUserData? updateUserData)
        {
            var user = Mapping.UserFromLoginAndPassword(login, password);
            var (exists, errorMessage) = await _userService.ExistsUser(user);
            if (!string.IsNullOrEmpty(errorMessage) || !exists)
            {
                var error = new Error(401, "Unauthorized", "Invalid login and/or password");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 401
                };
                return ErrorResponse;
            }
            user = Mapping.UserFromGuid(guid);
            (exists, errorMessage) = await _userService.ExistsUser(user);
            if (!string.IsNullOrEmpty(errorMessage) || !exists)
            {
                var error = new Error(404, "Not found", "User with selected guid not found");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 404
                };
                return ErrorResponse;
            }
            (user, _) = await _userService.GetOneUser(user);
            if (!user.Admin || user.Guid != guid || user.RevokedBy != null || user.RevokedOn != null)
            {
                var error = new Error(403, "Forbidden", "User has not permisions for updating");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 403
                };
                return ErrorResponse;
            }
            if (updateUserData == null || user.Password == null)
            {
                var error = new Error(422, "Unprocessable Entity", "Input json is null or all of fields in json are null");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 422
                };
                return ErrorResponse;
            }
            user = Mapping.UserFromUpdateUserData(guid, updateUserData, login, DateTime.Now);
            errorMessage = await _userService.UpdateUserPassword(user);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                var error = new Error(422, "Unprocessable Entity", errorMessage);
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 422
                };
                return ErrorResponse;
            }
            var ok = new UpdatedUserData(200, "Ok", "User has been sucessfuly updated");
            var okResponse = new JsonResult(ok)
            {
                ContentType = "application/json",
                StatusCode = 200
            };
            return okResponse;
        }


        [HttpPut("update-login/{guid}")]
        [ProducesResponseType(typeof(UpdatedUserData), 200)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 403)]
        [ProducesResponseType(typeof(Error), 404)]
        [ProducesResponseType(typeof(Error), 409)]
        [ProducesResponseType(typeof(Error), 422)]
        public async Task<IActionResult> UpdateLogin([FromHeader] string? login, [FromHeader] string? password, Guid guid, [FromBody] UpdateUserData? updateUserData)
        {
            var user = Mapping.UserFromLoginAndPassword(login, password);
            var (exists, errorMessage) = await _userService.ExistsUser(user);
            if (!string.IsNullOrEmpty(errorMessage) || !exists)
            {
                var error = new Error(401, "Unauthorized", "Invalid login and/or password");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 401
                };
                return ErrorResponse;
            }
            user = Mapping.UserFromGuid(guid);
            (exists, errorMessage) = await _userService.ExistsUser(user);
            if (!string.IsNullOrEmpty(errorMessage) || !exists)
            {
                var error = new Error(404, "Not found", "User with selected guid not found");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 404
                };
                return ErrorResponse;
            }
            (user, _) = await _userService.GetOneUser(user);
            if (!user.Admin || user.Guid != guid || user.RevokedBy != null || user.RevokedOn != null)
            {
                var error = new Error(403, "Forbidden", "User has not permisions for updating");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 403
                };
                return ErrorResponse;
            }
            if (updateUserData == null || user.Login == null)
            {
                var error = new Error(422, "Unprocessable Entity", "Input json is null or all of fields in json are null");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 422
                };
                return ErrorResponse;
            }
            user = Mapping.UserFromUpdateUserData(guid, updateUserData, login, DateTime.Now);
            (var uniqueLogin, errorMessage) = await _userService.IsUniqueUserLogin(user);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                var error = new Error(422, "Unprocessable Entity", errorMessage);
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 422
                };
                return ErrorResponse;
            }
            if (!uniqueLogin)
            {
                var error = new Error(409, "Conflict", "Login already exists");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 409
                };
                return ErrorResponse;
            }
            errorMessage = await _userService.UpdateUserLogin(user);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                var error = new Error(422, "Unprocessable Entity", errorMessage);
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 422
                };
                return ErrorResponse;
            }
            var ok = new UpdatedUserData(200, "Ok", "User has been sucessfuly updated");
            var okResponse = new JsonResult(ok)
            {
                ContentType = "application/json",
                StatusCode = 200
            };
            return okResponse;
        }


        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<FullUserData>), 200)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 403)]
        public async Task<IActionResult> GetAllUsersActive([FromHeader] string? login, [FromHeader] string? password)
        {
            var user = Mapping.UserFromLoginAndPassword(login, password);
            var (exists, errorMessage) = await _userService.ExistsUser(user);
            if (!string.IsNullOrEmpty(errorMessage) || !exists)
            {
                var error = new Error(401, "Unauthorized", "Invalid login and/or password");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 401
                };
                return ErrorResponse;
            }
            (user, _) = await _userService.GetOneUser(user);
            if (!user.Admin)
            {
                var error = new Error(403, "Forbidden", "User is not admin");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 403
                };
                return ErrorResponse;
            }
            var (users, _) = await _userService.GetAllUsersActive();
            var fullUserDatas = users.Select(u => Mapping.FullUserDataFromUser(u)).ToList();
            var okResponse = new JsonResult(fullUserDatas)
            {
                ContentType = "application/json",
                StatusCode = 200
            };
            return okResponse;
        }


        [HttpGet("by-login/{userLogin}")]
        [ProducesResponseType(typeof(ShortUserData), 200)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 403)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> GetOneUserByLogin([FromHeader] string? login, [FromHeader] string? password, string? userLogin)
        {
            var user = Mapping.UserFromLoginAndPassword(login, password);
            var (exists, errorMessage) = await _userService.ExistsUser(user);
            if (!string.IsNullOrEmpty(errorMessage) || !exists)
            {
                var error = new Error(401, "Unauthorized", "Invalid login and/or password");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 401
                };
                return ErrorResponse;
            }
            (user, _) = await _userService.GetOneUser(user);
            if (!user.Admin)
            {
                var error = new Error(403, "Forbidden", "User is not admin");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 403
                };
                return ErrorResponse;
            }
            user = Mapping.UserFromLogin(userLogin);
            (exists, errorMessage) = await _userService.ExistsUser(user);
            if (!string.IsNullOrEmpty(errorMessage) || !exists)
            {
                var error = new Error(404, "Unauthorized", "User with selected login not found");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 404
                };
                return ErrorResponse;
            }
            (user, _) = await _userService.GetOneUser(user);
            var shortUserData = Mapping.ShortUserDataFromUser(user);
            var okResponse = new JsonResult(shortUserData)
            {
                ContentType = "application/json",
                StatusCode = 200
            };
            return okResponse;
        }


        [HttpGet("me")]
        [ProducesResponseType(typeof(FullUserData), 200)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 403)]
        public async Task<IActionResult> GetSelfUser([FromHeader] string login, [FromHeader] string password)
        {
            var user = Mapping.UserFromLoginAndPassword(login, password);
            var (exists, errorMessage) = await _userService.ExistsUser(user);
            if (!string.IsNullOrEmpty(errorMessage) || !exists)
            {
                var error = new Error(401, "Unauthorized", "Invalid login and/or password");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 401
                };
                return ErrorResponse;
            }
            (user, _) = await _userService.GetOneUser(user);
            if (user.RevokedBy != null || user.RevokedOn != null)
            {
                var error = new Error(403, "Forbidden", "User is not active");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 403
                };
                return ErrorResponse;
            }
            var fullUserData = Mapping.FullUserDataFromUser(user);
            var okResponse = new JsonResult(fullUserData)
            {
                ContentType = "application/json",
                StatusCode = 200
            };
            return okResponse;
        }


        [HttpGet("older-that/{years}")]
        [ProducesResponseType(typeof(FullUserData), 200)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 403)]
        [ProducesResponseType(typeof(Error), 422)]
        public async Task<IActionResult> GetAllUsersOlderThat([FromHeader] string? login, [FromHeader] string? password, int years)
        {
            var user = Mapping.UserFromLoginAndPassword(login, password);
            var (exists, errorMessage) = await _userService.ExistsUser(user);
            if (!string.IsNullOrEmpty(errorMessage) || !exists)
            {
                var error = new Error(401, "Unauthorized", "Invalid login and/or password");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 401
                };
                return ErrorResponse;
            }
            (user, _) = await _userService.GetOneUser(user);
            if (!user.Admin)
            {
                var error = new Error(403, "Forbidden", "User is not admin");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 403
                };
                return ErrorResponse;
            }
            if (years <= 0)
            {
                var error = new Error(422, "Unprocessable Entity", "Input parameter 'years' must be greater that 0");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 422
                };
                return ErrorResponse;
            }
            var birthday = DateTime.Now.Subtract(TimeSpan.FromDays(365.25 * years));
            user = Mapping.UserFromDateTime(birthday);
            (var users, _) = await _userService.GetAllUsersOlderThat(user);
            var fullUserDatas = users.Select(u => Mapping.FullUserDataFromUser(u)).ToList();
            var okResponse = new JsonResult(fullUserDatas)
            {
                ContentType = "application/json",
                StatusCode = 200
            };
            return okResponse;
        }


        [HttpDelete("delete/{guid}")]
        [ProducesResponseType(typeof(UpdatedUserData), 200)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 403)]
        [ProducesResponseType(typeof(Error), 404)]
        public async Task<IActionResult> DeleteUser([FromHeader] string? login, [FromHeader] string? password, Guid guid, [FromQuery] bool soft = true)
        {
            var user = Mapping.UserFromLoginAndPassword(login, password);
            var (exists, errorMessage) = await _userService.ExistsUser(user);
            if (!string.IsNullOrEmpty(errorMessage) || !exists)
            {
                var error = new Error(401, "Unauthorized", "Invalid login and/or password");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 401
                };
                return ErrorResponse;
            }
            (user, _) = await _userService.GetOneUser(user);
            if (!user.Admin)
            {
                var error = new Error(403, "Forbidden", "User is not admin");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 403
                };
                return ErrorResponse;
            }
            user = Mapping.UserFromGuid(guid, login, DateTime.Now);
            (exists, errorMessage) = await _userService.ExistsUser(user);
            if (!string.IsNullOrEmpty(errorMessage) || !exists)
            {
                var error = new Error(404, "Not found", "User with selected guid not found");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 404
                };
                return ErrorResponse;
            }
            _ = await _userService.DeleteUser(user, soft);
            var ok = new UpdatedUserData(200, "Ok", "User has been sucessfuly deleted");
            var okResponse = new JsonResult(ok)
            {
                ContentType = "application/json",
                StatusCode = 200
            };
            return okResponse;
        }


        [HttpPatch("recovery/{guid}")]
        [ProducesResponseType(typeof(UpdatedUserData), 200)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 403)]
        [ProducesResponseType(typeof(Error), 404)]
        [ProducesResponseType(typeof(Error), 409)]
        public async Task<IActionResult> RecoveryUser([FromHeader] string? login, [FromHeader] string? password, Guid guid)
        {
            var user = Mapping.UserFromLoginAndPassword(login, password);
            var (exists, errorMessage) = await _userService.ExistsUser(user);
            if (!string.IsNullOrEmpty(errorMessage) || !exists)
            {
                var error = new Error(401, "Unauthorized", "Invalid login and/or password");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 401
                };
                return ErrorResponse;
            }
            (user, _) = await _userService.GetOneUser(user);
            if (!user.Admin)
            {
                var error = new Error(403, "Forbidden", "User is not admin");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 403
                };
                return ErrorResponse;
            }
            user = Mapping.UserFromGuid(guid);
            (exists, errorMessage) = await _userService.ExistsUser(user);
            if (!string.IsNullOrEmpty(errorMessage) || !exists)
            {
                var error = new Error(404, "Not found", "User with selected guid not found");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 404
                };
                return ErrorResponse;
            }
            (user, _) = await _userService.GetOneUser(user);
            if (user.RevokedOn == null || user.RevokedBy == null)
            {
                var error = new Error(409, "Conflict", "User is not deleted");
                _logger.LogError(error.ToLog());
                var ErrorResponse = new JsonResult(error)
                {
                    ContentType = "application/json",
                    StatusCode = 404
                };
                return ErrorResponse;
            }
            user = Mapping.UserFromGuid(guid, login, DateTime.Now);
            _ = await _userService.RecoveryUser(user);
            var ok = new UpdatedUserData(200, "Ok", "User has been sucessfuly recovered");
            var okResponse = new JsonResult(ok)
            {
                ContentType = "application/json",
                StatusCode = 200
            };
            return okResponse;
        }
    }
}