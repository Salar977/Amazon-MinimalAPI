using Amazon.Models;
using Microsoft.AspNetCore.Mvc;

namespace Amazon.Repository
{
	public interface IAmazonRepository
	{
		Task<Books> AddBookAsync(Books book);
		Task<Books?> UpdateBookAsync(int id, Books book);
		Task<Books?> DeleteBookAsync(int id);
		Task<Books?> GetBookByIdAsync(int id);

		Task<IEnumerable<Books>> GetBooksAsync();
		Task<IEnumerable<Books>> GetBooksByTitleAsync(string? title);
		Task<IEnumerable<Books>> GetBooksByAuthorAsync(string? author);
		Task<IEnumerable<Books>> GetBooksByPublicationYearAsync(int? year);
		Task<IActionResult> DeleteSeveralBooksAsync(int[] ids);
	}
}
