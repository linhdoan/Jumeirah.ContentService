using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Core;

namespace Jumeirah.DummyData.Cms
{
    public interface IBlockDataHasImages
    {
        ContentReference Image { get; set; }
        IEnumerable<ContentReference> OtherImages { get; set; }
    }
}
