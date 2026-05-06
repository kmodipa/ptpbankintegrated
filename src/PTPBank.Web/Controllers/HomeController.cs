using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PTPBank.Web.Models.ViewModels;

namespace PTPBank.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    public IActionResult About() => View();

    public IActionResult Contact() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("~/Views/Shared/Error.cshtml", new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
