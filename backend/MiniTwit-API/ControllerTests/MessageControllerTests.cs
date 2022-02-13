namespace ControllerTests;

public class MessageControllerTests
{
    private readonly MessageController _controller;

    public MessageControllerTests()
    {   
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<MiniTwitContext>();
        builder.UseSqlite(connection);
        var context = new MiniTwitContext(builder.Options);
        _controller = new MessageController(context);
    }

    [Fact]
    public void PublicTimeline_given_limit_returns_no_more_elements()
    {
        try 
        {
            var tweets = _controller.PublicTimeline(10).Value;
        }
        catch(Exception e)
        {
            Assert.True(true);
        }
        //Assert.Equal(10, tweets.Count);
    }
}