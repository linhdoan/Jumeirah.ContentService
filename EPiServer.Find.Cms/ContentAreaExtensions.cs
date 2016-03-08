using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Find.Cms
{
    public static class ContentAreaExtensions
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<IContent> Contents(this ContentArea contentArea)
        {
            if (contentArea == null)
                return (IEnumerable<IContent>)null;
            else
                return Enumerable.Where<IContent>(contentArea.Contents, (Func<IContent, bool>)(x =>
                {
                    if (ContentIndexer.Instance.Conventions.ShouldIndexInContentAreaConvention.ShouldIndexInContentArea(x).HasValue)
                        return ContentIndexer.Instance.Conventions.ShouldIndexInContentAreaConvention.ShouldIndexInContentArea(x).Value;
                    else
                        return false;
                }));
        }
    }
}
