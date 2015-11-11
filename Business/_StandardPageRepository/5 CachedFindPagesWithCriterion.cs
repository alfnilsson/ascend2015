using System.Threading;
using Ascend2015.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using EPiServer.Framework;
using EPiServer.Framework.Cache;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace Ascend2015.Business.StandardPageRepository
{
    //[ServiceConfiguration(typeof(IStandardPageRepository))]
    public class CachedFindPagesWithCriterion : IStandardPageRepository
    {
        private readonly SearchPages _searchPages;
        private readonly PageTypeRepository _pageTypeRepository;
        private readonly ISynchronizedObjectInstanceCache _synchronizedObjectInstanceCache;

        public CachedFindPagesWithCriterion(SearchPages searchPages, PageTypeRepository pageTypeRepository, ISynchronizedObjectInstanceCache synchronizedObjectInstanceCache)
        {
            _searchPages = searchPages;
            _pageTypeRepository = pageTypeRepository;
            _synchronizedObjectInstanceCache = synchronizedObjectInstanceCache;
        }

        public static string GetCacheKey(PageReference pageLink)
        {
            string cacheKey = string.Format("CachedFindPagesWithCriterion-{0}", pageLink);
            return cacheKey;
        }

        public PageDataCollection List(PageReference pageLink)
            {
            string cacheKey = GetCacheKey(pageLink.ToReferenceWithoutVersion());

            var standardPages = _synchronizedObjectInstanceCache.Get(cacheKey) as PageDataCollection;
            if (standardPages == null)
            {
                string pageTypeId = _pageTypeRepository.Load<StandardPage>()?.ID.ToString();
                if (pageTypeId == null)
                {
                    return new PageDataCollection();
                }

                PropertyCriteria productPageTypeCriterion = new PropertyCriteria
                {
                    Name = "PageTypeID",
                    Type = PropertyDataType.PageType,
                    Value = pageTypeId,
                    Condition = CompareCondition.Equal,
                    Required = true
                };

                var criteria = new PropertyCriteriaCollection
                {
                    productPageTypeCriterion
                };

                standardPages = _searchPages.FindPagesWithCriteria(pageLink, criteria);

                // Lots of things can happen while fetching the cache
                Thread.Sleep(1000);

                // Actually caching the ContentData is not a good practice. Caching the ContentReference is  better
                _synchronizedObjectInstanceCache.Insert(cacheKey, standardPages, CacheEvictionPolicy.Empty);
            }

            return standardPages;
        }
    }

    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class CachingInitializationModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            return;
            IContentEvents contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.PublishedContent += ContentEventsOnPublishedContent;
        }

        private void ContentEventsOnPublishedContent(object sender, ContentEventArgs contentEventArgs)
        {
            var productPage = contentEventArgs.Content as ProductPage;
            if (productPage == null)
            {
                return;
            }

            string cacheKey = CachedFindPagesWithCriterion.GetCacheKey(productPage.PageLink);

            ISynchronizedObjectInstanceCache synchronizedObjectInstanceCache = ServiceLocator.Current.GetInstance<ISynchronizedObjectInstanceCache>();
            synchronizedObjectInstanceCache.Remove(cacheKey);
        }

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context)
        {
            //Add uninitialization logic
        }
    }
}