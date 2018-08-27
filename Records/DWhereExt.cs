namespace CSharpDynSQL.Records {
	public class DWhereExt : IDRecord {
		public WhereType WhereType { get; set; }
		public object That { get; set; }
		public object Value1 { get; set; }
		public object Value2 { get; set; }

		public DWhereExt(WhereType type, object that, object value1, object value2 = null) {
			WhereType = type;
			That = that;
			Value1 = value1;
			Value2 = value2;
		}
	}
}