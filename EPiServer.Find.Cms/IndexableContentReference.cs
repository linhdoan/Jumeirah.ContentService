using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Find.Cms
{
    public class IndexableContentReference : ContentReference
    {
        public IndexableContentReference()
        {
        }

        public IndexableContentReference(ContentReference wrapped)
        {
            this.ID = wrapped.ID;
            this.WorkID = wrapped.WorkID;
            this.ProviderName = wrapped.ProviderName;
        }
    }
}
