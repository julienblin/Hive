using System.Collections.Generic;
using FluentAssertions;
using Hive.Web.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Xunit;
using System.Linq;
using Hive.Foundation.Extensions;
using Hive.Tests.Mocks;
using Hive.Web.Rest.Serializers.Impl;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Hive.Web.Tests.Rest
{
	public class RestQueryStringTests
	{
		[Theory]
		[MemberData(nameof(RestQueryStringCtorData))]
		public void RestQueryStringCtor(RestProcessParameters param, RestQueryString expected)
		{
			var queryString = new RestQueryString(param);
			queryString.ShouldBeEquivalentTo(expected, options => options.IncludingProperties().IncludingNestedObjects());
		}

		public static IEnumerable<object[]> RestQueryStringCtorData()
		{
			yield return new object[]
			{
				CreateRestProcessParameter("/patients"),
				new RestQueryString(
					"patients",
					null,
					new Dictionary<string, string>().ToImmutableDictionary(),
					new Dictionary<string, StringValues>().ToImmutableDictionary())
			};

			yield return new object[]
			{
				CreateRestProcessParameter("/patients/1"),
				new RestQueryString(
					"patients",
					"1",
					new Dictionary<string, string>().ToImmutableDictionary(),
					new Dictionary<string, StringValues>().ToImmutableDictionary())
			};

			yield return new object[]
			{
				CreateRestProcessParameter("/clients/1/patients/1"),
				new RestQueryString(
					"patients",
					"1",
					new Dictionary<string, string>()
					{
						{ "clients", "1" }
					}.ToImmutableDictionary(),
					new Dictionary<string, StringValues>().ToImmutableDictionary())
			};

			yield return new object[]
			{
				CreateRestProcessParameter("/clients/patients/1"),
				new RestQueryString(
					"1",
					null,
					new Dictionary<string, string>()
					{
						{ "clients", "patients" }
					}.ToImmutableDictionary(),
					new Dictionary<string, StringValues>().ToImmutableDictionary())
			};

			yield return new object[]
			{
				CreateRestProcessParameter("/clients/1/patients/1", "include=orders&limit=2"),
				new RestQueryString(
					"patients",
					"1",
					new Dictionary<string, string>()
					{
						{ "clients", "1" }
					}.ToImmutableDictionary(),
					new Dictionary<string, StringValues>()
					{
						{ "include", new StringValues("orders") },
						{ "limit", new StringValues("2") }
					}.ToImmutableDictionary())
			};

			yield return new object[]
			{
				CreateRestProcessParameter("/patients/1", "include=orders&include=invoices&limit=2"),
				new RestQueryString(
					"patients",
					"1",
					new Dictionary<string, string>().ToImmutableDictionary(),
					new Dictionary<string, StringValues>()
					{
						{ "include", new StringValues(new[] { "orders", "invoices"}) },
						{ "limit", new StringValues("2") }
					}.ToImmutableDictionary())
			};
		}

		private static RestProcessParameters CreateRestProcessParameter(string pathSegments, string queryString = null)
		{
			return new RestProcessParameters(
				new DefaultHttpContext { Request = { Query = new QueryCollection(QueryHelpers.ParseQuery(queryString)) } },
				new RequestHeaders(new HeaderDictionary()),
				pathSegments.Split('/').Where(x => !x.Trim().IsNullOrEmpty()).ToArray(),
				new ModelMock(),
				new JsonRestSerializer(),
				new JsonRestSerializer()
			);
		}
	}
}