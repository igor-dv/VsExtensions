using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace VsExtensions
{
	internal class CommentClassifier : IClassifier
	{
		private const string CommentPrefix = "//";

		private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

		public CommentClassifier(IClassificationTypeRegistryService classificationTypeRegistryService)
		{
			_classificationTypeRegistryService = classificationTypeRegistryService;
		}

		public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
		{
			var type = _classificationTypeRegistryService.GetClassificationType("comment");

			return GetCommentSpans(span, type)
				.ToList();
		}

		private static IEnumerable<ClassificationSpan> GetCommentSpans(SnapshotSpan span, IClassificationType classificationType)
		{
			var text = span.GetText().Trim();

			if(text.StartsWith(CommentPrefix))
				yield return new ClassificationSpan(span, classificationType);
		}

		public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
	}
}