namespace CSharpDynSQL.Records {
	public class DWhere : IDRecord {
		public string That { get; set; }
		public string EqualValue { get; set; }

		public DWhere(string that, string equalValue = null) {
			That = that;
			EqualValue = equalValue;
		}
	}
}