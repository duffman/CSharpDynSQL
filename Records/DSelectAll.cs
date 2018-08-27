namespace CSharpDynSQL.Records {
	public class DSelectAll : IDRecord {
		public string Column { get; set; }
		public bool HaveAlias { get; set; }

		public DSelectAll(string column, string alias = null) {
			HaveAlias = alias != null;
			Column = column;
		}
	}
}