using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace Jumeirah.DummyData.Cms
{
    public static class Helpers
    {
        private static Injected<UrlResolver> _urlResolver;

        public static string ImageUrl(this IBlockDataHasImages hasImageBlock)
        {
            return GetPublicUrl(hasImageBlock.Image);
        }

        public static IEnumerable<string> OtherImagesUrl(this IBlockDataHasImages hasImagesBlock)
        {
            if (hasImagesBlock.OtherImages == null || hasImagesBlock.OtherImages.Count() < 1)
            {
                return Enumerable.Empty<string>();
            }

            var result = new List<string>();
            foreach (var cref in hasImagesBlock.OtherImages)
            {
                result.Add(GetPublicUrl(cref));
            }
            return result;
        }

        public static string GetPublicUrl(this ContentReference contentlink, string language = null)
        {
            if (ContentReference.IsNullOrEmpty(contentlink))
            {
                return string.Empty;
            }

            //// need check for content exist & publish???
            //var content = contentlink.GetContent(language);
            //if (content == null || !PublishedStateAssessor.IsPublished(content))
            //{
            //    return string.Empty;
            //}

            return _urlResolver.Service.GetUrl(contentlink, language);
        }
    }
}
