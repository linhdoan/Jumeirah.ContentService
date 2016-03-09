using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Security;
using Jumeirah.DummyData.Cms;
using Jumeirah.FindClient;
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
        private ICityRepository _cityRepository;

        public CityController(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public IEnumerable<CityBlock> Get(string site, string language) 
        {
            var client = Client.CreateFromConfig();
            CmsClientConventions.ApplyCmsConventions(client);
            _cityRepository.Client = client;
            var result = _cityRepository.GetAllCities();

            return result;
        }
    }
}


