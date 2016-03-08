using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Find.Cms
{
    public static class ILocalizableExtensions
    {
        public static IEnumerable<string> ExistingLanguages(this ILocalizable localizable)
        {
            return Enumerable.Select<CultureInfo, string>(localizable.ExistingLanguages, (Func<CultureInfo, string>)(x => x.Name));
        }
    }
}
