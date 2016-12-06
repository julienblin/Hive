﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hive.Web.Rest.Serializers.Impl
{
	public class JsonRestSerializer : RestSerializer
	{
		public JsonRestSerializer()
			: base (new []{ "application/json" })
		{
		}

		protected override void SerializeEntity(IEntity entity, Stream stream)
		{
			
		}

		protected override void SerializeEntities(IEnumerable<IEntity> enumerable, Stream stream)
		{
			
		}

		protected override void SerializeMessage(object message, Stream stream)
		{
			HiveJsonSerializer.Instance.Serialize(message, stream);
		}

		public override async Task<IEntity> Deserialize(IEntityDefinition entityDefinition, Stream stream, CancellationToken ct)
		{
			var entity = new Entity(entityDefinition);

			using (var reader = new StreamReader(stream))
			{
				var jObject = JObject.Parse(await reader.ReadToEndAsync());
				foreach (var property in jObject.Properties())
				{
					entity.SetPropertyValue(property.Name, property.Value);
				}
			}

			await entity.Init(ct);

			return entity;
		}
	}
}