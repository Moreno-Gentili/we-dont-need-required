using Microsoft.AspNetCore.Mvc;
using WeDontNeedRequired.Models;

namespace WeDontNeedRequired.Controllers;

[ApiController]
[Route("[controller]")]
public class TimeTravelController : ControllerBase
{
    [HttpPost]
    [Route("Configure")]
    public IActionResult Post(TimeTravelConfiguration configuration)
    {
        // TODO: Set the time panel, adjust the flux capacitor

        return Accepted();
    }
}
