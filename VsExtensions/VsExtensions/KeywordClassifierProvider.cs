using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace VsExtensions
{
	[Export(typeof (IClassifierProvider))]
	[ContentType("text")]
	internal sealed class KeywordClassifierProvider : IClassifierProvider
	{
		private static bool _ignoreRequest;

		[Import]
		internal IClassificationTypeRegistryService ClassificationTypeRegistryService { get; set; }
		[Import]
		internal IClassifierAggregatorService ClassifierAggregatorService { get; set; }

		public IClassifier GetClassifier(ITextBuffer textBuffer)
		{
			if (_ignoreRequest)
				return null;

			try
			{
				_ignoreRequest = true;
				return textBuffer.Properties.GetOrCreateSingletonProperty(
					() =>
					new KeywordClassifier(ClassificationTypeRegistryService, ClassifierAggregatorService.GetClassifier(textBuffer)));
			}
			finally
			{
				_ignoreRequest = false;
			}
		}
	}
}