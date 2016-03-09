using EPiServer.Find;
using Jumeirah.DummyData.Cms;
using Jumeirah.DummyData.Cms.BlockDatas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumeirah.FindClient
{
    public class HotelRepository : IHotelRepository
    {
        public IClient Client
        {
            get;
            set;
        }

        public IEnumerable<HotelBlock> GetAllHotels()
        {
            return Client.Search<HotelBlock>().GetResult();
        }
    }
}
