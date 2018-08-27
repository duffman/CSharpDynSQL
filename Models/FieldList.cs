namespace CSharpDynSQL.Models {
	public class FieldList {
		public FieldVal[] Fields;

		public FieldList(params FieldVal[] fields) {
			Fields = fields;
		}
	}
}
