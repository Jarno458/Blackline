using System;
using Blackline.Data;

namespace Blackline.Models
{
	public class DocumentResponse
	{
		public string Content;
		public string Owner;

		public static DocumentResponse FromDocument(Document document)
		{
			return new DocumentResponse
			{
				Owner = document.Owner,
				Content = GetBlacklinedContent(document)
			};
		}

		static string GetBlacklinedContent(Document document)
		{
			var content = document.Content;

			foreach (var blackLine in document.BlackLines)
				content = content.Replace(blackLine.Text, new string('█', blackLine.Length));

			return content;
		}
	}
}