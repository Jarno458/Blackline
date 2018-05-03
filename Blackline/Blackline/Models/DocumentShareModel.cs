namespace Blackline.Models
{
	public class DocumentShareModel
	{
		public string Email;
		public string[] AutoDetects;
		public BlackLine[] CustomBlackLines;
	}

	public class BlackLine
	{
		public string Type;
		public string Text;
	}
}