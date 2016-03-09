using EPiServer.Core;
using EPiServer.Data.Entity;
using EPiServer.DataAbstraction;
using EPiServer.DataAbstraction.RuntimeModel;
using EPiServer.Find.ClientConventions;
using EPiServer.Find.Cms.Json;
using EPiServer.Find.Helpers;
using EPiServer.Security;
using EPiServer.SpecializedProperties;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Find.Cms
{
    public static class CmsClientConventions
    {
        public static void ApplyCmsConventions(IClient client)
        {
            ClientConventions.ConventionsExtensions.ForInstancesOf<object>(client.Conventions).FieldsOfType<ContentReference>().ConvertBeforeSerializing((Func<ContentReference, ContentReference>)(x =>
            {
                if (ObjectExtensions.IsNull((object)x))
                    return (ContentReference)null;
                else
                    return (ContentReference)new IndexableContentReference(x);
            }));
            ClientConventions.ConventionsExtensions.ForInstancesOf<object>(client.Conventions).FieldsOfType<IEnumerable<ContentReference>>().ConvertBeforeSerializing((Func<IEnumerable<ContentReference>, IEnumerable<ContentReference>>)(x =>
            {
                if (ObjectExtensions.IsNull((object)x))
                    return (IEnumerable<ContentReference>)null;
                List<ContentReference> list = new List<ContentReference>();
                foreach (IndexableContentReference contentReference in Enumerable.Select<ContentReference, IndexableContentReference>(Enumerable.OfType<ContentReference>((IEnumerable)x), (Func<ContentReference, IndexableContentReference>)(item => new IndexableContentReference(item))))
                    list.Add((ContentReference)contentReference);
                return (IEnumerable<ContentReference>)list;
            }));
            ClientConventions.ConventionsExtensions.ForInstancesOf<object>(client.Conventions).FieldsOfType<PageReference>().ConvertBeforeSerializing((Func<PageReference, PageReference>)(x =>
            {
                if (ObjectExtensions.IsNull((object)x))
                    return (PageReference)null;
                else
                    return (PageReference)new IndexablePageReference(x);
            }));
            ClientConventions.ConventionsExtensions.ForInstancesOf<PageReference>(client.Conventions).ExcludeField((Expression<Func<PageReference, object>>)(x => (object)x.GetPublishedOrLatest)).ExcludeField((Expression<Func<PageReference, object>>)(x => (object)x.WorkID));
            ClientConventions.ConventionsExtensions.ForInstancesOf<object>(client.Conventions).FieldsOfType<PageReferenceCollection>().ConvertBeforeSerializing((Func<PageReferenceCollection, PageReferenceCollection>)(x =>
            {
                if (ObjectExtensions.IsNull((object)x))
                    return (PageReferenceCollection)null;
                PageReferenceCollection referenceCollection = new PageReferenceCollection();
                foreach (IndexablePageReference indexablePageReference in Enumerable.Select<PageReference, IndexablePageReference>(Enumerable.OfType<PageReference>((IEnumerable)x), (Func<PageReference, IndexablePageReference>)(item => new IndexablePageReference(item))))
                    referenceCollection.Add((PageReference)indexablePageReference);
                return referenceCollection;
            }));
            ClientConventions.ConventionsExtensions.ForInstancesOf<XhtmlString>(client.Conventions).ExcludeField((Expression<Func<XhtmlString, object>>)(x => x.Fragments)).ExcludeField((Expression<Func<XhtmlString, object>>)(x => (object)x.FragmentParser)).ExcludeField((Expression<Func<XhtmlString, object>>)(x => (object)x.IsReadOnly)).ExcludeField((Expression<Func<XhtmlString, object>>)(x => (object)x.ParserMode)).UseJsonContract((Func<Type, JsonContract>)(type => (JsonContract)client.Conventions.ContractResolver.CreateObjectContractPublic(type)));
            ClientConventions.ConventionsExtensions.ForType<XhtmlString>(client.Conventions).IncludeField<string>((Expression<Func<XhtmlString, string>>)(x => XhtmlStringExtensions.AsViewedByAnonymous(x)));
            ClientConventions.FieldConventionBuilderExtensions.StripHtml(ConventionsExtensions.ForInstancesOf<XhtmlString>(client.Conventions).Field<string>((Expression<Func<XhtmlString, string>>)(x => XhtmlStringExtensions.AsViewedByAnonymous(x))));
            client.Conventions.CustomizeProjectionHandling = (System.Action<IList<IProjectionProcessor>>)(x => x.Insert(0, (IProjectionProcessor)new XhtmlStringProjectionProcessor()));
            ClientConventions.ConventionsExtensions.ForInstancesOf<IContentMixin>(client.Conventions).ExcludeField((Expression<Func<IContentMixin, object>>)(x => x.MixinInstance)).ExcludeFieldMatching((Func<JsonProperty, bool>)(property => property.PropertyName.StartsWith("__mixin_")));
            ClientConventions.ConventionsExtensions.ForInstancesOf<IReadOnly>(client.Conventions).ExcludeField((Expression<Func<IReadOnly, object>>)(x => (object)x.IsReadOnly));
            Expression<Func<IModifiedTrackable, object>> expression1 = (Expression<Func<IModifiedTrackable, object>>)(x => (object)x.IsModified);
            string explicityIsModifiedPropertyName = typeof(IModifiedTrackable).Name + "." + client.Conventions.FieldNameConvention.GetFieldName((Expression)expression1);
            ConventionsExtensions.ForInstancesOf<IModifiedTrackable>(client.Conventions).ExcludeField((Expression<Func<IModifiedTrackable, object>>)(x => (object)x.IsModified)).ExcludeFieldMatching((Func<JsonProperty, bool>)(property => property.PropertyName == explicityIsModifiedPropertyName));
            //((TypeConventionBuilder)ConventionsExtensions.ForInstancesOf<IContent>(client.Conventions).IncludeField<ContentReference>((Expression<Func<IContent, ContentReference>>)(x => x.ContentLink)).IncludeField<DateTime>((Expression<Func<IContent, DateTime>>)(x => ContentExtensions.GetTimestamp(x))).IncludeField<string>((Expression<Func<IContent, string>>)(x => ContentExtensions.SiteId(x))).IncludeField<string>((Expression<Func<IContent, string>>)(x => ContentExtensions.ContentTypeName(x))).IncludeField<IEnumerable<string>>((Expression<Func<IContent, IEnumerable<string>>>)(x => ContentExtensions.Ancestors(x))).IncludeField<Dictionary<string, LanguagePublicationStatus>>((Expression<Func<IContent, Dictionary<string, LanguagePublicationStatus>>>)(x => ContentExtensions.PublishedInLanguage(x))).IncludeField<string>((Expression<Func<IContent, string>>)(x => ContentExtensions.CommonName(x))).IncludeField<string>((Expression<Func<IContent, string>>)(x => ContentExtensions.CommonType(x))).IncludeField<string>((Expression<Func<IContent, string>>)(x => ContentExtensions.CommonTypeShortName(x))).IncludeField<string>((Expression<Func<IContent, string>>)(x => ContentExtensions.CommonTypeDisplayName(x))).IncludeField<string>((Expression<Func<IContent, string>>)(x => ContentExtensions.SearchTitle(x))).IncludeField<string>((Expression<Func<IContent, string>>)(x => ContentExtensions.SearchText(x))).IncludeField<string>((Expression<Func<IContent, string>>)(x => ContentExtensions.SearchSection(x))).IncludeField<string>((Expression<Func<IContent, string>>)(x => ContentExtensions.SearchSubsection(x))).IncludeField<bool>((Expression<Func<IContent, bool>>)(x => ContentExtensions.HasTemplate(x))).IdIs((Func<IContent, DocumentId>)(x => (DocumentId)ContentExtensions.GetIndexId(x)))).ExcludeField("__interceptors").ExcludeFieldMatching((Func<JsonProperty, bool>)(prop => typeof(XForm).IsAssignableFrom(prop.PropertyType))).ExcludeFieldMatching((Func<JsonProperty, bool>)(prop => typeof(PageType).IsAssignableFrom(prop.PropertyType))).ExcludeFieldMatching((Func<JsonProperty, bool>)(prop => typeof(Url).IsAssignableFrom(prop.PropertyType))).ExcludeFieldMatching((Func<JsonProperty, bool>)(prop => typeof(LinkItemCollection).IsAssignableFrom(prop.PropertyType)));
            ConventionsExtensions.ForInstancesOf<IContent>(client.Conventions).Field<string>((Expression<Func<IContent, string>>)(x => ContentExtensions.CommonName(x))).Modify((System.Action<JsonProperty>)(x => x.PropertyName = "_Name"));
            ConventionsExtensions.ForInstancesOf<IContent>(client.Conventions).Field<string>((Expression<Func<IContent, string>>)(x => ContentExtensions.CommonType(x))).Modify((System.Action<JsonProperty>)(x => x.PropertyName = "_Type"));
            ConventionsExtensions.ForInstancesOf<IContent>(client.Conventions).Field<string>((Expression<Func<IContent, string>>)(x => ContentExtensions.CommonTypeShortName(x))).Modify((System.Action<JsonProperty>)(x => x.PropertyName = "_TypeShortName"));
            ConventionsExtensions.ForInstancesOf<IContent>(client.Conventions).Field<string>((Expression<Func<IContent, string>>)(x => ContentExtensions.CommonTypeDisplayName(x))).Modify((System.Action<JsonProperty>)(x => x.PropertyName = "_TypeDisplayName"));
            ConventionsExtensions.ForInstancesOf<ILocalizable>(client.Conventions).ExcludeField((Expression<Func<ILocalizable, object>>)(x => x.Language)).IncludeField<string>((Expression<Func<ILocalizable, string>>)(x => x.Language.Name)).IncludeField<string>((Expression<Func<ILocalizable, string>>)(x => x.MasterLanguage.Name)).IncludeField<IEnumerable<string>>((Expression<Func<ILocalizable, IEnumerable<string>>>)(x => ILocalizableExtensions.ExistingLanguages(x)));
            ConventionsExtensions.ForInstancesOf<IContentSecurable>(client.Conventions).IncludeField<IEnumerable<string>>((Expression<Func<IContentSecurable, IEnumerable<string>>>)(x => ContentExtensions.RolesWithReadAccess(x))).IncludeField<IEnumerable<string>>((Expression<Func<IContentSecurable, IEnumerable<string>>>)(x => ContentExtensions.UsersWithReadAccess(x)));
            ConventionsExtensions.ForInstancesOf<IVersionable>(client.Conventions).ExcludeField((Expression<Func<IVersionable, object>>)(x => (object)x.Status)).IncludeField<VersionStatus>((Expression<Func<IVersionable, VersionStatus>>)(x => ContentExtensions.Status(x))).IncludeField<DateTime?>((Expression<Func<IVersionable, DateTime?>>)(x => ContentExtensions.StartPublishedNormalized(x))).IncludeField<DateTime?>((Expression<Func<IVersionable, DateTime?>>)(x => ContentExtensions.SearchPublishDate(x)));
            ConventionsExtensions.ForInstancesOf<IChangeTrackable>(client.Conventions).IncludeField<DateTime?>((Expression<Func<IChangeTrackable, DateTime?>>)(x => ContentExtensions.SearchUpdateDate(x)));
            Expression<Func<IContentData, object>> expression2 = (Expression<Func<IContentData, object>>)(x => (object)(x.Property == (object)null || !Enumerable.Any<PropertyData>(x.Property)));
            string name = typeof(IContentData).Name + "." + client.Conventions.FieldNameConvention.GetFieldName((Expression)expression2);
            ((TypeConventionBuilder)ConventionsExtensions.ForInstancesOf<IContentData>(client.Conventions).ExcludeField((Expression<Func<IContentData, object>>)(x => x.Property))).ExcludeField("Property").ExcludeField(name).ExcludeField("__interceptors").ExcludeFieldMatching((Func<JsonProperty, bool>)(prop => typeof(PageType).IsAssignableFrom(prop.PropertyType))).ExcludeFieldMatching((Func<JsonProperty, bool>)(prop => typeof(Url).IsAssignableFrom(prop.PropertyType))).ExcludeFieldMatching((Func<JsonProperty, bool>)(prop => typeof(LinkItemCollection).IsAssignableFrom(prop.PropertyType)));
            ConventionsExtensions.ForInstancesOf<PageData>(client.Conventions).ExcludeField((Expression<Func<PageData, object>>)(x => x.ACL)).IncludeField<DateTime?>((Expression<Func<PageData, DateTime?>>)(x => ContentExtensions.StartPublishedNormalized(x))).IncludeField<IEnumerable<string>>((Expression<Func<PageData, IEnumerable<string>>>)(x => ContentExtensions.SearchCategories(x)));
            ConventionsExtensions.ForInstancesOf<ContentReference>(client.Conventions).UseJsonContract((Func<Type, JsonContract>)(type => (JsonContract)client.Conventions.ContractResolver.CreateObjectContractPublic(type)));
            ConventionsExtensions.ForInstancesOf<Url>(client.Conventions).ExcludeField((Expression<Func<Url, object>>)(x => x.Encoding)).ExcludeField((Expression<Func<Url, object>>)(x => x.QueryCollection)).ExcludeField((Expression<Func<Url, object>>)(x => x.Uri)).ModifyContract((System.Action<JsonObjectContract>)(x => x.Converter = (JsonConverter)new FindUrlConverter()));
            ConventionsExtensions.ForInstancesOf<ContentArea>(client.Conventions).ExcludeField((Expression<Func<ContentArea, object>>)(x => x.ContentFragments)).ExcludeField((Expression<Func<ContentArea, object>>)(x => x.FilteredContents)).ExcludeField((Expression<Func<ContentArea, object>>)(x => x.FilteredContentFragments)).ExcludeField((Expression<Func<ContentArea, object>>)(x => (object)x.FragmentFactory)).ExcludeField((Expression<Func<ContentArea, object>>)(x => x.Items)).ExcludeField((Expression<Func<ContentArea, object>>)(x => x.FilteredItems)).ExcludeField((Expression<Func<ContentArea, object>>)(x => (object)x.MarkupGeneratorFactory)).ExcludeField((Expression<Func<ContentArea, object>>)(x => x.Contents));
            ConventionsExtensions.ForInstancesOf<IContentMedia>(client.Conventions).ExcludeField((Expression<Func<IContentMedia, object>>)(x => x.BinaryData)).ExcludeField((Expression<Func<IContentMedia, object>>)(x => x.Thumbnail)).IncludeField<string>((Expression<Func<IContentMedia, string>>)(x => ContentExtensions.SearchFilename(x))).IncludeField<string>((Expression<Func<IContentMedia, string>>)(x => ContentExtensions.SearchFileExtension(x)));
        }
    }
}
