using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
namespace TaskMaster.Infrastructure.Data
{
    public class UserDataAccess
    {
        private readonly string _connectionString;

        public UserDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TaskMasterDb")
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        // GET users with optional filtering by userId
        public async Task<IEnumerable<UserDto>> GetUsers(int? userId = null, int rowFirst = 1, int rowLast = 10)
        {
            var users = new List<UserDto>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("PROC_CRUD_Users", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@TaskID", 0);
            command.Parameters.AddWithValue("@UserId", userId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@RowFirst", rowFirst);
            command.Parameters.AddWithValue("@RowLast", rowLast);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(new UserDto
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                });
            }

            return users;
        }

        // INSERT new user
        public async Task<UserOperationResultDto> InsertUser(UserCreateDto user)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("PROC_CRUD_Users", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@TaskID", 1);
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

            var output = new SqlParameter("@ResultMessage", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(output);

            await command.ExecuteNonQueryAsync();

            return new UserOperationResultDto
            {
                ResultMessage = output.Value?.ToString() ?? ""
            };
        }

        // UPDATE existing user
        public async Task<UserOperationResultDto> UpdateUser(UserUpdateDto user)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("PROC_CRUD_Users", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@TaskID", 2);
            command.Parameters.AddWithValue("@UserId", user.UserId);
            command.Parameters.AddWithValue("@Name", user.Name ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash ?? (object)DBNull.Value);

            var output = new SqlParameter("@ResultMessage", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(output);

            await command.ExecuteNonQueryAsync();

            return new UserOperationResultDto
            {
                ResultMessage = output.Value?.ToString() ?? ""
            };
        }

        // DELETE user by ID
        public async Task<UserOperationResultDto> DeleteUser(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("PROC_CRUD_Users", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@TaskID", 3);
            command.Parameters.AddWithValue("@UserId", userId);

            var output = new SqlParameter("@ResultMessage", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(output);

            await command.ExecuteNonQueryAsync();

            return new UserOperationResultDto
            {
                ResultMessage = output.Value?.ToString() ?? ""
            };
        }

        // LOGIN user
        public async Task<(UserDto? User, string ResultMessage)> Login(UserLoginDto login)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("PROC_CRUD_Users", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@TaskID", 4);
            command.Parameters.AddWithValue("@Email", login.Email);
            command.Parameters.AddWithValue("@PasswordHash", login.PasswordHash);

            var output = new SqlParameter("@ResultMessage", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(output);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var user = new UserDto
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Name = reader["Name"] as string,
                    Email = reader["Email"] as string,
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                };

                return (user, output.Value?.ToString() ?? "Login successful");
            }

            return (null, output.Value?.ToString() ?? "Invalid credentials");
        }
    }
}
