namespace CSharpDynSQL.Records {
	public class DInto : IDRecord {
		public string TableName { get; set; }

		public DInto(string tableName) {
			TableName = tableName;
		}
	}
}