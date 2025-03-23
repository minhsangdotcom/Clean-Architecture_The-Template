using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Contracts.ApiWrapper;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Common.Messages;

namespace Api.common.RouteResults;

public static class Results
{
    public static ActionResult<ApiResponse<T>> Ok200<T>(this ControllerBase controller, T data)
        where T : class =>
        controller.ToActionResult(
            new Result<ApiResponse<T>>(
                new ApiResponse<T>(data, Message.SUCCESS, StatusCodes.Status200OK)
            )
        );

    public static ActionResult<ApiResponse<T>> Created201<T>(
        this ControllerBase controller,
        string routeName,
        Ulid id,
        T? data = null
    )
        where T : class => controller.CreatedAtRoute(routeName, new { id }, data);

    public static ActionResult NoContent204(this ControllerBase controller) =>
        controller.NoContent();
}
