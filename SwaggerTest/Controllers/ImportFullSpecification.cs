using System.Collections.Generic;

namespace SwaggerTest.Controllers
{
  public class ImportFullSpecification : ImportSpecification
  {
    public List<ImportDefinition> ImportDefinitions { get; set; } = new List<ImportDefinition>();
  }
}
