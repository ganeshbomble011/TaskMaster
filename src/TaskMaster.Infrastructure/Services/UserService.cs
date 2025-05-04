using System.Collections.Generic;
using System.Threading.Tasks;
using TaskMaster.Infrastructure.Data;

namespace TaskMaster.Infrastructure.Services
{
    public class UserService
    {
        private readonly UserDataAccess _dataAccess;

        public UserService(UserDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync(int? userId = null, int rowFirst = 1, int rowLast = 10)
        {
            return await _dataAccess.GetUsers(userId, rowFirst, rowLast);
        }

        public async Task<UserOperationResultDto> InsertUserAsync(UserCreateDto dto)
        {
            return await _dataAccess.InsertUser(dto);
        }

        public async Task<UserOperationResultDto> UpdateUserAsync(UserUpdateDto dto)
        {
            return await _dataAccess.UpdateUser(dto);
        }

        public async Task<UserOperationResultDto> DeleteUserAsync(int userId)
        {
            return await _dataAccess.DeleteUser(userId);
        }

        public async Task<(UserDto? User, string ResultMessage)> LoginAsync(UserLoginDto loginDto)
        {
            return await _dataAccess.Login(loginDto);
        }
    }
} 