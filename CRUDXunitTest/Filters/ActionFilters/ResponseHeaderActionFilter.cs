using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters.ActionFilters
{

    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;

        private string? Key {  get; set; }
        private string? Value { get; set; }

        public ResponseHeaderFilterFactoryAttribute(string key,string value)
        {
            Key = key;
            Value = value;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
            filter.Key = Key;
            filter.Value = Value;
            return filter;
        }
    }


    public class ResponseHeaderActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger;

        public string Key { get; set; }
        public string Value { get; set; }

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method - before", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
            await next();
            _logger.LogInformation("{FilterName}.{MethodName} method - after", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
            context.HttpContext.Response.Headers[Key] = Value;
        }
    }
}
