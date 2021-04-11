using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SwaggerTest.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class ImporterController : ControllerBase
  {
    [HttpPost]
    [Route(nameof(ImportFull))]
    public string ImportFull(
      [FromHeader] string importSpec,
      IFormFile file)
    {
      var importSpecObj = JsonConvert.DeserializeObject<ImportFullSpecification>(importSpec);
      var json = JsonConvert.SerializeObject(importSpecObj);
      return file?.FileName + " --> " + json;
    }
  }
}
