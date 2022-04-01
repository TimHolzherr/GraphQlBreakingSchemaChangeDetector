namespace CodeFirstHotChocolate;

public class Query
{
    public Task<List<Book>> GetBooks() => Task.FromResult(new List<Book>());
}

public record Book(int Id, Author Author, string Description);
public record Author(int Id, string Name);
