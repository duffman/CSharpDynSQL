namespace CSharpDynSQL.Records {
	public class DSelect : IDRecord {
		public bool HaveAlias { get; set; }
		public string Column { get; set; }

		public DSelect(string column, string alias = null) {
			HaveAlias = alias != null;
		}
	}
}