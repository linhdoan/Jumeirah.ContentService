using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Jumeirah.ContentService.Controllers
{
    //[RoutePrefix("content/{site}/{language}/city")]
    public class CityController : ApiController
    {
        public string Get(string site, string language) 
        {
            return "Called Get / City";
        }
    }
}
