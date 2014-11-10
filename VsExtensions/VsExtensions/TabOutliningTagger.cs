using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace VsExtensions
{
	internal sealed class TabOutliningTagger : ITagger<IOutliningRegionTag>
	{
		private ITextBuffer _buffer;

		internal TabOutliningTagger(ITextBuffer buffer)
		{
			_buffer = buffer;
		}

		public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			foreach (var span in spans)
			{
				ITextSnapshotLine parent = null;
				ITextSnapshotLine endLine = null;

				foreach (var line in span.Snapshot.Lines)
				{
					var text = line.GetText();

					if (parent != null && text.StartsWith("\t"))
						endLine = line;

					if (parent != null && endLine != null && (!text.StartsWith("\t") || string.IsNullOrWhiteSpace(text)))
					{
						var snapshot = new SnapshotSpan(parent.End, endLine.End);

						yield return
							new TagSpan<IOutliningRegionTag>(snapshot, new OutliningRegionTag(false, false, "...", snapshot.GetText()));

						parent = line;
						endLine = null;
					}

					if (!text.StartsWith("\t"))
						parent = line;

					if (string.IsNullOrWhiteSpace(text))
					{
						parent = null;
						endLine = null;
					}
				}
			}
		}

		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
	}
}
