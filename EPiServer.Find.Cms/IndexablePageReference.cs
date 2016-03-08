using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Find.Cms
{
    public class IndexablePageReference : PageReference
    {
        public IndexablePageReference()
        {
        }

        public IndexablePageReference(PageReference wrapped)
        {
            this.ID = wrapped.ID;
            this.WorkID = wrapped.WorkID;
            this.ProviderName = wrapped.ProviderName;
        }
    }
}
