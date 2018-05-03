using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;
using Blackline.Data;
using Blackline.Models;
using BlackLine = Blackline.Data.BlackLine;

namespace Blackline.Controllers
{
	public class DocumentController : ApiController
	{
		public IHttpActionResult Get(int id)
		{
			if(!DocumentStore.Documents.ContainsKey(id))
				return Content(HttpStatusCode.NotFound, @"Document does not exsist");

			return Content(HttpStatusCode.OK, DocumentResponse.FromDocument(DocumentStore.Documents[id], GetUserEmail()));
		}

		[HttpPost]
		[ActionName("Share")]
		public IHttpActionResult Share(int id, [FromBody] DocumentShareModel shareModel)
		{
			if (!DocumentStore.Documents.ContainsKey(id))
				return Content(HttpStatusCode.NotFound, @"Document does not exsist");

			if (shareModel == null)
				return Content(HttpStatusCode.BadRequest, @"plz provide the required shareModel as post data { ""Email"": ""mail@mail.com"", ""AutoDetects"": [ ""Money"", ""PostalCode"", n ], ""CustomBlackLines"": [ { type: ""personal"", text: ""text"" }, n ] }");

			if(string.IsNullOrEmpty(shareModel.Email))
				return Content(HttpStatusCode.BadRequest, @"plz provide the required shareModel as post data { ""Email"": ""mail@mail.com"", ""AutoDetects"": [ ""Money"", ""PostalCode"", n ], ""CustomBlackLines"": [ { type: ""personal"", text: ""text"" }, n ] }");

			var document = DocumentStore.Documents[id];
			if (document.Owner != GetUserEmail())
				return Content(HttpStatusCode.Forbidden, @"only the document owner can change the sharing permisions");

			var blacklines = new List<BlackLine>();

			if (shareModel.CustomBlackLines != null)
				blacklines.AddRange(shareModel.CustomBlackLines.Select(b => new BlackLine {Type = ParseBlacklineType(b.Type), Text = b.Text}));

			if (shareModel.AutoDetects != null)
				blacklines.AddRange(InformationDetection.GenerateBlackLinesForContent(document.Content, ParseInformationType(shareModel.AutoDetects)));

			document.Shares[shareModel.Email.ToLower()] = new Share { BlackLines = blacklines };
			
			DocumentStore.Save();

			return Content(HttpStatusCode.Created, $@"Shared with email: {shareModel.Email}");
		}

		SensativeInfomation[] ParseInformationType(string[] type)
		{
			return type.Select(ParseInformationType).Distinct().ToArray();
		}

		SensativeInfomation ParseInformationType(string type)
		{
			SensativeInfomation parsedType;
			return Enum.TryParse(type, true, out parsedType)
				? parsedType
				: SensativeInfomation.None;
		}

		BlackLineType ParseBlacklineType(string type)
		{
			BlackLineType parsedType;
			return Enum.TryParse(type, true, out parsedType) 
				? parsedType 
				: BlackLineType.OIther;
		}

		string GetUserEmail()
		{
			var authHeader = Request.Headers.Authorization;

			if (authHeader != null && authHeader.Scheme == "Basic")
			{
				var encodedUsernamePassword = authHeader.Parameter.Trim();
				var encoding = Encoding.GetEncoding("iso-8859-1");
				var usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

				var seperatorIndex = usernamePassword.IndexOf(':');

				var username = usernamePassword.Substring(0, seperatorIndex);
				//var password = usernamePassword.Substring(seperatorIndex + 1);

				return username.ToLower();
			}
			else
			{
				return "UNKNOWN";
			}
		}
	}
}