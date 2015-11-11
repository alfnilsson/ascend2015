using Ascend2015.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using EPiServer.ServiceLocation;

namespace Ascend2015.Business.StandardPageRepository
{
    //[ServiceConfiguration(typeof(IStandardPageRepository))]
    public class FindPagesWithCritera : IStandardPageRepository
    {
        private readonly IPageCriteriaQueryService _searchPages;
        private readonly PageTypeRepository _pageTypeRepository;

        public FindPagesWithCritera(IPageCriteriaQueryService searchPages, PageTypeRepository pageTypeRepository)
        {
            _searchPages = searchPages;
            _pageTypeRepository = pageTypeRepository;
        }

        public virtual PageDataCollection List(PageReference pageLink)
        {
            // Using functionality from
            // http://world.episerver.com/documentation/Items/Developers-Guide/EPiServer-CMS/9/Search/Searching-for-pages-based-on-page-type/
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

            PageDataCollection pageDataCollection = _searchPages.FindPagesWithCriteria(pageLink, criteria);

            return pageDataCollection;
        }
    }
}