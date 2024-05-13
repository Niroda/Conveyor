using Conveyor.Utility;
using Conveyor.Sample.Client;
using System.Linq.Expressions;
using System.Text.Json;
using Newtonsoft.Json;

Environment.SetEnvironmentVariable("CONVEYOR_SECRET_KEY", "4-S3cr3t-v4lu3", EnvironmentVariableTarget.Process);

HttpClient httpClient = new HttpClient();

Expression<Func<Person, bool>> expression = x => x.Age > 18 || x.Birthday != null;

// change the url if you have a different one!
HttpResponseMessage httpResponse = await httpClient.WhereAsync("https://localhost:7164/api/Sample", expression);

string result = await httpResponse.Content.ReadAsStringAsync();

List<Person> people = JsonConvert.DeserializeObject<List<Person>>(result);

Environment.SetEnvironmentVariable("CONVEYOR_SECRET_KEY", null, EnvironmentVariableTarget.Process);