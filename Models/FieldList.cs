using System.Text;
using System.Xml;

namespace DynSql.Models {
	public class FieldList {
		public FieldVal[] Fields;

		public FieldList(params FieldVal[] fields) {
			Fields = fields;
		}
	}
}
