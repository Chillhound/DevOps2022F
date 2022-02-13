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
        context.Database.EnsureCreated();

        var hmac = new HMACSHA512(Encoding.ASCII.GetBytes("secretkey"));

        var m1 = new Message
        {
            Text = "Nice text"
        
        };
        var m2 = new Message{Text = "Nice text"};
        var m3 = new Message{Text = "Nice text"};
        var m4 = new Message{Text = "Nice text"};
        var m5 = new Message{Text = "Nice text"};
        var m6 = new Message{Text = "Nice text"};
        var m7 = new Message{Text = "Nice text"};
        var m8 = new Message{Text = "Nice text"};
        var m9 = new Message{Text = "Nice text"};
        var m10 = new Message{Text = "Nice text"};
        var m11 = new Message{Text = "Nice text"};

        var messages1 = new List<Message>() {m1, m2, m3};
        var messages2 = new List<Message>() {m4, m5, m6};
        var messages3 = new List<Message>() {m7, m8, m9, m10, m11};

        var u1 = new User
        {
            UserName = "user1",
            Email = "user1@test.dk",
            PasswordHash = Encoding.ASCII.GetString(hmac.ComputeHash(Encoding.ASCII.GetBytes("kode1"))),
            Messages = messages1
        };

        var u2 = new User
        {
            UserName = "user2",
            Email = "user2@test.dk",
            PasswordHash = Encoding.ASCII.GetString(hmac.ComputeHash(Encoding.ASCII.GetBytes("kode2"))),
            Messages = messages2
        };

        var u3 = new User
        {
            UserName = "user3",
            Email = "user3@test.dk",
            PasswordHash = Encoding.ASCII.GetString(hmac.ComputeHash(Encoding.ASCII.GetBytes("kode3"))),
            Messages = messages3
        };

        context.Users.AddRange(u1, u2, u3);
        context.SaveChanges();

        _controller = new MessageController(context);
    }

    [Fact]
    public void PublicTimeline_given_limit_returns_no_more_elements()
    {
        var tweets = _controller.PublicTimeline(10).Value;
        Assert.Equal(10, tweets.Count);
    }

    [Fact]
    public void Post_given_somethingthatjustworkstm_justworks()
    {
        //work in progress
    }
}