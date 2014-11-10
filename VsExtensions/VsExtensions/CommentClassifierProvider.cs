using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace VsExtensions
{
	[Export(typeof (IClassifierProvider))]
	[ContentType("text")]
	internal sealed class CommentClassifierProvider : IClassifierProvider
	{
		[Import]
		internal IClassificationTypeRegistryService ClassificationTypeRegistryService { get; set; }

		public IClassifier GetClassifier(ITextBuffer textBuffer)
		{
			return textBuffer.Properties.GetOrCreateSingletonProperty(
				() =>
				new CommentClassifier(ClassificationTypeRegistryService));
		}
	}
}