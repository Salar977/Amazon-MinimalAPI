using Amazon.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace Amazon.Repository
{
	public class AmazonDbHandler : IAmazonRepository
	{
		// trenger connection string
		private readonly string? _connectionString;
		private readonly ILogger<AmazonDbHandler> _logger;

		// hvordan får vi tak i den => DI-container (dependency Injection)
		public AmazonDbHandler(IConfiguration config, ILogger<AmazonDbHandler> logger)
		{
			_connectionString = config.GetConnectionString("DefaultConnection");
			_connectionString = _connectionString?
				.Replace("{DB_AMAZON_USERNAME}", Environment.GetEnvironmentVariable("DB_AMAZON_USERNAME"))
				.Replace("{DB_AMAZON_PASSWORD}", Environment.GetEnvironmentVariable("DB_AMAZON_PASSWORD"));

			_logger = logger;
		}

		public async Task<IEnumerable<Books>> GetBooksAsync()
		{
			_logger.LogDebug("Get all books.");

			var bookList = new List<Books>();
			using MySqlConnection connection = new(_connectionString);
			await connection.OpenAsync();

			MySqlCommand cmd = new("SELECT * FROM Books", connection);

			using var reader = await cmd.ExecuteReaderAsync();
			while (await reader.ReadAsync()) // Await here to read rows asynchronously
			{
				var book = new Books()
				{
					Id = reader.GetInt32("Id"),
					Title = reader.GetString("Title"),
					Author = reader.GetString("Author"),
					PublicationYear = reader.GetInt32("PublicationYear"),
					ISBN = reader.GetString("ISBN"),
					InStock = reader.GetInt32("InStock")
				};
				bookList.Add(book);
			}
			return bookList;
		}
		public async Task<IEnumerable<Books>> GetBooksByTitleAsync(string? title)
		{
			_logger.LogDebug($"Get book by title: {title}");

			var bookByTitleList = new List<Books>();
			using MySqlConnection connection = new(_connectionString);
			await connection.OpenAsync();

			MySqlCommand cmd = new("SELECT * FROM Books WHERE Title LIKE @Title", connection);
			cmd.Parameters.AddWithValue("@Title", $"%{title}%");

			using var reader = await cmd.ExecuteReaderAsync();

			while (await reader.ReadAsync())
			{
				var bookByTitle = new Books
				{
					Id = reader.GetInt32("Id"),
					Title = reader.GetString("Title"),
					Author = reader.GetString("Author"),
					PublicationYear = reader.GetInt32("PublicationYear"),
					ISBN = reader.GetString("ISBN"),
					InStock = reader.GetInt32("InStock")
				};
				bookByTitleList.Add(bookByTitle);
			}
			return bookByTitleList;
		}
		public async Task<IEnumerable<Books>> GetBooksByAuthorAsync(string? author)
		{
			_logger.LogDebug($"Get book by author: {author}");

			var bookByAuthorList = new List<Books>();
			using MySqlConnection connection = new(_connectionString);
			await connection.OpenAsync();

			MySqlCommand cmd = new("SELECT * FROM Books WHERE Author LIKE @Author", connection);
			cmd.Parameters.AddWithValue("@Author", $"%{author}%");

			using var reader = await cmd.ExecuteReaderAsync();

			while (await reader.ReadAsync())
			{
				var bookByAuthor = new Books
				{
					Id = reader.GetInt32("Id"),
					Title = reader.GetString("Title"),
					Author = reader.GetString("Author"),
					PublicationYear = reader.GetInt32("PublicationYear"),
					ISBN = reader.GetString("ISBN"),
					InStock = reader.GetInt32("InStock")
				};
				bookByAuthorList.Add(bookByAuthor);
			}
			return bookByAuthorList;
		}
		public async Task<IEnumerable<Books>> GetBooksByPublicationYearAsync(int? publicationYear)
		{
			_logger.LogDebug($"Get book by publication year: {publicationYear}");

			var bookByPublicationYearList = new List<Books>();
			using MySqlConnection connection = new(_connectionString);
			await connection.OpenAsync();

			MySqlCommand cmd = new("SELECT * FROM Books WHERE PublicationYear = @PublicationYear", connection);
			cmd.Parameters.AddWithValue("@PublicationYear", publicationYear);

			using var reader = await cmd.ExecuteReaderAsync();

			while (await reader.ReadAsync())
			{
				var bookByPublicationYear = new Books
				{
					Id = reader.GetInt32("Id"),
					Title = reader.GetString("Title"),
					Author = reader.GetString("Author"),
					PublicationYear = reader.GetInt32("PublicationYear"),
					ISBN = reader.GetString("ISBN"),
					InStock = reader.GetInt32("InStock")
				};
				bookByPublicationYearList.Add(bookByPublicationYear);
			}
			return bookByPublicationYearList;
		}

		public async Task<Books?> GetBookByIdAsync(int id)
		{
			_logger.LogDebug($"Get book by id: {id}");

			using MySqlConnection connection = new(_connectionString);
			await connection.OpenAsync();

			MySqlCommand cmd = new("SELECT * FROM Books WHERE Id = @Id", connection);
			cmd.Parameters.AddWithValue("@Id", id);

			using var reader = await cmd.ExecuteReaderAsync();

			while (await reader.ReadAsync())
			{
				return new Books
				{
					Id = reader.GetInt32("Id"),
					Title = reader.GetString("Title"),
					Author = reader.GetString("Author"),
					PublicationYear = reader.GetInt32("PublicationYear"),
					ISBN = reader.GetString("ISBN"),
					InStock = reader.GetInt32("InStock")
				};
			}
			return null;
		}

		public async Task<Books> AddBookAsync(Books book)
		{

			_logger?.LogDebug("Added a new book: {@Book}", book);

			// Exception handling, Logging
			using MySqlConnection connection = new(_connectionString);
			await connection.OpenAsync();

			// lage query
			MySqlCommand cmd = new("INSERT INTO Books (Title, Author, PublicationYear, ISBN, InStock) " +
				"VALUES (@Title, @Author, @PublicationYear, @ISBN, @InStock)", connection);

			cmd.Parameters.AddWithValue("@Title", book.Title);
			cmd.Parameters.AddWithValue("@Author", book.Author);
			cmd.Parameters.AddWithValue("@PublicationYear", book.PublicationYear);
			cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
			cmd.Parameters.AddWithValue("@InStock", book.InStock);

			var rowsAffected = await cmd.ExecuteNonQueryAsync();

			cmd.CommandText = "SELECT LAST_INSERT_ID()";
			var lastIdObject = await cmd.ExecuteScalarAsync();

			book.Id = Convert.ToInt32(lastIdObject);
			return book;
		}

		public async Task<Books?> UpdateBookAsync(int id, Books book)
		{
			_logger.LogDebug($"Updated book: {book}");

			using MySqlConnection connection = new(_connectionString);
			await connection.OpenAsync();


			// starter en transaksjon
			MySqlTransaction mySqlTransaction = await connection.BeginTransactionAsync();

			try
			{
				MySqlCommand cmd = new(
				"UPDATE Books " +
				"SET Title=@Title, Author=@Author, PublicationYear=@PublicationYear, " +
				"ISBN=@ISBN, InStock=@InStock " +
				"WHERE Id=@Id", connection);

				cmd.Parameters.AddWithValue("@Title", book.Title);
				cmd.Parameters.AddWithValue("@Author", book.Author);
				cmd.Parameters.AddWithValue("@PublicationYear", book.PublicationYear);
				cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
				cmd.Parameters.AddWithValue("@InStock", book.InStock);
				cmd.Parameters.AddWithValue("@Id", id);

				var rowsAffected = await cmd.ExecuteNonQueryAsync();

				if (rowsAffected == 0)
				{
					await mySqlTransaction.RollbackAsync();
					return null;
				}

				// gjør endringer i databasen
				await mySqlTransaction.CommitAsync();

				return await GetBookByIdAsync(id);
			}
			catch (Exception ex)
			{
				try
				{
					await mySqlTransaction.RollbackAsync();
				}
				catch
				{
					// error in transaction rollback
					await Console.Out.WriteLineAsync(ex.Message);
				}
				// throw the original exception with the stack trace
				_logger.LogError("Failed to update database", ex.StackTrace);
				throw;
			}
		}

		public async Task<Books?> DeleteBookAsync(int id)
		{
			// henter person fra db
			var bookToDelete = await GetBookByIdAsync(id);

			_logger.LogDebug($"Deleted book: {bookToDelete}");

			if (bookToDelete == null) { return null; }

			using MySqlConnection conn = new(_connectionString);
			await conn.OpenAsync();

			// cmd med spørring og knytting mot connection
			MySqlCommand cmd = new("DELETE FROM Books WHERE Id=@Id", conn);
			cmd.Parameters.AddWithValue("@Id", id);

			int rofsAffected = await cmd.ExecuteNonQueryAsync();

			if (rofsAffected == 0) { return null; }

			return bookToDelete;
		}
	}
}
