using System;
using System.Collections.Generic;
using System.Text;
using CSharpDynSQL.Models;
using CSharpDynSQL.Records;

namespace CSharpDynSQL {
	public enum WhereType {
		Between,
		Or,
		In,
	}

	public enum DataType {
		Varchar,
		Boolean,
		Int,
		Date
	}

	/**
		* Simple Active Record implementation
		* Note: This does not add any intelligens, stupid behaviour such
		* as calling an SELECT after a SET, broken SQL will remain broken :)
		*/
	public class DynSQL {
		const string DB_INSERT = "INSERT";
		const string DB_MYSQL_REPLACE = "REPLACE";
		const string DB_SELECT = "SELECT";
		const string DB_UPDATE = "UPDATE";
		const string DB_DELETE = "DELETE";
		const string DB_FROM = "FROM";
		const string DB_WHERE = "WHERE";
		const string DB_SET = "SET";
		const string DB_DROP = "DROP";

		public string PrepMySQLDate(DateTime dateObj) {
			//	dateObj.setHours(dateObj.getHours() - 2);
			//	return dateObj.toISOString().slice(0, 19).replace('T', ' ');
			return "";
		}

		public class DataColumn {
			DataType DataType;
			public string Name { get; set; }
			int Length;
			public object ValueTuple { get; set; }
			public DataColumn(object value) { }
		}

		private string DbName { get; set; }
		private IList<IDRecord>  DRecords { get; set; }

		public DynSQL(string dbName = "") {
			DbName = dbName;
			DRecords = new List<IDRecord>();
		}

		/**
 		 * For this sucker I actually performed a series of
 		 * performance benchmarks, this is (at least for this
 		 * app) the fastest and the most
  		 */
		public void Clear() {
			DRecords.Clear();
		}

