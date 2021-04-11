using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using RestSharp;

namespace ApiTest
{
  public class Program
  {
    public static void Main(string[] args)
    {
      ImportFull();
    }

    private static void ImportFull()
    {
      var client = new RestClient("http://localhost:5000/");
      var req = new RestRequest("/Importer/ImportFull", Method.POST);
      var currExe = Assembly.GetExecutingAssembly().Location;
      var data = new ImportFullSpecification
      {
        Links = new List<string>
        {
          "bbb",
          "ccc"
        },
        ImportDefinitions = new List<ImportDefinition>
        {
          new ImportDefinition
          {
            Name = "xxx"
          },
          new ImportDefinition
          {
            Name = "yyy"
          }
        },
        DataFile = currExe
      };
      var json = JsonConvert.SerializeObject(data);

      // BEWARE - names have to match API parameter names!
      req.AddHeader("importSpec", json);
      req.AddFile("file", currExe);

      var res = client.Execute<string>(req);
      var apiRes = res.Data;

      Console.WriteLine($"ImportFull --> {apiRes}");
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
