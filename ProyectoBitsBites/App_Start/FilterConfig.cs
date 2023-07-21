using System.Web;
using System.Web.Mvc;

namespace ProyectoBitsBites
{
      public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // This line is added to redirect to login page if user is not logged in
            filters.Add(new Filters.VerificarSession());
        }
    }
}
    
