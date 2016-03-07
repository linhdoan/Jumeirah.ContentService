using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Jumeirah.ContentService.Controllers
{
    public class HotelController : ApiController
    {
        public string Get(string site, string language, string cityName)
        {
            return "Called Get / Hotel";
        }
    }
}
