using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace ExtensionsLibrary.Tools;

[McpServerToolType]
public class BookDetailsTool
{
    private readonly string BooksJson = """
    {
        "books": [
            {
                "title": "Introduction to Algorithms",
                "author": "Thomas H. Cormen",
                "isbn": "9780262033848",
                "price": 89.99,
                "category": "Algorithms",
                "description": "A comprehensive book covering a broad range of algorithms in depth."
            },
            {
                "title": "Clean Code",
                "author": "Robert C. Martin",
                "isbn": "9780132350884",
                "price": 49.99,
                "category": "Software Engineering",
                "description": "A handbook of agile software craftsmanship with a focus on writing clean code."
            },
            {
                "title": "Design Patterns",
                "author": "Erich Gamma",
                "isbn": "9780201633610",
                "price": 59.99,
                "category": "Architecture",
                "description": "Elements of reusable object-oriented software and design principles."
            },
            {
                "title": "The Pragmatic Programmer",
                "author": "Andy Hunt",
                "isbn": "9780135957059",
                "price": 44.99,
                "category": "Programming",
                "description": "Your journey to mastery in modern software development."
            }
        ]
    }
    """;

    [McpServerTool, Description("Get all available computer books.")]
    public Task<string> GetAllBooks()
    {
        var jsonDoc = JsonDocument.Parse(BooksJson);
        var books = jsonDoc.RootElement.GetProperty("books").EnumerateArray();

        var bookList = books.Select(book => $"""
            Title: {book.GetProperty("title").GetString()}
            Author: {book.GetProperty("author").GetString()}
            ISBN: {book.GetProperty("isbn").GetString()}
            Price: ${book.GetProperty("price").GetDecimal()}
            Category: {book.GetProperty("category").GetString()}
            Description: {book.GetProperty("description").GetString()}
            """);

        return Task.FromResult(string.Join("\n--\n", bookList));
    }
                                                        
    [McpServerTool, Description("Find books by category.")]
    public Task<string> GetBooksByCategory(
        [Description("Category of the book (e.g., Algorithms, Programming).")] string category)
    {
        var jsonDoc = JsonDocument.Parse(BooksJson);
        var books = jsonDoc.RootElement.GetProperty("books").EnumerateArray();

        var matchedBooks = books
            .Where(book => string.Equals(book.GetProperty("category").GetString(), category, StringComparison.OrdinalIgnoreCase))
            .Select(book => $"""
                Title: {book.GetProperty("title").GetString()}
                Author: {book.GetProperty("author").GetString()}
                Price: ${book.GetProperty("price").GetDecimal()}
                Description: {book.GetProperty("description").GetString()}
                """);

        var enumerable = matchedBooks.ToList();
        if (!enumerable.Any())
        {
            return Task.FromResult($"No books found in the category '{category}'.");
        }

        return Task.FromResult(string.Join("\n--\n", enumerable));
    }
}
