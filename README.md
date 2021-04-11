# SwaggerApiTest - uploading a file *and* data!

**TL;DR** marshal data as json in a header

## Background
I developed an API to receive a data file *and* some configuration.
This was a RESTful API along the lines of:

```csharp
public string ImportFull(ImportFullSpecification importSpec, IFormFile file);
```

## Problems
### HTTP request
The main problem is combining an object and a file in the same HTTP request.
It turns out that mixing form data (*IFormFile*) and a json body (*ImportFullSpecification*)
is not really possible.

### Swagger
We are using [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) to
provide a UI to test our API.  Unfortunately, *ImportFullSpecification* is a nested object
and creating these *and* selecting *IFormFile* is not really supported with *Swashbuckle*
(or *NSwag*).  It was a case of one or the other but never both.

## Solution
The API was changed to:

```csharp
public string ImportFull([FromHeader] string importSpec, IFormFile file);
```

and *ImportFullSpecification* is sent across as a json string in a header.

Obviously, any object must be serialisable as json, so this works best for POCOs.

### API code
```csharp
public string ImportFull([FromHeader] string importSpec, IFormFile file)
{
  var importSpecObj = JsonConvert.DeserializeObject<ImportFullSpecification>(importSpec);
  var json = JsonConvert.SerializeObject(importSpecObj);
  return file?.FileName + " --> " + json;
}
```

Note that the header parameter is immediately deserialised into a strongly typed object.

### Client code
```csharp
private static void ImportFull()
{
  // form URI to API
  var client = new RestClient("http://localhost:5000/");
  var req = new RestRequest("/Importer/ImportFull", Method.POST);

  // some arbitrary file on disk
  var currExe = Assembly.GetExecutingAssembly().Location;

  // configure import spec here...
  var data = new ImportFullSpecification();

  // convert to a json string for marshalling
  var json = JsonConvert.SerializeObject(data);

  // BEWARE - names have to match API parameter names!
  // marshal import specification as a header parameter
  req.AddHeader("importSpec", json);
  req.AddFile("file", currExe);

  var res = client.Execute<string>(req);
  var apiRes = res.Data;

  Console.WriteLine($"ImportFull --> {apiRes}");
}
```

Note that the API and client code **have** to agree on the names of the header parameter
(*"importSpec"*) and file parameter (*"file"*), so the parameters can be extracted from
the underlying http request.

The client code uses [RestSharp](https://restsharp.dev/) for conveniently adding headers
and files.

However, the important point is that an object is sent to the API as a json string
in an agreed header parameter.

### Swagger UI
[SwaggerUI](http://localhost:5000/swagger/index.html) now has a text box to hold a json
representation of an *ImportFullSpecification*:

```json
{"ImportDefinitions":[{"Name":"xxx"},{"Name":"yyy"}],"DataFile":"some arbitrary file","Links":["bbb","ccc"]}
```
and a button to browse for a file