		/**
		 * Returns the previous record from a given
		 * record in the record array
		 * @param {IDRecord} record
		 * @returns {IDRecord}
		 */
		private IDRecord GetPreviousRecord(IDRecord record) {
			IDRecord result = null;
			var index = DRecords.IndexOf(record);
			if (index > -1 && index -1 > 0) {
				result = DRecords[index] as IDRecord;
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elements"></param>
		/// <returns></returns>
		public DynSQL SelectAll(params string[] elements) {
			foreach (var element in elements) {
				DRecords.Add(new DSelectAll(element));
			}
			return this;
		}

		public DynSQL Select(params string[] elements) {
			foreach (var element in elements) {
				DRecords.Add(new DSelect(element));
			}
			return this;
		}

		public DynSQL Update(params string[] elements) {
			foreach (var element in elements) {
				DRecords.Add(new DUpdate(element));
			}
			return this;
		}

		//public DynSQL Insert(string tableName, params string[] elements) {
		public DynSQL Insert(string table, FieldList data, bool replace = false) {
			DRecords.Add(new DInsert(table, data, replace));

			/*
			foreach (var element in elements) {
				DRecords.Add(new DInsert(table, data);
			}
			*/
			return this;
		}

		public DynSQL With(params string[] elements) {
			foreach (var element in elements) {
				DRecords.Add(new DWith(element));
			}

			return this;
		}

		public DynSQL Into(params string[] elements) {
			foreach (var element in elements) {
				DRecords.Add(new DInto(element));
			}

			return this;
		}

		public DynSQL Set(string column, object value) {
			DRecords.Add(new DSet(column, value));
			return this;
		}

		public DynSQL LeftJoin(string table, string on) {
			DRecords.Add(new DLeftJoin(table, on));
			return this;
		}

		public DynSQL SelectAs(string fromTable, string alias = null) {
			DRecords.Add(new DSelect(fromTable, alias));
			return this;
		}

		public DynSQL From(string table, string alias = null) {
			var rec = new DFrom(table, alias);
			DRecords.Add(rec);
			return this;
		}

		private string PrepValue(object value) {
			var strValue = Convert.ToString(value);

			if (TypeUtils.IsString(value)) {
				strValue = "'" + strValue + "'";
			}
			else if (TypeUtils.IsNumber(value)) {
				//strValue = strValue;
			}
			else if (TypeUtils.IsDate(strValue)) {
				strValue = TypeUtils.PrepMySQLDate(strValue);
			}

			return strValue;
		}

		/**
			* Adds a Where record to the active record stack
			* @param thisElem
			* @param elemIs
			* @param escapeValue - set this to true when handling user inputted values, false when like "lucas.arts=rulez.row"
			* @returns {DynSQL}
			*/
		public DynSQL Where(string thisElem, object elemIs = null, bool escapeValue = true) {
			var equalValue = escapeValue ? PrepValue(elemIs) : Convert.ToString(elemIs);
			var rec = new DWhere(thisElem, equalValue);
			DRecords.Add(rec);

			return this;
		}

		public DynSQL WhereBetween(object value, int rangeStart, int rangeEnd) {
			value = PrepValue(value);
			var rec = new DWhereExt(WhereType.Between, value, rangeStart, rangeEnd);
			DRecords.Add(rec);
				
			return this;
		}

		public DynSQL OrderBy(string col) {
			var rec = new DOrderBy(col);
			DRecords.Add(rec);
			return this;
		}

		public DynSQL OrderByRandom() {
			var rec = new DOrderBy("RAND()");
			DRecords.Add(rec);
			return this;
		}

		public DynSQL And(string col, object equals = null) {
			var rec = new DAnd(col, equals);
			DRecords.Add(rec);
			return this;
		}

		public DynSQL LimitBy(int fromValue, int? toValue = null) {
			var rec = new DLimit(fromValue, toValue);
			DRecords.Add(rec);
			return this;
		}

		/**
			* Checks whether a given record position contains a
			* given instance type.
			* @param recordNum
			* @param recordInstanceType
			*/
		public bool IsExpectedRecord(int recordNum, IDRecord recordInstanceType) {
			var isExpected = false;
			var inRange = (recordNum <= DRecords.Count && recordNum >= 0);

			//TODO: Investigate instace comparison
			if (inRange && (DRecords[recordNum] == recordInstanceType)) {
				isExpected = true;
			}

			return isExpected;
		}

		public string ToSQL() {
			var sql = "";

			/**
				* Iterate the array on loopback for each type, that´s the most system
				* efficient and readable, don´t get confused by compiler masturbations
				* and smart array functions, they will boil down to something much
				* worse if you look behind the curtain.
				*/

			sql = ParseInsert(sql);
			sql = ParseSelect(sql);
			sql = ParseSelectAll(sql);
			sql = ParseUpdate(sql);
			sql = ParseSet(sql);
			sql = ParseFrom(sql);
			sql = ParseLeftJoin(sql);
			sql = ParseWhere(sql);
			sql = ParseAnd(sql);
			sql = ParseOrderBy(sql);
			sql = ParseLimit(sql);

			return sql;
		}

		////////////////////////////////////////
		// SELECT

		string ParseInsert(string sql) {
			const string KEY_DELIM = ", ";

			for (var i = 0; i < DRecords.Count; i++) {
				var record = DRecords[i];

				if (record is DInsert) {
					var result = new StringBuilder();
					var keys = new StringBuilder();
					var values = new StringBuilder();

					var dRec = record as DInsert;
					var type = dRec.MySQLReplace ? result.Append(DB_MYSQL_REPLACE) : result.Append(DB_INSERT);
					result.Append(" INTO ").Append(dRec.Table).Append(" (");

					for (var idx = 0; idx < dRec.FieldCount(); idx++) {
						var field = dRec.Data.Fields[idx];

						keys.Append(field.ColName);
						values.Append(field.Val());

						if (idx + 1 < dRec.FieldCount()) {
							keys.Append(KEY_DELIM);
							values.Append(KEY_DELIM);
						}
					}

					result.Append(keys);
					result.Append(") VALUES (");
					result.Append(values);
					result.Append(")");

					return result.ToString();
				}
			}



			/*
			var localCounter = 0;

			for (var i = 0; i< DRecords.Count; i++) {
				var record = DRecords[i];

				if (record is DInsert) {
					var dRec = record as DInsert;
					var type = dRec.MySQLReplace ? DB_MYSQL_REPLACE : DB_INSERT;

					sql = "INSERT (";

					for (var col = 0; col < dRec.Columns.Length; col++) {
 						sql += dRec.Columns[col];

						if (col<dRec.Columns.Length-1) {
							sql += ", ";
						}
					}

					sql += ") VALUES (";


					//TODO: Investigate WTF this is not implemented
					if (DRecords[i + 1] is DWith withRec)
						for (var val = 0; val < withRec.Data.Length; val++) {

						}

					localCounter++;
				}
			} // end for
			*/

			return sql;
		}

		////////////////////////////////////////
		// SELECT

		string ParseSelect(string sql) {
			var localCounter = 0;

			for (var i = 0; i < DRecords.Count; i++) {
				var record = DRecords[i];

				if (record is DSelect) {
					var dRec = record as DSelect;

					if (localCounter == 0) {
						sql += "SELECT";
					} else {
						sql += ",";
					}

					sql += " " + dRec.Column;

					localCounter++;
				}
			} // end for

			return sql;
		} // parseSelect

		string ParseSelectAll(string sql) {
			var localCounter = 0;

			for (var i = 0; i< DRecords.Count; i++) {

				var record = DRecords[i];

				if (record is DSelectAll) {
					var dRec = record as DSelectAll;

					if (localCounter == 0) {
						sql += "SELECT";
					} else {
						sql += ",";
					}

					sql += " " + dRec.Column + ".*";

					localCounter++;
				}
			} // end for

			return sql;
		} // parseSelect


		////////////////////////////////////////
		// FROM

		string ParseFrom(string sql) {
			var localCounter = 0;

			for (var i = 0; i< DRecords.Count; i++) {
				var record = DRecords[i];

				if (record is DFrom) {
					var rec = record as DFrom;

					if (localCounter == 0) {
						sql += " FROM";
					} else {
						sql += ",";
					}

					sql += " " + rec.Table;

					if (rec.Alias != null) {
						sql += " AS " + rec.Alias;
					}

					localCounter++;
				}
			}
			return sql;
		} // parseFrom

		////////////////////////////////////////
		// UPDATE

		string ParseUpdate(string sql) {
			for (var i = 0; i< DRecords.Count; i++) {
				var record = DRecords[i];

				if (record is DUpdate) {
					var rec = record as DUpdate;
					sql += "UPDATE " + rec.Table;
					break;
				}
			}
			return sql;
		}

		////////////////////////////////////////
		// SET

		string ParseSet(string sql) {
			DSet rec;

			var localCounter = 0;

			for (var i = 0; i< DRecords.Count; i++) {
				var record = DRecords[i];

				if (record is DSet) {
					rec = record as DSet;
					if (localCounter == 0) {
						sql += " SET";
					} else {
						sql += " ,";
 					}

					sql += " " + rec.Column + "='" + this.PrepValue(rec.Value)+"'";

					localCounter++;
				}
			} // end for

			return sql;
		}

		////////////////////////////////////////
		// LEFT JOIN

		string ParseLeftJoin(string sql) {
			var localCounter = 0;

			for (var i = 0; i< DRecords.Count; i++) {
				var record = DRecords[i];

				if (record is DLeftJoin) {
					var rec = record as DLeftJoin;

					sql += " LEFT JOIN " + rec.Table + " ON " + rec.On;
				}
			}
			return sql;
		} // parseLeftJoin

		////////////////////////////////////////
		// WHERE
		bool IsWhereRecord(object record) {
			return (record is DWhere || record is DWhereExt);
		}

		string ParseWhere(string sql) {
			var firstIteration = true;

			for (var i = 0; i< DRecords.Count; i++) {
				var record = DRecords[i];
				if (!this.IsWhereRecord(record)) continue;

				if (firstIteration) {
					sql += " WHERE";
				} else {
					sql += " AND";
				}

				if (record is DWhereExt) {
					var rec = record as DWhereExt;
					sql += " " + rec.That + " BETWEEN '"
					+ PrepValue(rec.Value1) + "' AND " + PrepValue(rec.Value2);
				}
				else if (record is DWhere) {
					var rec = record as DWhere;
					sql += " " + rec.That;

					if (rec.EqualValue != null)
						sql += "=" + rec.EqualValue;
					}

					firstIteration = false;
				}

				return sql;
			}

			////////////////////////////////////////
			// And
			string ParseAnd(string sql) {
				for (var i = 0; i< DRecords.Count; i++) {
					var record = DRecords[i];

					if (record is DAnd) {
						var rec = record as DAnd;
						sql += " AND " + rec.Col;
						sql += " = '" + PrepValue(rec.EqualValue) + "'";

						break;
					}
				}
				return sql;
			}

		////////////////////////////////////////
		// Order
		string ParseOrderBy(string sql) {
			for (var i = 0; i< DRecords.Count; i++) {
				var record = DRecords[i];

				if (record is DOrderBy) {
					var rec = record as DOrderBy;
					sql += " ORDER BY " + rec.Col;

					break;
				}
			}
			return sql;

		} // end parseOrderBy

		////////////////////////////////////////
		// Limit

		string ParseLimit(string sql) {
			for (var i = 0; i< DRecords.Count; i++) {
				var record = DRecords[i];

				if (record is DLimit) {
					var rec = record as DLimit;
					sql += " LIMIT " + rec.FromValue;

					if (rec.ToValue != null) {
						sql += ", " + rec.ToValue;
					}

					break;
				}
			}
			return sql;
		} // end parseLimit
	}
	//////////////////////////////////////////
}
