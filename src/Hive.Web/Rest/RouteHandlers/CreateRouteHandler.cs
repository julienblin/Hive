using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Handlers;
using Hive.Meta;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Linq;

namespace Hive.Web.Rest.RouteHandlers
{
	public class CreateRouteHandler<TResource> : RouteHandler
		where TResource : class
	{
		private readonly IHttpRequestStreamReaderFactory _readerFactory;
		private readonly IServiceProvider _serviceProvider;
		private readonly IEnumerable<IInputFormatter> _inputFormatters;
		private readonly HandlerInfo _handlerInfo;
		private readonly ModelMetadata _modelMetadata;

		public CreateRouteHandler(
			IServiceProvider serviceProvider,
			IEnumerable<IInputFormatter> inputFormatters,
			IHttpRequestStreamReaderFactory readerFactory,
			HandlerInfo handlerInfo,
			ModelMetadata modelMetadata)
		{
			_serviceProvider = serviceProvider.NotNull(nameof(serviceProvider));
			_inputFormatters = inputFormatters.NotNull(nameof(inputFormatters)).ToList();
			_readerFactory = readerFactory.NotNull(nameof(readerFactory));
			_handlerInfo = handlerInfo.NotNull(nameof(handlerInfo));
			_modelMetadata = modelMetadata.NotNull(nameof(modelMetadata));
		}

		public override async Task Handle(HttpContext context)
		{
			var handler = _serviceProvider.GetService(_handlerInfo.HandlerInterfaceType) as IHandleCreate<TResource>;
			var modelStateDictionary = new ModelStateDictionary();
			var inputFormaterContext = new InputFormatterContext(
				context,
				_handlerInfo.ResourceType.Name,
				modelStateDictionary,
				_modelMetadata,
				_readerFactory.CreateReader
				);

			var formatter = _inputFormatters.FirstOrDefault(inputFormatter => inputFormatter.CanRead(inputFormaterContext));

			if (formatter == null)
			{
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				return;
			}

			var formatterResult = await formatter.ReadAsync(inputFormaterContext);
			if (formatterResult.HasError)
			{
				context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
				return;
			}


			var result = await handler.Create((TResource)formatterResult.Model, context.RequestAborted);
			await InterpretResult(context, result);
		}
	}
}