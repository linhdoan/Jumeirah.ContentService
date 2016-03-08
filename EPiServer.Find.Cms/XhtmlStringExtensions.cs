using EPiServer.Core;
using EPiServer.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Find.Cms
{
    public static class XhtmlStringExtensions
    {
        public static string AsViewedByAnonymous(this XhtmlString xhtmlString)
        {
            return xhtmlString.ToHtmlString(PrincipalInfo.AnonymousPrincipal);
        }
    }
}
