using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace VsExtensions
{
	internal class KeywordClassifier : IClassifier
	{
		private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;
		private readonly IClassifier _classifier;

		public KeywordClassifier(IClassificationTypeRegistryService classificationTypeRegistryService, IClassifier classifier)
		{
			_classificationTypeRegistryService = classificationTypeRegistryService;
			_classifier = classifier;
		}

		public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
		{
			var comments = _classifier.GetClassificationSpans(span)
				.Where(classificationSpan => classificationSpan.ClassificationType.Classification.ToLower().Contains("comment"))
				.Select(classificationSpan => classificationSpan.Span);

			var ignored = new NormalizedSnapshotSpanCollection(comments);
			var request = new NormalizedSnapshotSpanCollection(span);

			var spansForKeyWords = NormalizedSnapshotSpanCollection.Difference(request, ignored);

			if (!spansForKeyWords.Any())
				return null;

			var type = _classificationTypeRegistryService.GetClassificationType("keyword");

			return GetKeywords(spansForKeyWords, type)
				.ToList();
		}

		private static IEnumerable<ClassificationSpan> GetKeywords(IEnumerable<SnapshotSpan> collection, IClassificationType type)
		{
			return collection
				.SelectMany(span => GetSpans(span, type));
		}

		private static IEnumerable<ClassificationSpan> GetSpans(SnapshotSpan span, IClassificationType type)
		{
			var text = span.GetText();

			return GetSpans(span, text, type, "js")
				.Concat(GetSpans(span, text, type, "css"));
		}

		private static IEnumerable<ClassificationSpan> GetSpans(SnapshotSpan span, string text, IClassificationType type, string key)
		{
			var indexes = GetIndexes(text, key);

			var spans = indexes.Select(index => new ClassificationSpan(new SnapshotSpan(span.Start + index, key.Length), type));

			return spans;
		}

		private static IEnumerable<int> GetIndexes(string text, string key)
		{
			var index = -1;

			while ((index = text.IndexOf(key, index + 1, StringComparison.Ordinal)) != -1)
			{
				yield return index;
			}
		}

		public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
	}
}