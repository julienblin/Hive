using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Hive.Foundation.Extensions;
using Microsoft.Extensions.Primitives;

namespace Hive.Web.Rest
{
	public class RestQueryString
	{
		public RestQueryString(RestProcessParameters param)
		{
			Root = param.PathSegments.Length.IsOdd()
				? param.PathSegments.Last()
				: param.PathSegments.ElementAtOrDefault(param.PathSegments.Length - 2);
			AdditionalQualifier = param.PathSegments.Length.IsEven() ? param.PathSegments.Last() : null;

			var pathValues = new Dictionary<string, string>();
			var remainingPathSegments = param.PathSegments.Length.IsOdd()
				? param.PathSegments.Take(param.PathSegments.Length - 1)
				: param.PathSegments.Take(param.PathSegments.Length - 2);

			string currentKey = null;
			foreach (var remainingPathSegment in remainingPathSegments)
				if (currentKey == null)
				{
					currentKey = remainingPathSegment;
				}
				else
				{
					pathValues.Add(currentKey, remainingPathSegment);
					currentKey = null;
				}
			PathValues = pathValues.ToImmutableDictionary();
			QueryStringValues = param.Context.Request.Query.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);
		}

		public RestQueryString(string root, string additionalQualifier, IImmutableDictionary<string, string> pathValues,
			IImmutableDictionary<string, StringValues> queryStringValues)
		{
			Root = root;
			AdditionalQualifier = additionalQualifier;
			PathValues = pathValues;
			QueryStringValues = queryStringValues;
		}

		public string Root { get; }

		public string AdditionalQualifier { get; }

		public IImmutableDictionary<string, string> PathValues { get; }

		public IImmutableDictionary<string, StringValues> QueryStringValues { get; }
	}
}