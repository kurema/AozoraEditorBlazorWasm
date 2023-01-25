﻿// Generated on https://app.quicktype.io/?l=csharp
// Language: C#
// Serialization framework: System Text Json
// Generated namespace: AozoraEditor
// Use T[] or List<T>: List
// Output features: Complete
// Generate virtual properties: off
// Fail if required properties are missing: off
// C# version: 6
// Property density: Normal
// Type to use for numbers: Double
// Type to use for "any": Object
// Base class: Object

// <auto-generated />
//
// To parse this JSON data, add NuGet 'System.Text.Json' then do:
//
//    using AozoraEditor;
//
//    var snippets = Snippets.FromJson(jsonString);

#nullable enable
#pragma warning disable CS8618
#pragma warning disable CS8601
#pragma warning disable CS8603

namespace AozoraEditor.Shared.Snippets
{
	using System;
	using System.Collections.Generic;

	using System.Text.Json;
	using System.Text.Json.Serialization;
	using System.Globalization;

	public partial class Snippets
	{
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("contents")]
		public List<Content> Contents { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("keywords")]
		public List<Keyword> Keywords { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("templates")]
		public List<Template> Templates { get; set; }
	}

	public partial class Content
	{
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("arg_types")]
		public List<ArgType> ArgTypes { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("comment")]
		public Comment? Comment { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("description")]
		public string Description { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("documentLink")]
		public Uri DocumentLink { get; set; }

		[JsonPropertyName("labels")]
		public ContentLabels Labels { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("obsolete")]
		public bool? Obsolete { get; set; }

		[JsonPropertyName("templates")]
		public List<string> Templates { get; set; }

		[JsonPropertyName("text")]
		public List<string> Text { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("words")]
		public List<Word> Words { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("words_id")]
		public string WordsId { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("words_ref")]
		public string WordsRef { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("wordsLabelAutoCase")]
		public bool? WordsLabelAutoCase { get; set; }
	}

	public partial class ContentLabels
	{
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("en")]
		public List<string> En { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("jp")]
		public List<string> Jp { get; set; }

		[JsonPropertyName("jp_fully_roman")]
		public List<string> JpFullyRoman { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("short_en")]
		public List<string> ShortEn { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("short_jp")]
		public List<string> ShortJp { get; set; }
	}

	public partial class Word
	{
		[JsonPropertyName("labels")]
		public WordLabels Labels { get; set; }

		[JsonPropertyName("text")]
		public string Text { get; set; }
	}

	public partial class WordLabels
	{
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("en")]
		public List<string> En { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("jp")]
		public List<string> Jp { get; set; }

		[JsonPropertyName("jp_fully_roman")]
		public List<string> JpFullyRoman { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("short_en")]
		public List<string> ShortEn { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("short_jp")]
		public List<string> ShortJp { get; set; }
	}

	public partial class Keyword
	{
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("comment")]
		public Comment? Comment { get; set; }

		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("key")]
		public string Key { get; set; }

		[JsonPropertyName("text")]
		public string Text { get; set; }
	}

	public partial class Template
	{
		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("labels")]
		public TemplateLabels Labels { get; set; }

		[JsonPropertyName("text")]
		public List<string> Text { get; set; }
	}

	public partial class TemplateLabels
	{
		[JsonPropertyName("en")]
		public string En { get; set; }

		[JsonPropertyName("jp")]
		public string Jp { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("jp_fully_roman")]
		public string JpFullyRoman { get; set; }

		[JsonPropertyName("short_en")]
		public string ShortEn { get; set; }

		[JsonPropertyName("short_jp")]
		public string ShortJp { get; set; }
	}

	public enum ArgType { Alphabet, Any, NumberFull, NumberHalf, Ref };

	public partial struct Comment
	{
		public string String;
		public List<string> StringArray;

		public static implicit operator Comment(string String) => new Comment { String = String };
		public static implicit operator Comment(List<string> StringArray) => new Comment { StringArray = StringArray };
	}

	public partial class Snippets
	{
		public static Snippets FromJson(string json) => JsonSerializer.Deserialize<Snippets>(json, AozoraEditor.Shared.Snippets.Converter.Settings);
	}

	public static class Serialize
	{
		public static string ToJson(this Snippets self) => JsonSerializer.Serialize(self, AozoraEditor.Shared.Snippets.Converter.Settings);
	}

	internal static class Converter
	{
		public static readonly JsonSerializerOptions Settings = new(JsonSerializerDefaults.General)
		{
			Converters =
			{
				ArgTypeConverter.Singleton,
				CommentConverter.Singleton,
				new DateOnlyConverter(),
				new TimeOnlyConverter(),
				IsoDateTimeOffsetConverter.Singleton
			},
		};
	}

	internal class ArgTypeConverter : JsonConverter<ArgType>
	{
		public override bool CanConvert(Type t) => t == typeof(ArgType);

		public override ArgType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var value = reader.GetString();
			switch (value)
			{
				case "alphabet":
					return ArgType.Alphabet;
				case "any":
					return ArgType.Any;
				case "number_full":
					return ArgType.NumberFull;
				case "number_half":
					return ArgType.NumberHalf;
				case "ref":
					return ArgType.Ref;
			}
			throw new Exception("Cannot unmarshal type ArgType");
		}

		public override void Write(Utf8JsonWriter writer, ArgType value, JsonSerializerOptions options)
		{
			switch (value)
			{
				case ArgType.Alphabet:
					JsonSerializer.Serialize(writer, "alphabet", options);
					return;
				case ArgType.Any:
					JsonSerializer.Serialize(writer, "any", options);
					return;
				case ArgType.NumberFull:
					JsonSerializer.Serialize(writer, "number_full", options);
					return;
				case ArgType.NumberHalf:
					JsonSerializer.Serialize(writer, "number_half", options);
					return;
				case ArgType.Ref:
					JsonSerializer.Serialize(writer, "ref", options);
					return;
			}
			throw new Exception("Cannot marshal type ArgType");
		}

		public static readonly ArgTypeConverter Singleton = new ArgTypeConverter();
	}

	internal class CommentConverter : JsonConverter<Comment>
	{
		public override bool CanConvert(Type t) => t == typeof(Comment);

		public override Comment Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			switch (reader.TokenType)
			{
				case JsonTokenType.String:
					var stringValue = reader.GetString();
					return new Comment { String = stringValue };
				case JsonTokenType.StartArray:
					var arrayValue = JsonSerializer.Deserialize<List<string>>(ref reader, options);
					return new Comment { StringArray = arrayValue };
			}
			throw new Exception("Cannot unmarshal type Comment");
		}

		public override void Write(Utf8JsonWriter writer, Comment value, JsonSerializerOptions options)
		{
			if (value.String != null)
			{
				JsonSerializer.Serialize(writer, value.String, options);
				return;
			}
			if (value.StringArray != null)
			{
				JsonSerializer.Serialize(writer, value.StringArray, options);
				return;
			}
			throw new Exception("Cannot marshal type Comment");
		}

		public static readonly CommentConverter Singleton = new CommentConverter();
	}

	public class DateOnlyConverter : JsonConverter<DateOnly>
	{
		private readonly string serializationFormat;
		public DateOnlyConverter() : this(null) { }

		public DateOnlyConverter(string? serializationFormat)
		{
			this.serializationFormat = serializationFormat ?? "yyyy-MM-dd";
		}

		public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var value = reader.GetString();
			return DateOnly.Parse(value!);
		}

		public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value.ToString(serializationFormat));
	}

	public class TimeOnlyConverter : JsonConverter<TimeOnly>
	{
		private readonly string serializationFormat;

		public TimeOnlyConverter() : this(null) { }

		public TimeOnlyConverter(string? serializationFormat)
		{
			this.serializationFormat = serializationFormat ?? "HH:mm:ss.fff";
		}

		public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var value = reader.GetString();
			return TimeOnly.Parse(value!);
		}

		public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value.ToString(serializationFormat));
	}

	internal class IsoDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
	{
		public override bool CanConvert(Type t) => t == typeof(DateTimeOffset);

		private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

		private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
		private string? _dateTimeFormat;
		private CultureInfo? _culture;

		public DateTimeStyles DateTimeStyles
		{
			get => _dateTimeStyles;
			set => _dateTimeStyles = value;
		}

		public string? DateTimeFormat
		{
			get => _dateTimeFormat ?? string.Empty;
			set => _dateTimeFormat = (string.IsNullOrEmpty(value)) ? null : value;
		}

		public CultureInfo Culture
		{
			get => _culture ?? CultureInfo.CurrentCulture;
			set => _culture = value;
		}

		public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
		{
			string text;


			if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
				|| (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
			{
				value = value.ToUniversalTime();
			}

			text = value.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);

			writer.WriteStringValue(text);
		}

		public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			string? dateText = reader.GetString();

			if (string.IsNullOrEmpty(dateText) == false)
			{
				if (!string.IsNullOrEmpty(_dateTimeFormat))
				{
					return DateTimeOffset.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
				}
				else
				{
					return DateTimeOffset.Parse(dateText, Culture, _dateTimeStyles);
				}
			}
			else
			{
				return default(DateTimeOffset);
			}
		}


		public static readonly IsoDateTimeOffsetConverter Singleton = new IsoDateTimeOffsetConverter();
	}
}
#pragma warning restore CS8618
#pragma warning restore CS8601
#pragma warning restore CS8603