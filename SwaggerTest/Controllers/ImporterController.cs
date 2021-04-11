using System.Collections.Generic;
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

  #region Data Structures
  public class ImportBaseSpecification
  {
    public List<string> Links { get; set; } = new List<string>();
  }

  public class ImportSpecification : ImportBaseSpecification
  {
    public string DataFile { get; set; }
  }

  public class ImportFullSpecification : ImportSpecification
  {
    public List<ImportDefinition> ImportDefinitions { get; set; } = new List<ImportDefinition>();
  }

  public class ImportDefinition
  {
    public string Name { get; set; }
  }
  #endregion
}
