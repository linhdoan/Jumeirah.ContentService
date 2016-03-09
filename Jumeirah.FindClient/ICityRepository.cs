using EPiServer.Find;
using Jumeirah.DummyData.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumeirah.FindClient
{
    public interface ICityRepository
    {
        IClient Client { get; set; }
        IEnumerable<CityBlock> GetAllCities();
    }
}
