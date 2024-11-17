using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Hangfires;

public class ValidateHangfireStorage : IValidateOptions<HangfireStorageSettings>
{
    public ValidateOptionsResult Validate(string? name, HangfireStorageSettings options)
    {
        
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(options.ConnectionString)} must be not null or empty"
            );
        }

        return ValidateOptionsResult.Success;
    }
}
