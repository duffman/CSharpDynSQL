using System;
using System.Globalization;
using System.Text;

namespace DynSql.Models {
	public class FieldVal {
		public string ColName { get; set; }

		public int? Int = null;
		public string Str = null;
		public bool? Bool = null;
		public double? Double = null;

		public FieldVal(string col, int val) {
			ColName = col;
			Int = val;
		}

		public FieldVal(string col, string val) {
			ColName = col;
			Str = val;
		}

		public FieldVal(string col, bool val) {
			ColName = col;
			Bool = val;
		}

		public FieldVal(string col, double val) {
			ColName = col;
			Double = val;
		}

		public string Val() {
			if (Int != null) {
				return Convert.ToString(Int);
			}

			if (Str != null) {
				var res = new StringBuilder("'");
				res.Append(TypeUtils.Escape(Str));
				res.Append("'");

				return res.ToString();
			}

			if (Bool != null) {
				var bVal = Bool.GetValueOrDefault();
				return bVal ? "true" : "false";
			}

			if (Double != null) {
				var dVal = Double.GetValueOrDefault();
				return Convert.ToString(dVal, CultureInfo.InvariantCulture);
			}

			return string.Empty;
		}

	}
}
