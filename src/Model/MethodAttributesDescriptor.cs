using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnitTestBoilerplate.Model
{
	public class MethodAttributesDescriptor
	{
		private HttpType? _httpType;
		private readonly IEnumerable<AttributeSyntax> _attributeSyntaxes;

		public MethodAttributesDescriptor(IEnumerable<AttributeSyntax> attributeSyntaxes)
		{
			_attributeSyntaxes = attributeSyntaxes;
		}

		public HttpType Http
		{
			get
			{
				if (!_httpType.HasValue)
				{
					_httpType = GetHttpType();
				}
				return _httpType.Value;
			}
		}

		private HttpType GetHttpType()
		{
			HttpType http = HttpType.None;
			foreach (AttributeSyntax attributeSyntax in _attributeSyntaxes)
			{
				string attributeName = attributeSyntax.Name.ToString();
				if(attributeName.StartsWith("Http"))
				{
					if(Enum.TryParse(attributeName.Replace("Http",""), out http))
					{
						break;
					}
				}
			}

			return http;
		}
	}

	public enum HttpType
	{
		Get,
		Post,
		Put,
		Patch,
		Delete,
		Options,
		Head,
		None
	}
}