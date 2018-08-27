namespace CSharpDynSQL.Records {
	public class DLimit : IDRecord {
		public int FromValue { get; set; }
		public int? ToValue { get; set; }

		public DLimit(int fromValue, int? toValue) {
			FromValue = fromValue;
			ToValue = toValue;
		}
	}
}