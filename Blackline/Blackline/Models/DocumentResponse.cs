using System.Linq;
using Blackline.Data;
using Microsoft.Ajax.Utilities;

namespace Blackline.Models
{
	public class DocumentResponse
	{
		public string Content;
		public string Owner;
		public string[] SensativeInfomationTypes;

		public static DocumentResponse FromDocument(Document document, string email)
		{
			return new DocumentResponse
			{
				Owner = document.Owner,
				SensativeInfomationTypes = document.SensativeInfomationTypes.Select(sit => sit.ToString().ToLower()).ToArray(),
				Content = GetBlacklinedContent(document, email),
			};
		}

		static string GetBlacklinedContent(Document document, string email)
		{
			var content = document.Content;

			var blacklines = document.Shares.ContainsKey(email) 
				? document.Shares[email].BlackLines 
				: document.Shares.Values.SelectMany(s => s.BlackLines).DistinctBy(b => b.Text);
			
			foreach (var blackLine in blacklines)
				content = content.Replace(blackLine.Text, CreateBlackout(blackLine.Type, blackLine.Length));

			return content;
		}

		static string CreateBlackout(BlackLineType type, int size)
		{
			return $@"<span class=""blacked {type.ToString().ToLower()}"">{string.Concat(Enumerable.Repeat("&nbsp;", size))}</span>";
		}
	}
}