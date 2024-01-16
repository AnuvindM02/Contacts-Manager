using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters.ResourceFilters
{
    //Action method will be disabled if this filter is applied

    public class FeatureDisabledResourceFilter : IAsyncResourceFilter
    {
        private readonly ILogger<FeatureDisabledResourceFilter> _logger;
        private readonly bool _isDisabled;

        public FeatureDisabledResourceFilter(ILogger<FeatureDisabledResourceFilter> logger, bool isDisabled = true)
        {
            _logger = logger;
            _isDisabled = isDisabled;
        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            //OnResourceExecuting

            _logger.LogInformation("{FilterName}.{MethodName} - before",nameof(FeatureDisabledResourceFilter),nameof(OnResourceExecutionAsync));

            if(_isDisabled)
            {
                context.Result = new StatusCodeResult(501);
            }
            else
                await next();

            //OnResourceExecution

            _logger.LogInformation("{FilterName}.{MethodName} - after", nameof(FeatureDisabledResourceFilter), nameof(OnResourceExecutionAsync));

        }
    }
}
