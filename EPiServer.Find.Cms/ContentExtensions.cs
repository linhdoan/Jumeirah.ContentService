using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Find.Cms
{
    public static class ContentExtensions
    {
        private static readonly bool HasPageTypeBuilder = StaticTypeExtensions.Exists("PageTypeBuilder.PageTypeResolver, PageTypeBuilder");
        private static readonly ILog log = LogManager.GetLogger(typeof(CmsUnifiedSearchSetUp));
        private static readonly Lazy<IAttachmentHelper> AttachmentHelper = new Lazy<IAttachmentHelper>((Func<IAttachmentHelper>)(() =>
        {
            IAttachmentHelper instance = (IAttachmentHelper)null;
            if (!ServiceLocator.Current.TryGetExistingInstance<IAttachmentHelper>(out instance))
                ContentExtensions.log.Error((object)"IAttachmentHelper can not be initiated.");
            return instance;
        }));
        private static readonly Lazy<List<string>> FilterSupportedFileTypes = new Lazy<List<string>>((Func<List<string>>)(() =>
        {
            if (!ObjectExtensions.IsNotNull((object)ContentExtensions.AttachmentHelper.Value))
                return new List<string>();
            else
                return Enumerable.ToList<string>(ContentExtensions.AttachmentHelper.Value.GetSupportedFiletypes());
        }));
        private static readonly Type[] StringPropertyTypes = new Type[3]
    {
      typeof (PropertyXhtmlString),
      typeof (PropertyString),
      typeof (PropertyLongString)
    };

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

        public static string SiteId(this IContent content)
        {
            ObjectExtensions.ValidateNotNullArgument((object)content, "content");
            SiteDefinition definitionForContent = ServiceLocator.Current.GetInstance<SiteDefinitionResolver>().GetDefinitionForContent(content.ContentLink, false, false);
            if (ObjectExtensions.IsNull((object)definitionForContent) && content is IContentMedia)
            {
                IContentRepository instance = ServiceLocator.Current.GetInstance<IContentRepository>();
                IContent content1 = instance.Get<IContent>(content.ParentLink);
                if (content1 is ContentAssetFolder)
                {
                    ContentAssetFolder contentAssetFolder = (ContentAssetFolder)content1;
                    if (ObjectExtensions.IsNotNull((object)contentAssetFolder))
                        return ContentExtensions.SiteId(instance.Get<IContent>(contentAssetFolder.ContentOwnerID));
                }
                else if (content1 is ContentFolder)
                    return ContentExtensions.SiteId(content1);
                return (string)null;
            }
            else if (ObjectExtensions.IsNull((object)definitionForContent) && content is IContentAsset)
            {
                IContentRepository instance = ServiceLocator.Current.GetInstance<IContentRepository>();
                IContentAsset contentAsset = content as IContentAsset;
                if (ObjectExtensions.IsNotNull((object)contentAsset) && ObjectExtensions.IsNotNull((object)contentAsset.ContentOwnerID))
                    return ContentExtensions.SiteId(instance.Get<IContent>(contentAsset.ContentOwnerID));
                else
                    return (string)null;
            }
            else if (ObjectExtensions.IsNull((object)definitionForContent))
                return (string)null;
            else
                return definitionForContent.Id.ToString();
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
                                            bool? nullable = ContentIndexer.Instance.Conventions.ShouldIndexInContentAreaConvention.ShouldIndexInContentArea(x);
                                            if (nullable.HasValue)
                                                return nullable.Value;
                                            else
                                                return false;
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
                                                bool? nullable = ContentIndexer.Instance.Conventions.ShouldIndexInContentAreaConvention.ShouldIndexInContentArea(x);
                                                if (nullable.HasValue)
                                                    return nullable.Value;
                                                else
                                                    return false;
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

        public static string SearchText(this IContentData contentData)
        {
            return string.Join(Environment.NewLine, Enumerable.ToArray<string>(Enumerable.Where<string>(Enumerable.Select<PropertyData, string>((IEnumerable<PropertyData>)Enumerable.ThenBy<PropertyData, int>(Enumerable.OrderByDescending<PropertyData, bool>(ContentExtensions.GetSearchableProperties(contentData), (Func<PropertyData, bool>)(x => Enumerable.Contains<Type>((IEnumerable<Type>)ContentExtensions.StringPropertyTypes, x.GetType()))), (Func<PropertyData, int>)(x => x.FieldOrder)), (Func<PropertyData, string>)(x => ContentExtensions.ToStringAsViewedByAnonymous(x))), (Func<string, bool>)(x => !string.IsNullOrWhiteSpace(x)))));
        }

        public static Attachment SearchAttachment(this IContentMedia contentMedia)
        {
            ObjectExtensions.ValidateNotNullArgument((object)contentMedia, "contentMedia");
            MediaData mediaData = contentMedia as MediaData;
            if (ObjectExtensions.IsNotNull((object)mediaData) && ContentExtensions.FilterSupportedFileTypes.Value.Contains(Path.GetExtension(mediaData.RouteSegment)))
                return (Attachment)null;
            if (ContentIndexer.Instance.Conventions.ShouldIndexAttachmentConvention.ShouldIndexAttachment(contentMedia).HasValue && (ContentIndexer.Instance.Conventions.ShouldIndexAttachmentConvention.ShouldIndexAttachment(contentMedia).Value && ObjectExtensions.IsNotNull((object)contentMedia.BinaryData)))
                return new Attachment((Func<Stream>)(() => contentMedia.BinaryData.OpenRead()));
            else
                return (Attachment)null;
        }

        public static string SearchAttachmentText(this IContentMedia contentMedia)
        {
            ObjectExtensions.ValidateNotNullArgument((object)contentMedia, "contentMedia");
            MediaData mediaData = contentMedia as MediaData;
            if (ObjectExtensions.IsNotNull((object)mediaData) && ContentExtensions.FilterSupportedFileTypes.Value.Contains(Path.GetExtension(mediaData.RouteSegment)))
            {
                if (ObjectExtensions.IsNotNull((object)ContentExtensions.AttachmentHelper.Value))
                {
                    try
                    {
                        StringWriter stringWriter = new StringWriter();
                        ContentExtensions.AttachmentHelper.Value.ExtractFileText(contentMedia, (TextWriter)stringWriter);
                        return stringWriter.ToString();
                    }
                    catch (Exception ex)
                    {
                        ContentExtensions.log.Error((object)"Attachment cannot be parsed. ", ex);
                        return (string)null;
                    }
                }
            }
            return (string)null;
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

        public static string SearchTypeName(this PageData pageData)
        {
            return Text.Translate("/cms/unifiedsearch/typenames/page");
        }

        public static string SearchTypeName(this IContentMedia content)
        {
            return Text.Translate("/cms/unifiedsearch/typenames/file");
        }

        public static string SearchSection(this IContent content)
        {
            ObjectExtensions.ValidateNotNullArgument((object)content, "content");
            SiteDefinition siteDef = ServiceLocator.Current.GetInstance<SiteDefinitionResolver>().GetDefinitionForContent(content.ContentLink, true, true);
            IContent content1 = Enumerable.FirstOrDefault<IContent>(ServiceLocator.Current.GetInstance<IContentRepository>().GetAncestors(content.ContentLink), (Func<IContent, bool>)(x =>
            {
                if (x.ParentLink != (ContentReference)null)
                    return x.ParentLink.CompareToIgnoreWorkID(siteDef.StartPage);
                else
                    return false;
            }));
            if (content1 != null)
                return content1.Name;
            else
                return (string)null;
        }

        public static string SearchSubsection(this IContent content)
        {
            ObjectExtensions.ValidateNotNullArgument((object)content, "content");
            SiteDefinition siteDef = ServiceLocator.Current.GetInstance<SiteDefinitionResolver>().GetDefinitionForContent(content.ContentLink, true, true);
            IEnumerable<IContent> ancestors = ServiceLocator.Current.GetInstance<IContentRepository>().GetAncestors(content.ContentLink);
            IContent ancestorUnderStartPage = Enumerable.FirstOrDefault<IContent>(ancestors, (Func<IContent, bool>)(x =>
            {
                if (x.ParentLink != (ContentReference)null)
                    return x.ParentLink.CompareToIgnoreWorkID(siteDef.StartPage);
                else
                    return false;
            }));
            if (ancestorUnderStartPage != null)
            {
                IContent content1 = Enumerable.FirstOrDefault<IContent>(ancestors, (Func<IContent, bool>)(x =>
                {
                    if (x.ParentLink != (ContentReference)null)
                        return x.ParentLink.CompareToIgnoreWorkID(ancestorUnderStartPage.ContentLink);
                    else
                        return false;
                }));
                if (content1 != null)
                    return content1.Name;
            }
            return (string)null;
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
            if (!(content is PageData) || !ContentExtensions.HasPageTypeBuilder)
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

        public static Dictionary<string, LanguagePublicationStatus> PublishedInLanguage(this IContent content)
        {
            ObjectExtensions.ValidateNotNullArgument((object)content, "content");
            ILocalizable localizable = content as ILocalizable;
            IVersionable versionable1 = content as IVersionable;
            if (localizable == null || localizable.Language == null || versionable1 == null)
                return (Dictionary<string, LanguagePublicationStatus>)null;
            if (versionable1.Status != VersionStatus.Published)
                return (Dictionary<string, LanguagePublicationStatus>)null;
            Dictionary<string, LanguagePublicationStatus> languages = new Dictionary<string, LanguagePublicationStatus>();
            IEnumerable<ContentLanguageSetting> source1 = ServiceLocator.Current.GetInstance<IContentLanguageSettingsHandler>().Get(content.ContentLink);
            ContentLanguageSetting contentLanguageSetting1 = Enumerable.FirstOrDefault<ContentLanguageSetting>(source1, (Func<ContentLanguageSetting, bool>)(x => x.LanguageBranch == localizable.Language.Name));
            if (contentLanguageSetting1 == null || string.IsNullOrEmpty(contentLanguageSetting1.ReplacementLanguageBranch) || contentLanguageSetting1.ReplacementLanguageBranch == localizable.Language.Name)
            {
                LanguagePublicationStatus publicationStatus = new LanguagePublicationStatus()
                {
                    StartPublish = new DateTime?(DateExtensions.NormalizeToMinutes(versionable1.StartPublish.Value)),
                    StopPublish = versionable1.StopPublish
                };
                languages.Add(localizable.Language.Name, publicationStatus);
            }
            IEnumerable<ContentLanguageSetting> source2 = Enumerable.Where<ContentLanguageSetting>(source1, (Func<ContentLanguageSetting, bool>)(x => x.LanguageBranch != localizable.Language.Name));
            foreach (ContentLanguageSetting contentLanguageSetting2 in Enumerable.Where<ContentLanguageSetting>(Enumerable.Where<ContentLanguageSetting>(source2, (Func<ContentLanguageSetting, bool>)(x =>
            {
                if (x.ReplacementLanguageBranch != null)
                    return x.ReplacementLanguageBranch == localizable.Language.Name;
                else
                    return false;
            })), (Func<ContentLanguageSetting, bool>)(x => !languages.ContainsKey(x.LanguageBranch))))
            {
                LanguagePublicationStatus publicationStatus = new LanguagePublicationStatus()
                {
                    StartPublish = new DateTime?(DateExtensions.NormalizeToMinutes(versionable1.StartPublish.Value)),
                    StopPublish = versionable1.StopPublish
                };
                languages.Add(contentLanguageSetting2.LanguageBranch, publicationStatus);
            }
            foreach (ContentLanguageSetting contentLanguageSetting2 in source2)
            {
                if (contentLanguageSetting2.LanguageBranchFallback != null)
                {
                    IContentRepository instance = ServiceLocator.Current.GetInstance<IContentRepository>();
                    IContent content1 = (IContent)instance.Get<IContent>(content.ContentLink.CreateReferenceWithoutVersion(), (LoaderOptions)new LanguageSelector(contentLanguageSetting2.LanguageBranch));
                    DateTime? nullable1 = versionable1.StartPublish;
                    DateTime? nullable2 = versionable1.StopPublish;
                    if (content1 != null)
                    {
                        IVersionable versionable2 = content1 as IVersionable;
                        if (versionable2 != null)
                        {
                            if (versionable2.Status == VersionStatus.Published)
                            {
                                DateTime? startPublish1 = versionable2.StartPublish;
                                DateTime now1 = DateTime.Now;
                                if ((startPublish1.HasValue ? (startPublish1.GetValueOrDefault() <= now1 ? 1 : 0) : 0) != 0)
                                {
                                    if (versionable2.StopPublish.HasValue)
                                    {
                                        DateTime? stopPublish1 = versionable2.StopPublish;
                                        DateTime now2 = DateTime.Now;
                                        if ((stopPublish1.HasValue ? (stopPublish1.GetValueOrDefault() >= now2 ? 1 : 0) : 0) != 0)
                                        {
                                            DateTime? nullable3 = nullable1;
                                            DateTime? stopPublish2 = versionable2.StopPublish;
                                            if ((nullable3.HasValue & stopPublish2.HasValue ? (nullable3.GetValueOrDefault() < stopPublish2.GetValueOrDefault() ? 1 : 0) : 0) != 0)
                                                nullable1 = versionable2.StopPublish;
                                        }
                                    }
                                    else
                                        continue;
                                }
                                DateTime? startPublish2 = versionable2.StartPublish;
                                DateTime now3 = DateTime.Now;
                                if ((startPublish2.HasValue ? (startPublish2.GetValueOrDefault() > now3 ? 1 : 0) : 0) != 0)
                                {
                                    DateTime? startPublish3 = versionable2.StartPublish;
                                    DateTime? nullable3 = nullable2;
                                    if ((startPublish3.HasValue & nullable3.HasValue ? (startPublish3.GetValueOrDefault() < nullable3.GetValueOrDefault() ? 1 : 0) : 0) != 0 || !nullable2.HasValue)
                                        nullable2 = versionable2.StartPublish;
                                }
                            }
                        }
                        else
                            continue;
                    }
                    foreach (string languageBranch in contentLanguageSetting2.LanguageBranchFallback)
                    {
                        if (languageBranch == localizable.Language.Name && !languages.ContainsKey(contentLanguageSetting2.LanguageBranch))
                        {
                            LanguagePublicationStatus publicationStatus = new LanguagePublicationStatus()
                            {
                                StartPublish = new DateTime?(DateExtensions.NormalizeToMinutes(nullable1.Value)),
                                StopPublish = nullable2
                            };
                            languages.Add(contentLanguageSetting2.LanguageBranch, publicationStatus);
                            break;
                        }
                        else
                        {
                            IContent content2 = (IContent)instance.Get<IContent>(content.ContentLink.CreateReferenceWithoutVersion(), (LoaderOptions)new LanguageSelector(languageBranch));
                            if (content2 != null)
                            {
                                IVersionable versionable2 = content2 as IVersionable;
                                if (versionable2 == null || versionable2.Status == VersionStatus.Published)
                                    break;
                            }
                        }
                    }
                }
            }
            return languages;
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
