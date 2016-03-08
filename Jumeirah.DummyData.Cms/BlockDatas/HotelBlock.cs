using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using Jumeirah.DummyData.Cms.CustomProperties;

namespace Jumeirah.DummyData.Cms.BlockDatas
{
    [ContentType(GUID = "{CE7817E3-C6BB-42AD-A3BF-9FEF050EC01A}", DisplayName="Hotel Info", GroupName="Dummy Data")]
    public class HotelBlock : BlockData
    {
        [CultureSpecific]
        [Required(AllowEmptyStrings = false)]
        [Display(GroupName = Constants.BasicTab, Order = 1)]
        public virtual string Name { get; set; }

        [Display(GroupName = Constants.BasicTab, Order = 2)]
        public virtual string Slug { get; set; }    // short name

        [Required(AllowEmptyStrings = false)]
        [Display(GroupName = Constants.BasicTab, Order = 3)]
        public virtual string ShortAddress { get; set; }

        [Display(GroupName = Constants.BasicTab, Order = 4)]
        public virtual string LocationsString { get; set; }

        [Display(GroupName = Constants.BasicTab, Order = 5)]
        public virtual string Phone { get; set; }

        [Display(GroupName = Constants.BasicTab, Order = 6)]
        public virtual string Email { get; set; }

        [Display(GroupName = Constants.BasicTab, Order = 7)]
        public virtual Url Website { get; set; }

        [CultureSpecific]
        [Display(GroupName = Constants.BasicTab, Order = 8)]
        public virtual XhtmlString Desciption { get; set; }

        [CultureSpecific]
        [UIHint(UIHint.Image)]
        [Display(GroupName = Constants.BasicTab, Order = 9)]
        public virtual ContentReference Image { get; set; }

        [UIHint(UIHint.Block)]
        [Display(GroupName = Constants.BasicTab, Order = 9)]
        public virtual ContentReference City { get; set; }  // point to its City

        [CultureSpecific]
        [Display(GroupName = Constants.BasicTab, Order = 13)]
        public virtual IEnumerable<ContentReference> Rooms { get; set; }

        [Display(GroupName = Constants.ExtendTab, Order = 7)]
        public virtual double StarRating { get; set; }

        //[Display(GroupName = Constants.ExtendTab, Order = 8)]
        //public virtual int RoomCount { get; set; }

        [Display(GroupName = Constants.ExtendTab, Order = 9)]
        public virtual string LocalCurrency { get; set; }

        [Display(GroupName = Constants.ExtendTab, Order = 10)]
        public virtual int PriceUSD { get; set; }

        [Display(GroupName = Constants.ExtendTab, Order = 11)]
        public virtual string PriceFrom { get; set; }

        [Display(GroupName = Constants.ExtendTab, Order = 12)]
        [BackingType(typeof(PropertyStrings))]
        [UIHint(Constants.StringList)]
        public virtual string[] Features { get; set; }
        //public virtual List<string> Features { get; set; }

        [CultureSpecific]
        [Display(GroupName = Constants.ExtendTab, Order = 13)]
        public virtual IEnumerable<ContentReference> OtherImages { get; set; }

    }
}
