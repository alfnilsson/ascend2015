using System.Collections.Concurrent;
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
    public class MiniLockingCachedFindPagesWithCriterion : IStandardPageRepository
    {
        // ConcurrentDictionary is Thread safe
        private static readonly ConcurrentDictionary<string, object> MiniLocks = new ConcurrentDictionary<string, object>();

        private readonly SearchPages _searchPages;
        private readonly PageTypeRepository _pageTypeRepository;
        private readonly ISynchronizedObjectInstanceCache _synchronizedObjectInstanceCache;

        public MiniLockingCachedFindPagesWithCriterion(SearchPages searchPages, PageTypeRepository pageTypeRepository, ISynchronizedObjectInstanceCache synchronizedObjectInstanceCache)
        {
            _searchPages = searchPages;
            _pageTypeRepository = pageTypeRepository;
            _synchronizedObjectInstanceCache = synchronizedObjectInstanceCache;
        }

        public PageDataCollection List(PageReference pageLink)
        {
            string key = CachedFindPagesWithCriterion.GetCacheKey(pageLink.ToReferenceWithoutVersion());

            var standardPages = _synchronizedObjectInstanceCache.Get(key) as PageDataCollection;
            if (standardPages == null)
            {
                object miniLock = MiniLocks.GetOrAdd(key, k => new object());
                lock (miniLock)
                {
                    standardPages = _synchronizedObjectInstanceCache.Get(key) as PageDataCollection;
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

                        _synchronizedObjectInstanceCache.Insert(key, standardPages, CacheEvictionPolicy.Empty);

                        // Saving some space we will release the lock
                        object temp1;
                        if (MiniLocks.TryGetValue(key, out temp1) && (temp1 == miniLock))
                        {
                            object temp2;
                            MiniLocks.TryRemove(key, out temp2);
                        }
                    }
                }
            }

            return standardPages;
        }
    }
}