using Microsoft.AspNetCore.Mvc;
using OperationResults;
using OperationResults.AspNetCore;

namespace MusicUploader.Controllers.Common;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
{
    protected ControllerBase()
    {
    }


    protected IActionResult CreateResponse(Result result, int? successStatusCode = null)
    {
        return HttpContext.CreateResponse(result, successStatusCode);
    }

    protected IActionResult CreateResponse<T>(Result<T> result, int? successStatusCode = null)
    {
        return HttpContext.CreateResponse(result, successStatusCode);
    }
}