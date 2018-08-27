using System;
using System.Text;
using System.Text.RegularExpressions;
using DataDict = System.Collections.Generic.Dictionary<string, CSharpDynSQL.Models.FieldVal>;

namespace CSharpDynSQL {
	public class TypeUtils {
		public static bool IsString(object value) {
			return (value is string);
		}

		public static bool IsBoolean(object value) {
			var inputValue = Convert.ToString(value);
			return bool.TryParse(inputValue, out var boolValue);
		}

		//TODO: Implement
		public static bool IsDate(string value) {
			throw new NotImplementedException();
		}

		//TODO: Implement
		public static bool IsNumber(object value) {
			var inputValue = Convert.ToString(value);
			return int.TryParse(inputValue, out var intValue);
		}

		//TODO: Implement
		public static string PrepMySQLDate(string value) {
			throw new NotImplementedException();
		}

		public string PrepValue(string value) {
			if (TypeUtils.IsString(value)) {
				value = "'" + value + "'";
			}
			else if (TypeUtils.IsNumber(value)) {
				value = Convert.ToString(value);
			}
			else if (TypeUtils.IsDate(value)) {
				value = TypeUtils.PrepMySQLDate(value);
			}

			return value;
		}

		public string DataDictToString(DataDict data) {
			var result = new StringBuilder();

			return result.ToString();
		}

		public static string Escape(string str) {
			return Regex.Replace(str, @"[\x00'""\b\n\r\t\cZ\\%_]",
				delegate (Match match) {
					string v = match.Value;
					switch (v) {
						case "\x00":            // ASCII NUL (0x00) character
							return "\\0";
						case "\b":              // BACKSPACE character
							return "\\b";
						case "\n":              // NEWLINE (linefeed) character
							return "\\n";
						case "\r":              // CARRIAGE RETURN character
							return "\\r";
						case "\t":              // TAB
							return "\\t";
						case "\u001A":          // Ctrl-Z
							return "\\Z";
						default:
							return "\\" + v;
					}
				});
		}
	}
}
