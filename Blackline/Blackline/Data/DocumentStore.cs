using System;
using System.Collections.Generic;

namespace Blackline.Data
{
	public class DocumentStore
	{
		public IEnumerable<Document> Documents;

		public static DocumentStore Load()
		{
			return new DocumentStore
			{
				Documents = new []
				{
					new Document
					{
						Id = 1,
						Content = "This is a testing message from Jarno Westhof, Its content is inrelavant, do not read this message, contact: phone 0612345678",
						BlackLines = new []
						{
							new BlackLine { Type = BlackLineType.Personal, Text = "Jarno Westhof" },
							new BlackLine { Type = BlackLineType.Personal, Text = "0612345678" },
						}
					},
					new Document
					{
						Id = 1,
						Content = "something testing something",
						BlackLines = new []
						{
							new BlackLine { Type = BlackLineType.Personal, Text = "something" },
						}
					}
				}
			};
		}

		public void Save()
		{
			throw new NotImplementedException();
		}
	}

	public class Document
	{
		public int Id;
		public string Content;
		public string Owner;
		public IEnumerable<BlackLine> BlackLines;
	}

	public class BlackLine
	{
		public BlackLineType Type;
		public string Text;
		public int Length => Text.Length;
	}

	public enum BlackLineType
	{
		Personal,
		Critical,
		Medical
	}
}