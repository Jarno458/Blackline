using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blackline.Data
{
	public class InformationDetection
	{
		static readonly Dictionary<SensativeInfomation, IEnumerable<Regex>> Regexes = new Dictionary<SensativeInfomation, IEnumerable<Regex>>
		{
			{SensativeInfomation.PostalCode, new[] {new Regex(@"(?<value>[1-9][0-9]{3}\s?[A-Za-z]{2})")}},
			{SensativeInfomation.IBan, new[] {new Regex(@"(?<value>[a-zA-Z]{2}[0-9]{2}[a-zA-Z0-9]{4}[0-9]{7}[a-zA-Z0-9]?{0,16})")}},
			{
				SensativeInfomation.PhoneNumber, new[]
				{
					new Regex(@"(?<value>((0[1-9]{2}[0-9][-]?[1-9][0-9]{5})|(\\+31|0|0031)[1-9][0-9][-]?[1-9][0-9]{6}))"),
					new Regex(@"(?<value>(\\+31|0|0031)6{1}\s?[1-9]{1}[0-9]{7})")
				}
			},
			{SensativeInfomation.Email, new[] { new Regex(@"(?<value>[\w-\.]+@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|([\w-]+\.+))[a-zA-Z]{2,4}|[0-9]{1,3}\]?)")}},
			{SensativeInfomation.Money, new[] {new Regex(@"(?<value>[€$]?\s?[0-9]+[.,]([0-9]|-)+)")}}
		};

		public static IEnumerable<SensativeInfomation> ExtractSensitiveInformationTypes(string content)
		{
			var sensitiveInformationTypes = new List<SensativeInfomation>();

			foreach (var regex in Regexes)
				if(regex.Value.Any(r => r.IsMatch(content)))
					sensitiveInformationTypes.Add(regex.Key);

			return sensitiveInformationTypes;
		}

		public static IEnumerable<BlackLine> GenerateBlackLinesForContent(string content,
			params SensativeInfomation[] sensativeInfomations)
		{
			var blacklines = new List<BlackLine>();

			foreach (var informationType in sensativeInfomations)
			{
				if(informationType == SensativeInfomation.None)
					continue;

				var regexes = Regexes[informationType];

				foreach (var regex in regexes)
				{
					var result = regex.Matches(content);

					var blacklineType = GetBlacklineType(informationType);

					foreach (Match match in result)
						blacklines.Add(new BlackLine {Text = match.Groups["value"].Value, Type = blacklineType });
				}
			}

			return blacklines;
		}

		static BlackLineType GetBlacklineType(SensativeInfomation informationType)
		{
			return informationType == SensativeInfomation.Money ? BlackLineType.Critical : BlackLineType.Personal;
		}
	}
}