using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using Newtonsoft.Json;
using RestSharp;

namespace ApiTest
{
  public class Program
  {
    public static void Main(string[] args)
    {
      Import_RestSharp();
      Import_HttpClient();
    }

    private static void Import_HttpClient()
    {
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

      var form = new MultipartFormDataContent();

      // BEWARE - names have to match API parameter names!
      form.Headers.Add("importSpec", json);

      var stream = new FileStream(currExe, FileMode.Open);
      var content = new StreamContent(stream);
      content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
      {
        // BEWARE - names have to match API parameter names!
        Name = "file",
        FileName = currExe
      };
      form.Add(content);

      var client = new HttpClient();
      var res = client.PostAsync("http://localhost:5000/Importer/ImportFull", form).Result;
      var apiRes = res.Content.ReadAsStringAsync().Result;

      Console.WriteLine($"{nameof(Import_HttpClient)} --> {apiRes}");
    }

    private static void Import_RestSharp()
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

      Console.WriteLine($"{nameof(Import_RestSharp)} --> {apiRes}");
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
