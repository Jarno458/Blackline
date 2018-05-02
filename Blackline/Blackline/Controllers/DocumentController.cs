using System.Linq;
using System.Net;
using System.Web.Http;
using Blackline.Data;
using Blackline.Models;

namespace Blackline.Controllers
{
	public class DocumentController : ApiController
	{
		readonly DocumentStore documentStore;

		public DocumentController()
		{
			documentStore = DocumentStore.Load();
		}

		public IHttpActionResult Get(int id)
		{
			var document = documentStore.Documents.FirstOrDefault(d => d.Id == id);
			
			if (document == null)
				return Content(HttpStatusCode.NotFound, "Document does not exsist");

			return Content(HttpStatusCode.OK, DocumentResponse.FromDocument(document));
		}
	}
}