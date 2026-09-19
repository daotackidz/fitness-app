using FitBodyApp.Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FitBodyApp.Api.Middleware;

public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (_serviceProvider.GetService(validatorType) is IValidator validator)
            {
                var result = await validator.ValidateAsync(new ValidationContext<object>(argument));
                if (!result.IsValid)
                {
                    var message = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
                    throw AppException.ValidationError(message);
                }
            }
        }

        await next();
    }
}
