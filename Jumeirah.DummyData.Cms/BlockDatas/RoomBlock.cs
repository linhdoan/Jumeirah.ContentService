using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using Jumeirah.DummyData.Cms;

namespace Jumeirah.DummyData.Cms.BlockDatas
{
    [ContentType(GUID = "{951834D0-32E7-4007-A8CC-72FEB02CE676}", DisplayName = "Room Info", GroupName = "Dummy Data")]
    public class RoomBlock : BlockData, IBlockDataHasImages
    {
        [CultureSpecific]
        [Required(AllowEmptyStrings = false)]
        [Display(GroupName = Constants.BasicTab, Order = 1)]
        public virtual string Title { get; set; }

        [CultureSpecific]
        [Display(GroupName = Constants.BasicTab, Order = 2)]
        public virtual XhtmlString Desciption { get; set; }

        [CultureSpecific]        
        [Display(GroupName = Constants.BasicTab, Order = 3)]
        public virtual string Size { get; set; }

        [CultureSpecific]
        [Display(GroupName = Constants.BasicTab, Order = 4)]
        public virtual string View { get; set; }

        [CultureSpecific]
        [UIHint(UIHint.LongString)]
        [Display(GroupName = Constants.BasicTab, Order = 5)]
        public virtual string Occupancy { get; set; }

        [CultureSpecific]
        [Display(GroupName = Constants.BasicTab, Order = 6)]
        public virtual string BedInfo { get; set; }

        [CultureSpecific]
        [UIHint(UIHint.LongString)]
        [Display(GroupName = Constants.BasicTab, Order = 7)]
        public virtual string DepositDetails { get; set; }

        [CultureSpecific]
        [UIHint(UIHint.Image)]
        [Display(GroupName = Constants.BasicTab, Order = 8)]
        public virtual ContentReference Image { get; set; }

        [CultureSpecific]
        [Display(GroupName = Constants.ExtendTab, Order = 5)]
        public virtual IEnumerable<ContentReference> OtherImages { get; set; }

    }
}
