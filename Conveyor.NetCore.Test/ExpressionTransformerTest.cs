using Conveyor.NetCore.Test.Tools;
using Conveyor.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Conveyor.NetCore.Test
{
	/// <summary>
	/// Tests for verifying the serialization and deserialization of expressions in the ExpressionTransformer.
	/// </summary>
	[TestFixture]
	public class ExpressionTransformerTest
	{


		[SetUp]
		public void Initialize()
		{
			Environment.SetEnvironmentVariable("CONVEYOR_SECRET_KEY", "4-S3cr3t-v4lu3", EnvironmentVariableTarget.Process);
		}



		[TearDown]
		public void Cleanup()
		{
			// Clean up the environment variable after the test completes
			Environment.SetEnvironmentVariable("CONVEYOR_SECRET_KEY", null, EnvironmentVariableTarget.Process);
		}

		private static IEnumerable<TestCaseData> ExpressionSources()
		{
			var expressions = new List<Expression<Func<SampleViewModel, bool>>>
			{
				x => x.IntValue > 1,
				x => x.ShortValue >= 3,
				x => x.LongValue < 55,
				x => x.DoubleValue > 7 || x.DoubleValue == 1.2 || x.DoubleValue != Constants.Gravity,
				x => x.FloatValue == 1.2 ^ x.IntValue > 5,
				x => x.ByteValue != 0x4 && x.IntValue <= 6
			};

			DateTime dateTime1 = DateTime.Now.AddDays(3);
			DateTime dateTime2 = DateTime.Now.AddDays(5);

			expressions.AddRange(new List<Expression<Func<SampleViewModel, bool>>>
			{
				x => x.DatetimeValue < dateTime1,
				x => x.DatetimeValue > dateTime2,
				x => x.DatetimeValue == dateTime1,
				x => x.DatetimeValue != dateTime2
			});

			string val = "o";
			expressions.AddRange(new List<Expression<Func<SampleViewModel, bool>>>
			{
				x => x.StringValue == "First Sample",
				x => x.StringValue.Equals("Second Sample"),
				x => x.StringValue.EndsWith("d Sample"),
				x => x.StringValue.StartsWith("S"),
				x => x.StringValue.Contains(val)
			});

			val = "Just a dummy text";
			expressions.AddRange(new List<Expression<Func<SampleViewModel, bool>>>
			{
				x => val.Length > x.IntValue
			});

			string[] vals = { "foo", "bar" };
			expressions.AddRange(new List<Expression<Func<SampleViewModel, bool>>>
			{
				x => vals.Contains(x.StringValue)
			});

			foreach (var expr in expressions)
				yield return new TestCaseData(expr);
		}

		private static ExpressionViewModel Serialize(Expression<Func<SampleViewModel, bool>> expression)
		{
			return new ExpressionViewModel
			{
				Expression = ExpressionTransformer.SerializeExpression(expression, typeof(SampleViewModel))
			};
		}

		private static Expression<Func<SampleModel, bool>> Deserialize(ExpressionViewModel viewModel)
		{
			return ExpressionTransformer.DeserializeExpression<SampleModel, SampleViewModel>(viewModel.Expression, typeof(SampleViewModel));
		}

		[TestCaseSource(nameof(ExpressionSources))]
		public void ValidateSerializationAndDeserialization(Expression<Func<SampleViewModel, bool>> expression)
		{
			var serializedExpression = Serialize(expression);
			var deserializedExpression = Deserialize(serializedExpression);

			Assert.That(deserializedExpression, Is.Not.Null, "Deserialized expression should not be null.");
			Trace.WriteLine(deserializedExpression);
		}
	}
}
