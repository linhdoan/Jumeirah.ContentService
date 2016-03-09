using EPiServer.Find;
using EPiServer.Find.Cms;
using Jumeirah.DummyData.Cms.BlockDatas;
using Jumeirah.FindClient;
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
        private IHotelRepository _hotelRepository;

        public HotelController(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public IEnumerable<HotelBlock> Get(string site, string language) 
        {
            var client = Client.CreateFromConfig();
            CmsClientConventions.ApplyCmsConventions(client);
            _hotelRepository.Client = client;
            var result = _hotelRepository.GetAllHotels();

            return result;
        }
    }
}
