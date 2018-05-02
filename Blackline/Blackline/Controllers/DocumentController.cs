using System;
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
				return Content(HttpStatusCode.NotFound, "Document does not exsist");

			return Content(HttpStatusCode.OK, DocumentResponse.FromDocument(DocumentStore.Documents[id], GetUserEmail()));
		}

		[HttpPost]
		[ActionName("Share")]
		public IHttpActionResult Share(int id, string email, [FromBody] BlackLineModel blacklines)
		{
			if (!DocumentStore.Documents.ContainsKey(id))
				return Content(HttpStatusCode.NotFound, "Document does not exsist");

			if (blacklines == null)
				return Content(HttpStatusCode.BadRequest, @"plz provide the required blacklines or this user as post data { BlackLines: [ { type: ""personal"", text: ""text"" }, n ] }");

			var document = DocumentStore.Documents[id];
			if (document.Owner != GetUserEmail())
				return Content(HttpStatusCode.Forbidden, @"only the document owner can change the sharing permisions");
			
			var share = new Share
			{
				BlackLines = blacklines.BlackLines.Select(b => new BlackLine { Type = ParseInformationType(b.Type), Text = b.Text })
			};

			document.Shares[email] = share;

			DocumentStore.Save();

			return Content(HttpStatusCode.Created, "Shared with email");
		}

		BlackLineType ParseInformationType(string type)
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
				string encodedUsernamePassword = authHeader.Parameter.Trim();
				Encoding encoding = Encoding.GetEncoding("iso-8859-1");
				string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

				int seperatorIndex = usernamePassword.IndexOf(':');

				var username = usernamePassword.Substring(0, seperatorIndex);
				//var password = usernamePassword.Substring(seperatorIndex + 1);

				return username;
			}
			else
			{
				return "UNKNOWN";
			}
		}
	}
}