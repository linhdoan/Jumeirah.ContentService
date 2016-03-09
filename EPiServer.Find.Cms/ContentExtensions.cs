using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Find.Helpers;
using EPiServer.Framework.Web;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Find.Cms
{
    public static class ContentExtensions
    {
        public static string CommonName(this IContent content)
        {
            ObjectExtensions.ValidateNotNullArgument((object)content, "content");
            return content.Name;
        }

        public static string CommonType(this IContent content)
        {
            ObjectExtensions.ValidateNotNullArgument((object)content, "content");
            if (content.GetType().Assembly.GetName().Name == "DynamicProxyGenAssembly2")
                return content.GetType().BaseType.FullName;
            else
                return content.GetType().FullName;
        }

        public static string CommonTypeShortName(this IContent content)
        {
            ObjectExtensions.ValidateNotNullArgument((object)content, "content");
            if (content.GetType().Assembly.GetName().Name == "DynamicProxyGenAssembly2")
                return content.GetType().BaseType.Name;
            else
                return content.GetType().Name;
        }

        public static string CommonTypeDisplayName(this IContent content)
        {
            ObjectExtensions.ValidateNotNullArgument((object)content, "content");
            return ContentExtensions.ContentTypeName(content);
        }

        public static string GetIndexId(this IContent content)
        {
            ILocalizable localizable = content as ILocalizable;
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0}_{1}{2}", (object)DataFactory.Instance.GetProvider(content.ContentLink).ProviderKey, (object)content.ContentGuid, localizable != null ? (object)("_" + localizable.Language.Name) : (object)string.Empty);
        }

        public static DateTime GetTimestamp(this IContent content)
        {
            ObjectExtensions.ValidateNotNullArgument((object)content, "content");
            return DateTime.Now;
        }

        public static IEnumerable<string> RolesWithReadAccess(this IContentSecurable contentSecurable)
        {
            return Enumerable.Union<string>(ContentExtensions.SecurityEntityWithReadAccess(contentSecurable, SecurityEntityType.Role), ContentExtensions.SecurityEntityWithReadAccess(contentSecurable, SecurityEntityType.VisitorGroup));
        }

        public static IEnumerable<string> UsersWithReadAccess(this IContentSecurable contentSecurable)
        {
            return ContentExtensions.SecurityEntityWithReadAccess(contentSecurable, SecurityEntityType.User);
        }

        private static IEnumerable<string> SecurityEntityWithReadAccess(this IContentSecurable contentSecurable, SecurityEntityType securityEntityType)
        {
            ObjectExtensions.ValidateNotNullArgument((object)contentSecurable, "contentSecurable");
            return (IEnumerable<string>)Enumerable.ToArray<string>(Enumerable.Select<AccessControlEntry, string>(Enumerable.Where<AccessControlEntry>(contentSecurable.GetContentSecurityDescriptor().Entries, (Func<AccessControlEntry, bool>)(x =>
            {
                if (x.EntityType == securityEntityType)
                    return (x.Access & AccessLevel.Read) == AccessLevel.Read;
                else
                    return false;
            })), (Func<AccessControlEntry, string>)(x => x.Name)));
        }

        public static VersionStatus Status(this IVersionable content)
        {
            if (content is IContentMedia)
            {
                IContent assetOwner = ServiceLocator.Current.GetInstance<ContentAssetHelper>().GetAssetOwner((content as IContent).ContentLink);
                if (ObjectExtensions.IsNotNull((object)assetOwner) && assetOwner is IVersionable)
                    return (assetOwner as IVersionable).Status;
            }
            return content.Status;
        }

        public static string ContentTypeName(this IContent content)
        {
            ObjectExtensions.ValidateNotNullArgument((object)content, "content");
            ContentType contentType = ServiceLocator.Current.GetInstance<IContentTypeRepository>().Load(content.ContentTypeID);
            if (contentType != (ContentType)null)
                return contentType.LocalizedName;
            else
                return (string)null;
        }

        public static IEnumerable<string> Ancestors(this IContent content)
        {
            ObjectExtensions.ValidateNotNullArgument((object)content, "content");
            return Enumerable.Select<IContent, string>(DataFactory.Instance.GetAncestors(content.ContentLink.CreateReferenceWithoutVersion()), (Func<IContent, string>)(x => x.ContentLink.CreateReferenceWithoutVersion().ToString()));
        }

        public static DateTime? StartPublishedNormalized(this IVersionable versionable)
        {
            ObjectExtensions.ValidateNotNullArgument((object)versionable, "versionable");
            if (versionable.StartPublish.HasValue)
                return new DateTime?(DateExtensions.NormalizeToMinutes(versionable.StartPublish.Value));
            else
                return new DateTime?();
        }

        public static IEnumerable<PropertyData> GetSearchableProperties(this IContentData contentData)
        {
            IPropertyDefinitionRepository instance = ServiceLocator.Current.GetInstance<IPropertyDefinitionRepository>();
            return ContentExtensions.GetSearchableProperties(contentData, instance, (IEnumerable<ContentReference>)new List<ContentReference>());
        }

        internal static IEnumerable<PropertyData> GetSearchableProperties(this IContentData contentData, IPropertyDefinitionRepository propertyDefinitionRepository, IEnumerable<ContentReference> referenceChain)
        {
            if (contentData != null)
            {
                IContent contentDataAsIContent = contentData as IContent;
                if (contentDataAsIContent == null || !Enumerable.Any<ContentReference>(Enumerable.Where<ContentReference>(referenceChain, (Func<ContentReference, bool>)(x => x.CompareToIgnoreWorkID(contentDataAsIContent.ContentLink)))))
                {
                    foreach (PropertyData propertyData1 in contentData.Property)
                    {
                        if (propertyData1.IsPropertyData)
                        {
                            PropertyInfo typeProperty = RuntimeModelExtensions.GetOriginalType((object)contentData).GetProperty(propertyData1.Name);
                            if (!(typeProperty == (PropertyInfo)null) && CustomAttributeExtensions.GetCustomAttribute<JsonIgnoreAttribute>((MemberInfo)typeProperty) == null)
                            {
                                PropertyContentArea contentAreaProperty = propertyData1 as PropertyContentArea;
                                PropertyDefinition propertyDefinition = propertyDefinitionRepository.Load(propertyData1.PropertyDefinitionID);
                                if (propertyDefinition.Searchable && contentAreaProperty == null)
                                    yield return propertyData1;
                                IPropertyBlock blockProperty = propertyData1 as IPropertyBlock;
                                if (blockProperty != null)
                                {
                                    BlockData block = blockProperty.Block;
                                    IPropertyDefinitionRepository propertyDefinitionRepository1 = propertyDefinitionRepository;
                                    IEnumerable<ContentReference> referenceChain1;
                                    if (contentDataAsIContent == null)
                                        referenceChain1 = referenceChain;
                                    else
                                        referenceChain1 = Enumerable.Concat<ContentReference>(referenceChain, (IEnumerable<ContentReference>)new ContentReference[1]
                    {
                      contentDataAsIContent.ContentLink
                    });
                                    foreach (PropertyData propertyData2 in ContentExtensions.GetSearchableProperties((IContentData)block, propertyDefinitionRepository1, referenceChain1))
                                        yield return propertyData2;
                                }
                                if (contentAreaProperty != null)
                                {
                                    ContentArea contentArea = contentAreaProperty.Value as ContentArea;
                                    if (contentArea != null)
                                    {
                                        foreach (PropertyData propertyData2 in Enumerable.SelectMany<IContent, PropertyData>(Enumerable.Where<IContent>(contentArea.Contents, (Func<IContent, bool>)(x =>
                                        {
                                            return true;
                                        })), (Func<IContent, IEnumerable<PropertyData>>)(x =>
                                        {
                                            IContent content = x;
                                            IPropertyDefinitionRepository propertyDefinitionRepository1 = propertyDefinitionRepository;
                                            IEnumerable<ContentReference> referenceChain1;
                                            if (contentDataAsIContent == null)
                                                referenceChain1 = referenceChain;
                                            else
                                                referenceChain1 = Enumerable.Concat<ContentReference>(referenceChain, (IEnumerable<ContentReference>)new ContentReference[1]
                        {
                          contentDataAsIContent.ContentLink
                        });
                                            return ContentExtensions.GetSearchableProperties((IContentData)content, propertyDefinitionRepository1, referenceChain1);
                                        })))
                                            yield return propertyData2;
                                        if (!propertyDefinition.ExistsOnModel)
                                        {
                                            foreach (IContent content in Enumerable.Where<IContent>(contentArea.Contents, (Func<IContent, bool>)(x =>
                                            {
                                                return true;
                                            })))
                                            {
                                                foreach (PropertyInfo propertyInfo in typeof(IContent).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                                                    yield return (PropertyData)new PropertyString(propertyInfo.GetValue((object)content, (object[])null).ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static string SearchTitle(this IContent content)
        {
            ObjectExtensions.ValidateNotNullArgument((object)content, "content");
            return content.Name;
        }

        public static DateTime? SearchPublishDate(this IVersionable content)
        {
            ObjectExtensions.ValidateNotNullArgument((object)content, "content");
            return content.StartPublish;
        }

        public static DateTime? SearchUpdateDate(this IChangeTrackable content)
        {
            ObjectExtensions.ValidateNotNullArgument((object)content, "content");
            return new DateTime?(content.Changed);
        }

        private static readonly Type[] StringPropertyTypes = new Type[3]
        {
          typeof (PropertyXhtmlString),
          typeof (PropertyString),
          typeof (PropertyLongString)
        };

        public static string SearchText(this IContentData contentData)
        {
            return string.Join(Environment.NewLine, Enumerable.ToArray<string>(Enumerable.Where<string>(Enumerable.Select<PropertyData, string>((IEnumerable<PropertyData>)Enumerable.ThenBy<PropertyData, int>(Enumerable.OrderByDescending<PropertyData, bool>(ContentExtensions.GetSearchableProperties(contentData), (Func<PropertyData, bool>)(x => Enumerable.Contains<Type>((IEnumerable<Type>)ContentExtensions.StringPropertyTypes, x.GetType()))), (Func<PropertyData, int>)(x => x.FieldOrder)), (Func<PropertyData, string>)(x => ContentExtensions.ToStringAsViewedByAnonymous(x))), (Func<string, bool>)(x => !string.IsNullOrWhiteSpace(x)))));
        }

        public static string SearchFilename(this IContentMedia contentMedia)
        {
            ObjectExtensions.ValidateNotNullArgument((object)contentMedia, "contentMedia");
            MediaData mediaData = contentMedia as MediaData;
            if (!ObjectExtensions.IsNotNull((object)mediaData))
                return (string)null;
            try
            {
                return Path.GetFileName(mediaData.RouteSegment);
            }
            catch
            {
                return (string)null;
            }
        }

        public static string SearchFileExtension(this IContentMedia contentMedia)
        {
            ObjectExtensions.ValidateNotNullArgument((object)contentMedia, "contentMedia");
            MediaData mediaData = contentMedia as MediaData;
            if (!ObjectExtensions.IsNotNull((object)mediaData))
                return (string)null;
            try
            {
                return Path.GetExtension(mediaData.RouteSegment).Replace(".", string.Empty);
            }
            catch
            {
                return (string)null;
            }
        }

        public static IEnumerable<string> SearchCategories(this ICategorizable categorizable)
        {
            ObjectExtensions.ValidateNotNullArgument((object)categorizable, "categorizable");
            if (ObjectExtensions.IsNull((object)categorizable.Category))
                return (IEnumerable<string>)null;
            else
                return (IEnumerable<string>)Enumerable.ToList<string>(Enumerable.Select<int, string>((IEnumerable<int>)categorizable.Category, (Func<int, string>)(x => Category.Find(x).Name)));
        }

        private static string ToStringAsViewedByAnonymous(this PropertyData propertyData)
        {
            if (propertyData.IsNull)
                return string.Empty;
            XhtmlString xhtmlString = propertyData.Value as XhtmlString;
            if (xhtmlString != null)
                return EPiServer.Find.Helpers.Text.StringExtensions.StripHtml(XhtmlStringExtensions.AsViewedByAnonymous(xhtmlString));
            else
                return ((object)propertyData).ToString();
        }

        public static IContent TryAsTyped(this IContent content)
        {
            ObjectExtensions.ValidateNotNullArgument((object)content, "content");
            if (!(content is PageData))
                return content;
            Type type = Type.GetType("PageTypeBuilder.PageTypeResolver, PageTypeBuilder");
            object target = type.InvokeMember("Instance", BindingFlags.GetProperty, (Binder)null, (object)type, (object[])null);
            return (IContent)type.InvokeMember("ConvertToTyped", BindingFlags.InvokeMethod, (Binder)null, target, (object[])new IContent[1]
      {
        content
      });
        }

        internal static string LinkURL(this PageData page)
        {
            ObjectExtensions.ValidateNotNullArgument((object)page, "page");
            string url = page["PageLinkURL"] as string;
            if (!string.IsNullOrWhiteSpace(url))
                url = UriSupport.AddLanguageSelection(url, page.LanguageBranch);
            return url ?? page.LinkURL;
        }

        public static bool HasTemplate(this IContent content)
        {
            ObjectExtensions.ValidateNotNullArgument((object)content, "content");
            IContentData contentData = (IContentData)content;
            if (contentData != null)
                return Enumerable.Any<TemplateModel>(ServiceLocator.Current.GetInstance<TemplateModelRepository>().List(RuntimeModelExtensions.GetOriginalType((object)contentData)), (Func<TemplateModel, bool>)(x => TemplateTypeCategoriesExtensions.IsCategory(x.TemplateTypeCategory, TemplateTypeCategories.Page)));
            else
                return false;
        }
    }
}
