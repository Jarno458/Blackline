using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Blackline.Data
{
	public static class DocumentStore
	{
		public static Dictionary<int, Document> Documents;

		static DocumentStore()
		{
			Documents = Load() ?? DefaultDocuments();

			DetectSensitiveInformation(Documents.Values);
		}

		static Dictionary<int, Document> DefaultDocuments()
		{
			return new Dictionary<int, Document> {
				{ 1,  new Document
				{
					Owner = "management@management.nl",
					Content = "This is a testing message from Jarno Westhof, Its content is inrelavant, do not read this message, contact: phone 0612345678",
					SensativeInfomationTypes = new []{ SensativeInfomation.PhoneNumber },
					Shares = new Dictionary<string, Share>
					{
						{
							"",
							new Share
							{
								BlackLines = new []
								{
									new BlackLine { Type = BlackLineType.Personal, Text = "Jarno Westhof" },
									new BlackLine { Type = BlackLineType.Personal, Text = "0612345678" },
								}
							}
						}
					}
				}},
				{ 3, new Document
				{
					Owner = "management@management.nl",
					Content = LoadFromFile(@"Data\LetterOfAgreement.html"),
					SensativeInfomationTypes = new []{ SensativeInfomation.PostalCode },
					Shares = new Dictionary<string, Share>
					{
						{
							"employee@employee.nl",
							new Share
							{
								BlackLines = new []
								{
									new BlackLine { Type = BlackLineType.Personal, Text = "1212345678" },
									new BlackLine { Type = BlackLineType.Critical, Text = "€ 1000,-" },
									new BlackLine { Type = BlackLineType.Personal, Text = "Frank" },
									new BlackLine { Type = BlackLineType.Personal, Text = "Anna" },
									new BlackLine { Type = BlackLineType.Personal, Text = "020-5432238" },
									new BlackLine { Type = BlackLineType.Personal, Text = "NL31INGB0673583801" },
								}
							}
						},
						{
							"client@client.nl",
							new Share
							{
								BlackLines = new []
								{
									new BlackLine { Type = BlackLineType.Personal, Text = "Lighthouse NV" },
									new BlackLine { Type = BlackLineType.Personal, Text = "Mainstreet 12" },
									new BlackLine { Type = BlackLineType.Personal, Text = "1234 AB" },
									new BlackLine { Type = BlackLineType.Personal, Text = "Amsterdam" },
								}
							}
						},
					}
				}}
			};
		}

		static void DetectSensitiveInformation(IEnumerable<Document> documents)
		{
			foreach (var document in documents)
			{
				document.SensativeInfomationTypes = InformationDetection.ExtractSensitiveInformationTypes(document.Content);
			}
		}



		static string LoadFromFile(string path)
		{
			try
			{
				var file = GetFilePath(path);
				using (StreamReader reader = new StreamReader(file))
					return reader.ReadToEnd();
			}
			catch
			{
				return "Failed to load file content";
			}
		}

		static Dictionary<int, Document> Load()
		{
			try
			{
				var file = GetFilePath(@"Data\Documents.json");
				using (StreamReader reader = new StreamReader(file))
					return JsonConvert.DeserializeObject<Dictionary<int, Document>>(reader.ReadToEnd());
			}
			catch
			{
				return null;
			}
		}

		internal static void Save()
		{
			try
			{
				var file = GetFilePath(@"Data\Documents.json");
				using (StreamWriter writer = new StreamWriter(file, false))
					writer.Write(JsonConvert.SerializeObject(Documents));
			}
			catch
			{
			}
		}

		static string GetFilePath(string path)
		{
			return Path.Combine(System.Web.HttpRuntime.BinDirectory, path);
		}
	}

	public class Document
	{
		public string Content;
		public string Owner;
		public IEnumerable<SensativeInfomation> SensativeInfomationTypes;
		public Dictionary<string, Share> Shares;
	}

	public enum SensativeInfomation
	{
		PhoneNumber,
		PostalCode,
		IBan,
		Email,
		Money,
		None
	}

	public class Share
	{
		public IEnumerable<BlackLine> BlackLines;
	}

	public class BlackLine
	{
		public BlackLineType Type;
		public string Text;

		public int Length => Text.Length;
	}
}