namespace CSharpDynSQL.Records {
	public class DWith : IDRecord {
		public string[] Data;
	
		public DWith(params string[] data) {
			Data = data;
		}
	}
}