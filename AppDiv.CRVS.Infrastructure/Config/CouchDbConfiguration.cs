namespace AppDiv.CRVS.Utility.Config
{
	public class CouchDbConfiguration
	{
		public static string CONFIGURATION_SECTION { get; set; } = "CouchDB";
		public string URL { get; set; } = null!;
		public string DbName { get; set; } = null!;
		public string UserName { get; set; } = null!;
		public string Password {get; set;} = null!;
	}
}
