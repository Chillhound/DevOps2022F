namespace ControllerTests;

public class UserControllerTests
{
    private readonly UserController _controller;

    public UserControllerTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<MiniTwitContext>();
        builder.UseSqlite(connection);
        var context = new MiniTwitContext(builder.Options);
        context.Database.EnsureCreated();
        _controller = new UserController(context);
    }

    [Fact]
    public void IsTrue()
    {
        Assert.True(true);
    }
}



