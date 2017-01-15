using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Hive.Handlers;
using Microsoft.AspNetCore.Http;

namespace Hive.Web.Rest.RouteHandlers
{
	public abstract class RouteHandler : IRouteHandler
	{
		public abstract Task Handle(HttpContext context);

		// See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing#route-constraint-reference
		private static readonly IImmutableDictionary<Type, Func<object, object>> TypeConstraints = new Dictionary<Type, Func<object, object>>
		{
			{ typeof(int), x => Convert.ToInt32(x) },
			{ typeof(bool), x => Convert.ToBoolean(x) },
			{ typeof(DateTime), x => DateTime.Parse(x.ToString()) },
			{ typeof(decimal), x => Convert.ToDecimal(x) },
			{ typeof(double), x => Convert.ToDouble(x) },
			{ typeof(float), x => Convert.ToSingle(x) },
			{ typeof(Guid), x => Guid.Parse(x.ToString()) },
			{ typeof(long), x => Convert.ToInt64(x) }
		}.ToImmutableDictionary();

		protected virtual object ConvertKnownIdType(object id, Type type)
		{
			if (type == null)
				return null;

			return TypeConstraints.ContainsKey(type) ? TypeConstraints[type](id) : id;
		}

		protected virtual Task InterpretResult(HttpContext context, IHandlerResult result)
		{
			context.Response.StatusCode = StatusCodes.Status418ImATeapot;
			return Task.CompletedTask;
		}
	}
}