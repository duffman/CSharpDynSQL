namespace DynSql.Records {
	public class DAnd : IDRecord {
		public string Col { get; set; }
		public object EqualValue { get; set; }

		public DAnd(string col, object equalValue = null) {
			Col = col;
			EqualValue = equalValue;
		}
	}
}