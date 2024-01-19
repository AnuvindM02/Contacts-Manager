using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters
{
    public class SkipFilter: Attribute, IFilterMetadata
    {
    }
}
