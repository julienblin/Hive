using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hive.Meta
{
	public interface IMetaService
	{
		IEnumerable<HandlerInfo> GetHandlerInfos();

		ResourceDescription GetResourceDescription(Type type);
	}
}