using System.Threading;
using Ascend2015.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Ascend2015.Business.StandardPageRepository
{
    public interface IStandardPageRepository
    {
        PageDataCollection List(PageReference pageLink);
    }

    [ServiceConfiguration(typeof(IStandardPageRepository))]
    public class EmptyStandardPageRepository : IStandardPageRepository
    {
        private readonly IContentRepository _contentLoader;

        public EmptyStandardPageRepository(IContentRepository contentLoader)
        {
            _contentLoader = contentLoader;
        }

        public PageDataCollection List(PageReference pageLink)
        {
            // This is to display before and after SessionState
            Thread.Sleep(3000);

            return new PageDataCollection();
        }
    }
}