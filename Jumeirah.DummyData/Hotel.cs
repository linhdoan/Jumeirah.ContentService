using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumeirah.DummyData
{
    //[Serializable]
    public class Hotel
    {
        public virtual string Name { get; set; }

        public virtual string Slug { get; set; }    // short name

        public virtual string ShortAddress { get; set; }

        public virtual string LocationsString { get; set; }

        public virtual string Phone { get; set; }

        public virtual string Email { get; set; }

        public virtual Uri Website { get; set; }

        public virtual string Desciption { get; set; }

        public virtual Uri Image { get; set; }

        public virtual City City { get; set; }  // point to its City

        public virtual IEnumerable<Room> Rooms { get; set; }

        //public virtual int RoomCount { 
        //    get{
        //    return ???
        //    }
        //}

        public virtual double StarRating { get; set; }        

        public virtual string LocalCurrency { get; set; }

        public virtual double PriceUSD { get; set; }

        public virtual string PriceFrom { get; set; }

        public virtual IEnumerable<string> Features { get; set; }

        public virtual IEnumerable<Uri> OtherImages { get; set; }
    }
}
