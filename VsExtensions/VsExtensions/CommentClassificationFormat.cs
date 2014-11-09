using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace VsExtensions
{
	//[Export(typeof(EditorFormatDefinition))]
	//[Name("Comment")]
	//[DisplayName("Comment")]
	//[UserVisible(true)]
	//[Order(Before = Priority.High)]
	//internal class CommentClassificationFormat : ClassificationFormatDefinition
	//{
	//	public CommentClassificationFormat()
	//	{
	//		ForegroundColor = Colors.LightGray;
	//	}
	//}

	internal class CommentClassifier : IClassifier
	{
		private readonly IClassifier _aggregator;
		private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

		public CommentClassifier(IClassifier aggregator, IClassificationTypeRegistryService classificationTypeRegistryService)
		{
			_aggregator = aggregator;
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
			return span.Snapshot.Lines
				.Where(line => line.GetText().Trim().StartsWith("//"))
				.Select(line => new ClassificationSpan(new SnapshotSpan(line.Start, line.End), classificationType));
		}

		public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
	}

	[Export(typeof (IClassifierProvider))]
	[ContentType("text")]
	internal sealed class CommentClassifierProvider : IClassifierProvider
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
					new CommentClassifier(ClassifierAggregatorService.GetClassifier(textBuffer), ClassificationTypeRegistryService));
			}
			finally
			{
				_ignoreRequest = false;
			}
		}
	}
}