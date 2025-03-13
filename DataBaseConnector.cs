
using MySqlConnector;


public class DataBaseConnector
{
	private readonly string server = "127.0.0.1";
	private readonly string port = "3307";
	private readonly string uid = "root";
	private readonly string pwd = "root";
	private readonly string database = "vn";
	public DataBaseConnector()
	{	
	}
	public MySqlConnection _getConnection() 
	{
		string connectionString = $"Server={server};Port={port};uid={uid};password={pwd};database={database}";
		MySqlConnection connection = new MySqlConnection(connectionString);
		return connection;
	}
	
}