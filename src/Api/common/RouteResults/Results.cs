using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Contracts.ApiWrapper;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Common.Messages;

namespace Api.common.RouteResults;

public static class Results
{
    public static ActionResult<ApiResponse> Ok200(this ControllerBase controller, object data) =>
        controller.ToActionResult(
            new Result<ApiResponse>(new ApiResponse(data, Message.SUCCESS, StatusCodes.Status200OK))
        );

    public static ActionResult<ApiResponse> Created201(
        this ControllerBase controller,
        string routeName,
        Ulid id,
        object? data = null
    ) => controller.CreatedAtRoute(routeName, new { id }, data);

    public static ActionResult NoContent204(this ControllerBase controller) =>
        controller.NoContent();
}
