using System.Threading;
using Ascend2015.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using EPiServer.Framework.Cache;
using EPiServer.ServiceLocation;

namespace Ascend2015.Business.StandardPageRepository
{
    //[ServiceConfiguration(typeof(IStandardPageRepository))]
    public class LockingCachedFindPagesWithCriterion : IStandardPageRepository
    {
        // Something about locking on strings
        private static readonly object Lock = new object();

        private readonly SearchPages _searchPages;
        private readonly PageTypeRepository _pageTypeRepository;
        private readonly ISynchronizedObjectInstanceCache _synchronizedObjectInstanceCache;

        public LockingCachedFindPagesWithCriterion(SearchPages searchPages, PageTypeRepository pageTypeRepository, ISynchronizedObjectInstanceCache synchronizedObjectInstanceCache)
        {
            _searchPages = searchPages;
            _pageTypeRepository = pageTypeRepository;
            _synchronizedObjectInstanceCache = synchronizedObjectInstanceCache;
        }

        public PageDataCollection List(PageReference pageLink)
        {
            string cacheKey = CachedFindPagesWithCriterion.GetCacheKey(pageLink.ToReferenceWithoutVersion());

            var standardPages = _synchronizedObjectInstanceCache.Get(cacheKey) as PageDataCollection;
            if (standardPages == null)
            {
                lock (Lock)
                {
                    // Check cache again since standardPages might have been cached while waiting for the lock to open
                    standardPages = _synchronizedObjectInstanceCache.Get(cacheKey) as PageDataCollection;
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

                        Thread.Sleep(5000);

                        _synchronizedObjectInstanceCache.Insert(cacheKey, standardPages, CacheEvictionPolicy.Empty);
                    }
                }
            }

            return standardPages;
        }
    }
}