using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Blackline.Data
{
	public static class DocumentStore
	{
		public static Dictionary<int, Document> Documents;

		static DocumentStore()
		{
			Documents = LoadDocuments();

			DetectSensitiveInformation(Documents.Values);
		}

		static Dictionary<int, Document> LoadDocuments()
		{
			return new Dictionary<int, Document> {
				{ 1,  new Document
				{
					Id = 1,
					Owner = "owner@woltherskluwer.com",
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
				{ 2, new Document
				{
					Id = 2,
					Owner = "jarno.westhof@woltherskluwer.com",
					Content = "something testing something",
					SensativeInfomationTypes = new SensativeInfomation[0],
					Shares = new Dictionary<string, Share>
					{
						{
							"",
							new Share
							{
								BlackLines = new []
								{
									new BlackLine { Type = BlackLineType.Personal, Text = "something" },
								}
							}
						}
					}
				}},
				{ 3, new Document
				{
					Id = 3,
					Owner = "owner@woltherskluwer.com",
					Content = LoadFromFile(@"Data\LetterOfAgreement.html"),
					SensativeInfomationTypes = new []{ SensativeInfomation.PostalCode },
					Shares = new Dictionary<string, Share>
					{
						{
							"jarno.westhof@woltherskluwer.com",
							new Share
							{
								BlackLines = new []
								{
									new BlackLine { Type = BlackLineType.Personal, Text = "Lighthouse NV" },
									new BlackLine { Type = BlackLineType.Personal, Text = "Mainstreet 12" },
									new BlackLine { Type = BlackLineType.Personal, Text = "1234 AB" },
									new BlackLine { Type = BlackLineType.Personal, Text = "Frank" },
									new BlackLine { Type = BlackLineType.Personal, Text = "Anna" },
									new BlackLine { Type = BlackLineType.Personal, Text = "1212345678" },
								}
							}
						},
						{
							"Freddy.Nijzink@wolterskluwer.com",
							new Share
							{
								BlackLines = new []
								{
									new BlackLine { Type = BlackLineType.Personal, Text = "Lighthouse NV" },
									new BlackLine { Type = BlackLineType.Personal, Text = "Mainstreet 12" },
									new BlackLine { Type = BlackLineType.Personal, Text = "1234 AB" },
									new BlackLine { Type = BlackLineType.Personal, Text = "Amsterdam" },
									new BlackLine { Type = BlackLineType.Critical, Text = "€ 1000,-" },
									new BlackLine { Type = BlackLineType.Personal, Text = "1212345678" },
								}
							}
						},
						{
							"ahmet.bektes@wolterskluwer.com",
							new Share
							{
								BlackLines = new []
								{
									new BlackLine { Type = BlackLineType.Personal, Text = "Amsterdam" },
									new BlackLine { Type = BlackLineType.Critical, Text = "€ 1000,-" },
									new BlackLine { Type = BlackLineType.Personal, Text = "Frank" },
									new BlackLine { Type = BlackLineType.Personal, Text = "Anna" },
									new BlackLine { Type = BlackLineType.Personal, Text = "1212345678" },
								}
							}
						},
						{
							"Wytze.Sijtsma@wolterskluwer.com",
							new Share
							{
								BlackLines = new []
								{
									new BlackLine { Type = BlackLineType.Personal, Text = "Frank" },
									new BlackLine { Type = BlackLineType.Personal, Text = "Anna" },
									new BlackLine { Type = BlackLineType.Personal, Text = "1212345678" },
								}
							}
						}
					}
				}}
			};
		}

		static void DetectSensitiveInformation(IEnumerable<Document> documents)
		{
			foreach (var document in documents)
			{
				document.SensativeInfomationTypes = ExtractSensitiveInformationTypes(document.Content);
			}
		}

		internal static IEnumerable<SensativeInfomation> ExtractSensitiveInformationTypes(string content)
		{
			var sensitiveInformationTypes = new List<SensativeInfomation>();

			if (Regex.IsMatch(content, @"[1-9][0-9]{3}\s?[A-Za-z]{2}"))
				sensitiveInformationTypes.Add(SensativeInfomation.PostalCode);

			if (Regex.IsMatch(content, @"[a-zA-Z]{2}[0-9]{2}[a-zA-Z0-9]{4}[0-9]{7}([a-zA-Z0-9]?){0,16}"))
				sensitiveInformationTypes.Add(SensativeInfomation.IBan);

			if (Regex.IsMatch(content, @"(((0)[1-9]{2}[0-9][-]?[1-9][0-9]{5})|(\\+31|0|0031)[1-9][0-9][-]?[1-9][0-9]{6})")
				 || Regex.IsMatch(content, @"((\\+31|0|0031)6){1}\s?[1-9]{1}[0-9]{7}"))
				sensitiveInformationTypes.Add(SensativeInfomation.PhoneNumber);

			if (Regex.IsMatch(content, @"([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)"))
				sensitiveInformationTypes.Add(SensativeInfomation.Email);

			return sensitiveInformationTypes;
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

		public static void Save()
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
		public int Id;
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
		Email
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