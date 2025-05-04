using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskMaster.Infrastructure.Services;

namespace TaskMaster.Web.Endpoints
{
    public static class UserEndpoints
    {
        public static void Map(WebApplication app)
        {
            var userGroup = app.MapGroup("/api/users");

            // GET: Retrieve all users (with pagination)
            userGroup.MapGet("/", async ([FromQuery] int rowFirst, [FromQuery] int rowLast, UserService userService) =>
            {
                try
                {
                    var users = await userService.GetUsersAsync(null, rowFirst, rowLast);
                    return Results.Ok(new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Users retrieved successfully",
                        Data = users
                    });
                }
                catch (Exception ex)
                {
                    return Results.Problem(detail: $"An error occurred: {ex.Message}");
                }
            })
            .WithName("GetAllUsers")
            .WithTags("User");

            // POST: Add a new user
            userGroup.MapPost("/", async ([FromBody] UserCreateDto userDto, UserService userService) =>
            {
                try
                {
                    var result = await userService.InsertUserAsync(userDto);
                    if (!string.IsNullOrEmpty(result.ResultMessage) && result.ResultMessage.ToLower().Contains("success"))
                    {
                        return Results.Ok(new
                        {
                            StatusCode = StatusCodes.Status201Created,
                            Message = result.ResultMessage,
                            Data = userDto
                        });
                    }
                    else
                    {
                        return Results.BadRequest(new
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = result.ResultMessage
                        });
                    }
                }
                catch (Exception ex)
                {
                    return Results.Problem(detail: $"An error occurred: {ex.Message}");
                }
            })
            .WithName("InsertUser")
            .WithTags("User");

            // PUT: Update user
            userGroup.MapPut("/", async ([FromBody] UserUpdateDto userDto, UserService userService) =>
            {
                try
                {
                    var result = await userService.UpdateUserAsync(userDto);
                    if (!string.IsNullOrEmpty(result.ResultMessage) && result.ResultMessage.ToLower().Contains("success"))
                    {
                        return Results.Ok(new
                        {
                            StatusCode = StatusCodes.Status200OK,
                            Message = result.ResultMessage,
                            Data = userDto
                        });
                    }
                    else
                    {
                        return Results.BadRequest(new
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = result.ResultMessage
                        });
                    }
                }
                catch (Exception ex)
                {
                    return Results.Problem(detail: $"An error occurred: {ex.Message}");
                }
            })
            .WithName("UpdateUser")
            .WithTags("User");

            // DELETE: Delete user
            userGroup.MapDelete("/{userId}", async (int userId, UserService userService) =>
            {
                try
                {
                    var result = await userService.DeleteUserAsync(userId);
                    if (!string.IsNullOrEmpty(result.ResultMessage) && result.ResultMessage.ToLower().Contains("success"))
                    {
                        return Results.Ok(new
                        {
                            StatusCode = StatusCodes.Status200OK,
                            Message = result.ResultMessage
                        });
                    }
                    else
                    {
                        return Results.BadRequest(new
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = result.ResultMessage
                        });
                    }
                }
                catch (Exception ex)
                {
                    return Results.Problem(detail: $"An error occurred: {ex.Message}");
                }
            })
            .WithName("DeleteUser")
            .WithTags("User");
        }
    }
}
