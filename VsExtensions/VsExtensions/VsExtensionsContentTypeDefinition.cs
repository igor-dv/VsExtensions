using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace VsExtensions
{
	internal static class VsExtensionsContentTypeDefinition
	{
		[Export]
		[Name("resources")]
		[BaseDefinition("text")]
		internal static ContentTypeDefinition ResourcesContentTypeDefinition = null;
	}
}