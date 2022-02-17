using System.Net;
using System.Security.Cryptography;
using System.Text;

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

        var hmac = new HMACSHA512(Encoding.ASCII.GetBytes("secretkey"));

        var hashed =  Encoding.ASCII.GetString(hmac.ComputeHash(Encoding.ASCII.GetBytes("SikkerKode")));

        var user = new User
        {
            UserName = "Testman",
            Email = "test@test.dk",
            PasswordHash = hashed
        };

        context.Users.Add(user);
        context.SaveChanges();
        _controller = new AccountController(context);
    }

    [Fact]
    public void Register_given_valid_user_returns_ok_response()
    {
        var newUser = new CreateUserDTO
        {
            username = "mikkis_far",
            email = "test@test.dk",
            pwd = "benis"
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
        Assert.Equal("Email and password needs to be provided", badResult.Value);
    }

    [Fact]
    public void Login_given_user_with_full_credentials_returns_okresult()
    {
        var user = new LoginDTO
        {
            Email = "test@test.dk",
            Password = "SikkerKode"
        };

        var result = _controller.Login(user);

        Assert.NotNull(result); //anything better to assert?
    }

    [Fact]
    public void Login_given_user_with_wrong_password_returns_badrequest()
    {
        var user = new LoginDTO
        {
            Email = "test@test.dk",
            Password = "UsikkerKode"
        };

        var result = _controller.Login(user).Result;
        var badResult = result as BadRequestObjectResult;

        Assert.Equal(400, badResult.StatusCode);
        Assert.Equal("User not Found", badResult.Value);
    }

}

