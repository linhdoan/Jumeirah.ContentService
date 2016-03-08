using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumeirah.DummyData
{
    public class City   // : ICity
    {
        public virtual string Name { get; set; }
        public virtual string Slug { get; set; }

        public virtual string Desciption { get; set; }

        public virtual Uri Image { get; set; }

        public virtual IEnumerable<Uri> OtherImages { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    //interface ICity
    //{
    //    string Name { get; set; }
    //    string Slug { get; set; }
    //}
}
