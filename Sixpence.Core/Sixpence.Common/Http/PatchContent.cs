using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Sixpence.Common.Http
{
	public class PatchContent : StringContent
	{
		public PatchContent(object value)
			: base(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json-patch+json")
		{
		}
	}
}
