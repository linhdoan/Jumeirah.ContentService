using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Find.Cms
{
    public static class XhtmlStringProjectionExtensions
    {
        public static string AsHighlighted(this XhtmlString xhtmlString)
        {
            throw new ClientException("The AsHighlighted method should only be used in projection expressions");
        }

        public static string AsHighlighted(this XhtmlString xhtmlString, HighlightSpec highlightSpec)
        {
            throw new ClientException("The AsHighlighted method should only be used in projection expressions");
        }

        public static string AsCropped(this XhtmlString xhtmlString, int maxLength)
        {
            throw new ClientException("The AsCropped method should only be used in projection expressions, such as when using the Select method.");
        }
    }
}
