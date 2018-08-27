namespace CSharpDynSQL.Records {
	public class DOrderBy : IDRecord {
		public string Col { get; set; }

		public DOrderBy(string col) {
			Col = col;
		}
	}
}