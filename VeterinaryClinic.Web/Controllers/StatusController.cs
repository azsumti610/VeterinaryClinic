using Microsoft.AspNetCore.Mvc;
using VeterinaryClinic.Application.Services;

namespace VeterinaryClinic.Web.Controllers
{
    public class StatusController : Controller
    {
        private readonly IGreetingService _service;

        public StatusController(IGreetingService service)
            => _service = service;

        public IActionResult Index()
            => Content(_service.GetMessage());
    }
}
