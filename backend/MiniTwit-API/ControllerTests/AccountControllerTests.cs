using System.Net;

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
        context.Database.EnsureCreated();
        _controller = new AccountController(context);
    }

    [Fact]
    public void Register_given_valid_user_returns_ok_response()
    {
        var newUser = new CreateUserDTO
        {
            UserName = "mikkis_far",
            Email = "test@test.dk",
            Password = "benis"
        };

        var result = _controller.Post(newUser).Result;
        var okResult = result as OkObjectResult;    
        
        //courtesy of: https://stackoverflow.com/questions/41292919/unit-testing-controller-methods-which-return-iactionresult 
        Assert.NotNull(result);
        Assert.Equal(200, okResult.StatusCode);        
    }

    [Fact]
    public void Register_given_invalid_user_returns_badrequest()
    {
        CreateUserDTO newUser = null;

        var result = _controller.Post(newUser).Result;
        var badResult = result as BadRequestResult;

        Assert.Equal(400, badResult.StatusCode);
    }

    [Fact]
    public void Login_given_user_with_null_email_returns_badrequest()
    {
        var user = new LoginDTO
        {
            Email = null,
            Password = "SikkertKodeord125"
        };

        var result = _controller.Login(user).Result;
        var badResult = result as BadRequestObjectResult; 
        Assert.Equal(400, badResult.StatusCode);
    }

    [Fact]
    public void Login_given_user_with_full_credentials_returns_okresult()
    {
        var user = new LoginDTO
        {
            Email = "test@test.dk",
            Password = "BiteThePillow184"
        };

        var result = _controller.Login(user).Result;
        
        //TODO needs seeding before we can progress
    }

}

