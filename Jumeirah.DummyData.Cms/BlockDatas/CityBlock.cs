using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;

namespace Jumeirah.DummyData.Cms
{
    [ContentType(GUID = "{29949142-47CF-422E-8326-B49AB8FC5084}", DisplayName = "City Info", GroupName = "Dummy Data")]
    public class CityBlock : BlockData
    {
        [CultureSpecific]
        [Required(AllowEmptyStrings = false)]
        [Display(GroupName = Constants.BasicTab, Order = 1)]
        public virtual string Name { get; set; }

        [Display(GroupName = Constants.BasicTab, Order = 2)]
        public virtual string Slug { get; set; }

        [CultureSpecific]
        [Display(GroupName = Constants.BasicTab, Order = 3)]
        public virtual XhtmlString Desciption { get; set; }

        [CultureSpecific]
        [UIHint(UIHint.Image)]
        [Display(GroupName = Constants.BasicTab, Order = 4)]
        public virtual ContentReference Image { get; set; }

        [CultureSpecific]
        [Display(GroupName = Constants.ExtendTab, Order = 5)]
        public virtual IEnumerable<ContentReference> OtherImages { get; set; }
    }
}
