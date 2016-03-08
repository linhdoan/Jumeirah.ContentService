using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Security;
using Jumeirah.DummyData.Cms;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Jumeirah.ContentService.Controllers
{
    public class CityController : ApiController
    {
        public IEnumerable<CityBlock> Get(string site, string language) 
        {
            var client = Client.CreateFromConfig();
            CmsClientConventions.ApplyCmsConventions(client);
            var result = client.Search<CityBlock>().GetResult();

            //var image = client.Search<ImageData>().Filter(t => t.ContentLink.ID.Match(268)).GetResult();

            return result;
        }
    }
}


