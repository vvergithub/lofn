using System.Web;
using System.Web.Mvc;

namespace MeetingAssistant_NET46
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
