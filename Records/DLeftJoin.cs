namespace CSharpDynSQL.Records {
	public class DLeftJoin : IDRecord {
		public string Table { get; set; }
		public string On { get; set; }

		public DLeftJoin(string table, string on) {
			Table = table;
			On = on;
		}
	}
}