using Amazon.Models;
using Amazon.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Amazon.Endpoints
{
	public static class AmazonEndpoints
	{
		public static void MapAmazonEndPoints(this WebApplication app)
		{
			app.MapGet("/books", GetAllBooksAsync).WithOpenApi().WithName("GetAllBooksAsync");
			app.MapGet("/books/{id}", GetBookByIdAsync).WithOpenApi().WithName("GetBookByIdAsync");
			app.MapPost("/books", AddBookAsync).WithOpenApi().WithName("AddBookAsync");
			app.MapDelete("/books{id}", DeleteBookAsync).WithOpenApi().WithName("DeleteBookAsync");
			app.MapPut("/books{id}", UpdateBookAsync).WithOpenApi().WithName("UpdateBookAsync");
		}

		private static async Task<IActionResult> AddBookAsync(IAmazonRepository repo, Books book)
		{
			if(book != null)
			{
				var addedBook = await repo.AddBookAsync(book);
				if (addedBook != null) { return new OkObjectResult(addedBook); }
			}
			return new NotFoundObjectResult(book);
		}

		// https://localhost:4242/books
		private static async Task<IActionResult> GetAllBooksAsync(
			IAmazonRepository repo,
			ILogger<Program> logger,
			[FromQuery] string? title,
			[FromQuery] string? author,
			[FromQuery] int? publicationYear)
		{

			if(title != null)
			{
				var getBooksByTitle = await repo.GetBooksByTitleAsync(title);

				if (getBooksByTitle != null)
				{
					return new OkObjectResult(getBooksByTitle);
				}
			}

			if(author != null)
			{
				var getBooksByAuthor = await repo.GetBooksByAuthorAsync(author);

				if (getBooksByAuthor != null)
				{
					return new OkObjectResult(getBooksByAuthor);
				}
			}

			if (publicationYear != null)
			{
				var getBookByPublicationYear = await repo.GetBooksByPublicationYearAsync(publicationYear);

				if (getBookByPublicationYear != null)
				{
					return new OkObjectResult(getBookByPublicationYear);
				}
			}
			


			var allBooks = await repo.GetBooksAsync();
			if(allBooks != null)
			{

				return new OkObjectResult(allBooks);
			}
			return new NotFoundObjectResult(allBooks);
		}

		private static async Task<IActionResult> GetBookByIdAsync(IAmazonRepository repo, int id)
		{
			var bookById = await repo.GetBookByIdAsync(id);
			if(bookById != null) { return new OkObjectResult(bookById); }

			return new NotFoundObjectResult(bookById);
		}

		private static async Task<IActionResult> DeleteBookAsync(IAmazonRepository repo, int id)
		{
			var bookToDelete = await repo.DeleteBookAsync(id);
			if (bookToDelete != null) { return new OkObjectResult(bookToDelete); }

			return new NotFoundObjectResult(bookToDelete);
		}

		private static async Task<IActionResult> UpdateBookAsync(IAmazonRepository repo, int id, Books book)
		{
			if(book != null)
			{
				var personToUpdate = await repo.UpdateBookAsync(id, book);
				if(personToUpdate != null) { return new OkObjectResult(personToUpdate); }
			}
			return new NotFoundObjectResult(book);
		}
	}
}
