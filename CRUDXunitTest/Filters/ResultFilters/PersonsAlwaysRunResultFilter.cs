using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContactsManager.Filters.ResultFilters
{
    //This filter always run irrespective of short circuits
    public class PersonsAlwaysRunResultFilter : IAlwaysRunResultFilter
    {
        
        public void OnResultExecuted(ResultExecutedContext context)
        {
            
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if(context.Filters.OfType<SkipFilter>().Any())
            {
                return;
            }
            //Before logic wo n't work for any action method having skip filter attribute
        }
    }
}
