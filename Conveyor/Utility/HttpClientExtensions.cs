using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Conveyor.Utility
{
	/// <summary>
	/// Extensions to serialize and post an expression.
	/// </summary>
	public static class HttpClientExtensions
	{

		/// <summary>
		/// Serializes given expression and encrypts the serialized JSON using the given secret key 
		/// and posts the encrypted model as a byte array.
		/// </summary>
		/// <typeparam name="T">Type of the <see cref="System.Linq.Expressions.Expression"/></typeparam>
		/// <param name="httpClient">An <see cref="HttpRequestMessage"/> to set the value</param>
		/// <param name="url">The url to post data to</param>
		/// <param name="expression">Desired expression to be set</param>
		/// <returns>A <see cref="Task{HttpResponseMessage}"/></returns>
		public static async Task<HttpResponseMessage> WhereAsync<T>(
			this HttpClient httpClient,
			string url,
			T expression
		)
			where T : Expression
		{
			if (httpClient == null)
				throw new ArgumentNullException(nameof(httpClient));

			using (var request = new HttpRequestMessage(HttpMethod.Post, url))
			{
				request.Headers.Accept.Clear();
				request.Headers.Accept.ParseAdd("application/json");

				var vm = new ExpressionViewModel {
					Expression = ExpressionTransformer.SerializeExpression(expression)
				};

				var vmJson = JsonConvert.SerializeObject(vm);
				var vmBytes = Encoding.UTF8.GetBytes(vmJson);

				using (request.Content = new ByteArrayContent(vmBytes))
				{
					request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

					return await httpClient.SendAsync(request);
				}
			}
		}
	}
}
