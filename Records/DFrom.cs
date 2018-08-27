namespace CSharpDynSQL.Records {
	public class DFrom : IDRecord {
		public string Table { get; set; }
		public string Alias { get; set; }

		public DFrom(string table, string alias = null) {
			Table = table;
			Alias = alias;
		}
	}
}