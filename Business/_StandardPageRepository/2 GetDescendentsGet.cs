using System.Collections.Generic;
using Ascend2015.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Ascend2015.Business.StandardPageRepository
{
    //[ServiceConfiguration(typeof(IStandardPageRepository))]
    public class GetDescendentsGet : IStandardPageRepository
    {
        private readonly IContentLoader _contentLoader;

        public GetDescendentsGet(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        public PageDataCollection List(PageReference pageLink)
        {
            IEnumerable<ContentReference> descendantContentLinks = _contentLoader.GetDescendents(pageLink);

            var standardPages = new PageDataCollection();
            foreach (ContentReference contentLink in descendantContentLinks)
            {
                StandardPage standardPage = GetStandardPage(contentLink);
                if (standardPage != null)
                {
                    standardPages.Add(standardPage);
                }
            }

            return standardPages;
        }

        private StandardPage GetStandardPage(ContentReference descendentContentLink)
        {
            var page = _contentLoader.Get<StandardPage>(descendentContentLink);
            return page;
        }
    }
}