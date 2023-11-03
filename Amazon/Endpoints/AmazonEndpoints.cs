using Amazon.Models;
using Amazon.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Amazon.Endpoints
{
	public static class AmazonEndpoints
	{
		public static void MapAmazonEndPoints(this WebApplication app)
		{
			app.MapGet("/books", GetAllBooksAsync).WithOpenApi().WithName("GetAllBooksAsync");
			app.MapGet("/books/{id}", GetBookByIdAsync).WithOpenApi().WithName("GetBookByIdAsync");
			app.MapPost("/books", AddBookAsync).WithOpenApi().WithName("AddBookAsync");
			app.MapDelete("/books/{id}", DeleteBookAsync).WithOpenApi().WithName("DeleteBookAsync");
			app.MapPut("/books/{id}", UpdateBookAsync).WithOpenApi().WithName("UpdateBookAsync");
		}

		private static async Task<IResult> AddBookAsync(IAmazonRepository repo, Books book)
		{
			if(book != null)
			{
				var addedBook = await repo.AddBookAsync(book);
				if (addedBook != null) { return Results.Ok(addedBook); }
			}
			return Results.NotFound();
		}

		// https://localhost:4242/books
		private static async Task<IResult> GetAllBooksAsync(
			IAmazonRepository repo,
			[FromQuery] string? title,
			[FromQuery] string? author,
			[FromQuery] int? publicationYear)
		{

			IEnumerable<Books> books = await repo.GetBooksAsync();

			if(title != null)
			{
				var getBooksByTitle = await repo.GetBooksByTitleAsync(title);

				if (getBooksByTitle != null)
				{
					books = books.Intersect(getBooksByTitle);
				}
			}

			if(author != null)
			{
				var getBooksByAuthor = await repo.GetBooksByAuthorAsync(author);

				if (getBooksByAuthor != null)
				{
                    books = books.Intersect(getBooksByAuthor);
                }
			}

			if (publicationYear != null)
			{
				var getBookByPublicationYear = await repo.GetBooksByPublicationYearAsync(publicationYear);

				if (getBookByPublicationYear != null)
				{
                    books = books.Intersect(getBookByPublicationYear);
                }
			}

			if(books.Any())
			{

				return Results.Ok(books);
			}

			return Results.NotFound(books);
		}

		private static async Task<IResult> GetBookByIdAsync(IAmazonRepository repo, int id)
		{
			var bookById = await repo.GetBookByIdAsync(id);
			if(bookById != null) { return Results.Ok(bookById); }

			return Results.NotFound(bookById);
		}

		private static async Task<IResult> DeleteBookAsync(IAmazonRepository repo, int id)
		{
			var bookToDelete = await repo.DeleteBookAsync(id);
			if (bookToDelete != null) { return Results.Ok(bookToDelete); }

			return Results.NotFound(bookToDelete);
		}

		private static async Task<IResult> UpdateBookAsync(IAmazonRepository repo, int id, Books book)
		{
			if(book != null)
			{
				var personToUpdate = await repo.UpdateBookAsync(id, book);
				if(personToUpdate != null) { return Results.Ok(personToUpdate); }
			}
			return Results.NotFound(book);
		}
	}
}
