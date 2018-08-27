using System.Collections.Generic;
using DynSql.Models;

namespace DynSql.Records {
	public class DInsert : IDRecord {
		public string Table { get; set; }
		public bool MySQLReplace { get; set; }
		public FieldList Data { get; set; }
//		public string[] Columns;

		//public DInsert(params string[] columns) {
		public DInsert(string table, FieldList data, bool replace) {
			Table = table;
			Data = data;
			MySQLReplace = replace;
			// Columns = columns;
		}

		public int FieldCount() {
			return Data.Fields.Length;
		}
	}
}