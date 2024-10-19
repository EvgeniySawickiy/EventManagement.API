using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;


namespace EventManagement.Application.Attributes
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidateModelAttribute(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                var argumentType = argument.GetType();
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

                var validator = _serviceProvider.GetService(validatorType);
                if (validator != null)
                {
                    var validateMethod = validatorType.GetMethod("Validate", new[] { argumentType });
                    var result = (FluentValidation.Results.ValidationResult)validateMethod.Invoke(validator, new[] { argument });

                    if (!result.IsValid)
                    {
                        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                        context.Result = new BadRequestObjectResult(new { Errors = errors });
                        return;
                    }
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
