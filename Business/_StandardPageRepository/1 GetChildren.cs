using System.Collections.Generic;
using Ascend2015.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using NuGet;

namespace Ascend2015.Business.StandardPageRepository
{
    //[ServiceConfiguration(typeof(IStandardPageRepository))]
    public class GetChildren : IStandardPageRepository
    {
        private readonly IContentLoader _contentLoader;

        public GetChildren(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        public PageDataCollection List(PageReference pageLink)
        {
            PageDataCollection pages = GetStandardPageChildren(pageLink);
            return pages;
        }

        private PageDataCollection GetStandardPageChildren(PageReference parent)
        {
            PageDataCollection standardPages = new PageDataCollection();

            IEnumerable<PageData> children = _contentLoader.GetChildren<PageData>(parent);

            foreach (PageData child in children)
            {
                var standardPage = child as StandardPage;
                if (standardPage != null)
                {
                    standardPages.Add(standardPage);
                }

                standardPages.AddRange(GetStandardPageChildren(child.PageLink));
            }

            return standardPages;
        }
    }
}