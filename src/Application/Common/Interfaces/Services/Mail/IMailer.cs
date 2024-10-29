using Application.Common.Interfaces.Registers;

namespace Application.Common.Interfaces.Services.Mail;

public interface IMailer : ISingleton
{
    public IMailService Email();
}
