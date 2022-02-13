namespace ControllerTests;
public class AccountControllerTests
{
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<MiniTwitContext>();
        builder.UseSqlite(connection);
        var context = new MiniTwitContext(builder.Options);
        _controller = new AccountController(context);
    }
}

