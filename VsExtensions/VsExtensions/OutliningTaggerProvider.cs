using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace VsExtensions
{
	[Export(typeof (ITaggerProvider))]
	[TagType(typeof (IOutliningRegionTag))]
	[ContentType("text")]
	internal sealed class OutliningTaggerProvider : ITaggerProvider
	{
		public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
		{
			return buffer.Properties.GetOrCreateSingletonProperty(() => new OutliningTagger(buffer) as ITagger<T>);
		}
	}
}