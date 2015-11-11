using System.Web.Mvc;
using System.Web.SessionState;
using Ascend2015.Business.StandardPageRepository;
using Ascend2015.Models.Pages;
using Ascend2015.Models.ViewModels;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Ascend2015.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class ProductPageController : PageControllerBase<ProductPage>
    {
        private readonly IStandardPageRepository _standardPageRepository;

        // You can also fetch IStandardPageRepository using ServiceLocator.Current.GetInstance<IStandardPageRepository>();
        public ProductPageController(IStandardPageRepository standardPageRepository)
        {
            _standardPageRepository = standardPageRepository;
        }

        public ActionResult Index(ProductPage currentPage)
        {
            var model = PageViewModel.Create(currentPage);

            PageDataCollection standardPages = _standardPageRepository.List(currentPage.PageLink);

            return View(model);
        }
    }
}
