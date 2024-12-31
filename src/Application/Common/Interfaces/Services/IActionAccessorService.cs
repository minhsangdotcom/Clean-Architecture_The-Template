using Application.Common.Interfaces.Registers;

namespace Application.Common.Interfaces.Services;

public interface IActionAccessorService : ISingleton
{
    public string? Id { get; }

    public string? GetController();

    public string? GetAction();

    public string? GetRouteData(string name);

    public string? GetHttpMethod();

    public string? GetRequestPath();
}
