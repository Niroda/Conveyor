using Conveyor.Sample.Api.Models;
using Conveyor.Utility;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq.Expressions;

namespace Conveyor.Sample.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SampleController : ControllerBase
	{

        List<Person> people = new List<Person>
		{
			new Person { Age = 34, Birthday = null, Firstname = "Ali", Lastname = "Whatever" },
			new Person { Age = 25, Birthday = new DateTime(1999, 11, 12) , Firstname = "John", Lastname = "Whatever" },
			new Person { Age = 22, Birthday = null, Firstname = "Martin", Lastname = "Whatever" }
		};

		// This endpoint can be declared publicly because we use a secret key to encrypt and decrypt expressions.
		// Important: Ensure the security of your secret key! Never generate the expression in publicly accessible code,
		// as it requires the secret key for encryption.
		[HttpPost]
		public IActionResult PostExpression([FromBody] ExpressionViewModel vm)
		{
			// Recommendation: Utilize a middleware for error handling rather than using try-catch blocks in production environments.
			try
			{

				Expression<Func<Person, bool>> exp = ExpressionTransformer.DeserializeExpression<Person, Person>(vm.Expression, typeof(Person));

				return Ok(people.Where(exp.Compile()));
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
	}
}
