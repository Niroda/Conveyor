namespace Conveyor.Sample.Api.Models
{
	public class Person
	{
		public string Firstname { get; set; }

		public string Lastname { get; set; }

		public DateTime? Birthday { get; set; }

		public int Age { get; set; }

		public City City { get; set; }
	}
}
