using System.Collections.Generic;
using Ascend2015.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Ascend2015.Business.StandardPageRepository
{
    //[ServiceConfiguration(typeof(IStandardPageRepository))]
    public class GetDescendentsGetItems : IStandardPageRepository
    {
        private readonly IContentLoader _contentLoader;

        public GetDescendentsGetItems(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        public PageDataCollection List(PageReference pageLink)
        {
            IEnumerable<ContentReference> descendantContentLinks = _contentLoader.GetDescendents(pageLink);
            IEnumerable<IContent> allDescendantContent = _contentLoader.GetItems(descendantContentLinks, new LoaderOptions());

            var standardPages = new PageDataCollection();
            foreach (IContent content in allDescendantContent)
            {
                if (IsStandardPage(content))
                {
                    standardPages.Add(content);
                }
            }

            return standardPages;
        }

        private bool IsStandardPage(IContent content)
        {
            return content is StandardPage;
        }
    }
}