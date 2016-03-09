using EPiServer.Find;
using Jumeirah.DummyData.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumeirah.FindClient
{
    public class CityRepository : ICityRepository
    {
        public IClient Client
        {
            get;
            set;
        }

        public IEnumerable<CityBlock> GetAllCities()
        {
            return Client.Search<CityBlock>().GetResult();
        }

        public CityBlock GetCity(string name)
        {
            return Client.Search<CityBlock>().Filter(x => x.Name.Match(name)).GetResult().FirstOrDefault();
        }
    }
}
