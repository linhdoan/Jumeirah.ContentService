using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Jumeirah.ContentService.Controllers
{
    public class SpaController : ApiController
    {
        public string Get(string site, string language, string cityName)
        {
            return "Called Get / Spa by City";
        }

        public string Get(string site, string language, string cityName, string hotelName)
        {
            return "Called Get / Spa by hotel";
        }
    }
}
