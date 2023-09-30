using System.ComponentModel.DataAnnotations;

namespace Amazon.Models
{
	public class Books
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Title is required.")]
		public string? Title { get; set; }
		[Required(ErrorMessage = "Author is required.")]
		public string? Author { get; set; }
		[Range(1000, 2100, ErrorMessage = "Publication year must be between 1000 and 2100.")]
		public int PublicationYear { get; set; }
		public string? ISBN { get; set; }
		[MinLength(0, ErrorMessage = "Must be atleast 0")]
		public int InStock { get; set; }
	}
}
