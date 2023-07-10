using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Models
{
	public partial record struct XsDuration
	{
		//普通は
		//System.Xml.XmlConvert.ToTimeSpan("PT25H")
		//とかで良いんだけど、TimeSpanは「1ヶ月後」とかを適切に表現できない。
		//スケジュール管理だと毎月のタスクとかあるからTimeSpanでは不適。
		//という訳でXsDurationを実装した。おそらく既にどこかにコードはあるだろう。
		//ISO 8601を正しく表現できてるかは怪しいが、必要十分。
		//特にマイナス関係は怪しいし、0.5ヶ月とかは表現できない。

		public int Years;
		public int Months;
		public int Days;
		public int Hours;
		public int Minutes;
		public double Seconds;

		public XsDuration(int year, int month, int day, int hours, int minutes, int second) : this()
		{
			Years = year;
			Months = month;
			Days = day;
			Hours = hours;
			Minutes = minutes;
			Seconds = second;
		}

		public XsDuration()
		{
		}

		public XsDuration(int year, int month, int day) : this()
		{
			Years = year;
			Months = month;
			Days = day;
		}

		public static bool TryParse(string text, out XsDuration duration)
		{
			var result = RegexDuration().Match(text);
			if (!result.Success)
			{
				duration = new XsDuration();
				return false;
			}
			sbyte sign = result.Groups[1].Value == "-" ? (sbyte)-1 : (sbyte)1;
			var nums = result.Groups.Values.Skip(2).SkipLast(1).Select(x => int.TryParse(x.Value, out int t) ? t : 0).ToArray();
			var sec = double.TryParse(result.Groups[7].Value, out double d) ? d : 0;
			duration = new XsDuration()
			{
				Years = nums[0] * sign,
				Months = nums[1] * sign,
				Days = nums[2] * sign,
				Hours = nums[3] * sign,
				Minutes = nums[4] * sign,
				Seconds = sec * sign,
			};
			return true;
		}

		public IEnumerable<double> Values
		{
			get
			{
				yield return Years;
				yield return Months;
				yield return Days;
				yield return Hours;
				yield return Minutes;
				yield return Seconds;
			}
		}

		public IEnumerable<int> ValuesInt
		{
			get
			{
				yield return Years;
				yield return Months;
				yield return Days;
				yield return Hours;
				yield return Minutes;
			}
		}

		public bool IsSingleUnit => Values.Count(a => a != 0) is 0 or 1;

		public Units FirstUnit
		{
			get
			{
				if (Years != 0) return Units.Years;
				if (Months != 0) return Units.Months;
				if (Days != 0) return Units.Days;
				if (Hours != 0) return Units.Hours;
				if (Minutes != 0) return Units.Minutes;
				if (Seconds != 0) return Units.Seconds;
				return Units.Days;
			}
		}

		public int FirstValue => ValuesInt.FirstOrDefault(a => a != 0);

		public enum Units
		{
			Years, Months, Days, Hours, Minutes, Seconds
		}

		public override string ToString()
		{
			sbyte positive = Values.FirstOrDefault(a => a != 0) >= 0 ? (sbyte)1 : (sbyte)-1;

			var sb = new StringBuilder();
			var inv = System.Globalization.CultureInfo.InvariantCulture;
			if (positive < 0) sb.Append("-");
			sb.Append("P");
			if (Years != 0) sb.Append(inv, $"{Years * positive}Y");
			if (Months != 0) sb.Append(inv, $"{Months * positive}M");
			if (Days != 0) sb.Append(inv, $"{Days * positive}D");
			if (Hours == 0 && Minutes == 0 && Seconds == 0) return sb.ToString();
			sb.Append("T");
			if (Hours != 0) sb.Append(inv, $"{Hours * positive}H");
			if (Minutes != 0) sb.Append(inv, $"{Minutes * positive}M");
			if (Seconds != 0) sb.Append(inv, $"{Seconds * positive}S");
			return sb.ToString();
		}

		public string ToStringJapanese()
		{
			sbyte positive = Values.FirstOrDefault(a => a != 0) >= 0 ? (sbyte)1 : (sbyte)-1;

			var sb = new StringBuilder();
			var inv = System.Globalization.CultureInfo.InvariantCulture;
			if (positive < 0) sb.Append("-");
			if (Years != 0) sb.Append(inv, $"{Years * positive}年");
			if (Months != 0) sb.Append(inv, $"{Months * positive}ヶ月");
			if (Days != 0) sb.Append(inv, $"{Days * positive}日");
			if (Hours == 0 && Minutes == 0 && Seconds == 0) return sb.ToString();
			if (Hours != 0) sb.Append(inv, $"{Hours * positive}時間");
			if (Minutes != 0) sb.Append(inv, $"{Minutes * positive}分");
			if (Seconds != 0) sb.Append(inv, $"{Seconds * positive}秒");
			return sb.ToString();
		}

		public DateTime GetAppended(DateTime t)
		{
			return t.AddYears(Years).AddMonths(Months).AddDays(Days).AddHours(Hours).AddMinutes(Minutes).AddSeconds(Seconds);
		}

		//負数の取り扱いなど若干の不安があります。
		[System.Text.RegularExpressions.GeneratedRegex("^(\\-?)P(?:(\\-?\\d+)Y)?(?:(\\-?\\d+)M)?(?:(\\-?\\d+)D)?(?:T(?:(\\d+)H)?(?:(\\d+)M)?(?:(\\d+\\.?\\d*)S)?)?$")]
		public static partial System.Text.RegularExpressions.Regex RegexDuration();
	}
}
