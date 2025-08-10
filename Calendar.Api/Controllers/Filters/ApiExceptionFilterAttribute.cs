using Calendar.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Calendar.Api.Controllers.Filters
{
    /// <summary>
    /// Exception filter attribute for handling and mapping application exceptions to HTTP responses.
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

        public ApiExceptionFilterAttribute()
        {
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(FluentValidation.ValidationException), HandleFluentValidationException },
                { typeof(ValidationException), HandleValidationException },
                { typeof(NotFoundException), HandleNotFoundException },
                { typeof(ExternalApiException), HandleExternalApiException },
                { typeof(MissingClientHeaderException), HandleMissingClientHeaderException },
                { typeof(AuthorizationException), HandleAuthorizationException },
                { typeof(NotAllowedException ), HandleNotAllowedException }
            };
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);
            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            var type = context.Exception.GetType();

            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);
                return;
            }

            if (!context.ModelState.IsValid)
            {
                HandleInvalidModelStateException(context);
                return;
            }

            HandleUnknownException(context);
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            var innerExceptionMessage = context.Exception.InnerException?.Message ?? " null";

            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Message:" + context.Exception.Message + "; Inner:" + innerExceptionMessage,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;
        }

        private void HandleInvalidModelStateException(ExceptionContext context)
        {
            var details = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleValidationException(ExceptionContext context)
        {
            var exception = context.Exception as ValidationException;
            context.Result = new BadRequestObjectResult(new ValidationProblemDetails(exception.Errors)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Detail = exception.Message
            });

            context.ExceptionHandled = true;
        }

        private void HandleFluentValidationException(ExceptionContext context)
        {
            var fluentException = context.Exception as FluentValidation.ValidationException;
            var exception = new ValidationException(fluentException.Errors);

            context.Result = new BadRequestObjectResult(new ValidationProblemDetails(exception.Errors)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Detail = exception.Message
            });

            context.ExceptionHandled = true;
        }

        private void HandleNotFoundException(ExceptionContext context)
        {
            var exception = context.Exception as NotFoundException;
            context.Result = new NotFoundObjectResult(new ProblemDetails
            {
                Title = "The specified resource was not found.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Detail = exception.Message
            });

            context.ExceptionHandled = true;
        }

        private void HandleExternalApiException(ExceptionContext context)
        {
            var exception = context.Exception as ExternalApiException;
            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status503ServiceUnavailable,
                Title = exception.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            })
            {
                StatusCode = StatusCodes.Status503ServiceUnavailable
            };

            context.ExceptionHandled = true;
        }

        private void HandleMissingClientHeaderException(ExceptionContext context)
        {
            var exception = context.Exception as MissingClientHeaderException;
            context.Result = new BadRequestObjectResult(new ProblemDetails
            {
                Title = exception.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            });

            context.ExceptionHandled = true;
        }

        private void HandleAuthorizationException(ExceptionContext context)
        {
            var exception = context.Exception as AuthorizationException;
            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = exception.Message,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };

            context.ExceptionHandled = true;
        }

        private void HandleNotAllowedException(ExceptionContext context)
        {
            var exception = context.Exception as NotAllowedException;
            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = exception.Message,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };

            context.ExceptionHandled = true;
        }
    }
}
