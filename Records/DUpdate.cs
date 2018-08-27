namespace CSharpDynSQL.Records {
	public class DUpdate : IDRecord {
		public string Table { get; set; }

		public DUpdate(string table) {
			Table = table;
		}
	}
}