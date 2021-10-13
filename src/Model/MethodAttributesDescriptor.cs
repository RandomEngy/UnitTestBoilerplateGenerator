using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
// ReSharper disable All

namespace UnitTestBoilerplate.Model
{
	/// <summary>
	/// Holds and analyzes an Enumerable of objects representing a particular Method's Attributes,
	/// and presents domain specific data necessary to give users the ability to specify UTBG templates
	/// for their domain.
	///
	/// The initial use of this class is to determine whether the given function is an ASP.NET REST
	/// endpoint, and if so, which type.  If there is one, checks as few attributes as possible to find
	/// it and caches the results so that each method only has to be evaluated once.
	///
	/// Future extended analysis of method attributes may require more than the minimum analysis required
	/// to find a simple attribute name.  Such searches should follow the standard of not being evaluated
	/// until the caller actually calls for the data, and should be cached once found, as no changes can
	/// be expected during the same generation session.
	/// </summary>
	public class MethodAttributesDescriptor
	{
		private HttpType? httpType;
		private readonly IEnumerable<AttributeSyntax> attributeSyntaxes;

		public MethodAttributesDescriptor(IEnumerable<AttributeSyntax> attributeSyntaxes)
		{
			this.attributeSyntaxes = attributeSyntaxes;
		}

		/// <summary>Analyzes the Attributes on a method and determines whether it has any ASP.NET MVC
		/// properties to make the method an HTTP REST Endpoint Method.  If there is, returns which type.
		/// If there isn't, it returns HttpType.None.
		/// Results are cached for each tested method for the remainder of the run.</summary>
		/// <remarks>This property can be used to determine whether ASP.NET Http style formatting is required and
		/// if so, which kinds of functions should be called against a particular type of Http endpoint.</remarks>
		/// <returns>HttpType.None if no ASP.NET style attributes can be found on the function.  Otherwise,
		/// the first HttpType that can be found.</returns>
		public HttpType Http
		{
			get
			{
				if (!this.httpType.HasValue)
				{
					this.httpType = GetHttpType();
				}
				return this.httpType.Value;
			}
		}

		/// <summary>
		/// Analyzes the Attributes on the current method and determines whether it has any ASP.NET MVC
		/// properties to make the method an HTTP REST Endpoint Method.  If there is, returns which type.
		/// If there isn't, it returns HttpType.None.
		/// </summary>
		/// <returns>HttpType.None if no ASP.NET style attributes can be found on the function.  Otherwise,
		/// the first HttpType that can be found.</returns>
		private HttpType GetHttpType()
		{
			HttpType http = HttpType.None;
			foreach (AttributeSyntax attributeSyntax in this.attributeSyntaxes)
			{
				string attributeName = attributeSyntax.Name.ToString();
				if(attributeName.StartsWith("Http"))
				{
					if(Enum.TryParse(attributeName.Replace("Http",""), out HttpType httpTemp))
					{
						http = httpTemp;
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