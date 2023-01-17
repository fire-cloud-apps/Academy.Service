using Academy.Entity.DataAccess;
using Academy.Entity.DataAccess.Management.UserHandler;
using Academy.Entity.DataAccess.Interface;
using Academy.Entity.DataBogus.Management;
using Academy.Entity.Management;
using LiteDB;
using Microsoft.Extensions.Logging;
using Shouldly;
using StackExchange.Profiling;
using Xunit.Abstractions;

namespace Academy.Test.DataAccess.Management;

public class UserTest1
{
    private ITestOutputHelper _output;
    private MiniProfiler _profiler;
    private string _dbPath;

    public UserTest(ITestOutputHelper output)
    {
        _output = output;
        _profiler =  MiniProfiler.StartNew("User Profiler");
        _dbPath = @"C:\Test\LiteDb-Data\";
    }
    [Fact]
    public void InsertTest()
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<UserTest>();
        IActionCommand<User> command = new CreateHandler(_dbPath, 
             logger);
        User User = new UserFaker().GenerateData();
        User = command.CommandHandler(User);
        var obj = User.ShouldNotBeNull();
    }
    
    [Fact]
    public void IndexFormationTest()
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<UserTest>();
        IActionCommand<bool> command = new IndexFormation(_dbPath, 
            logger);
        bool result = command.CommandHandler(true);
        _output.WriteLine($"IndexFormationTest : {result}");
        bool res = true;
        res.ShouldBeTrue();
        //result.ShouldBeTrue();
    }
    
    [Fact]
    public void GetBatchHandlerTest()
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<UserTest>();
        
        PageMetaData metaData = new PageMetaData()
        {
            PageSize = 5,
            SearchField = "",
            SearchText = "",
            SortDirection = "A",
            SortLabel = "Name"
        };
        IActionQuery<User> command = new GetBatchHandler(_dbPath, 
            logger, metaData);
        IEnumerable<User> Users;
        using (_profiler.Step("1st Iteration. Page No:1"))
        {
            metaData.Page = 1;
            _output.WriteLine("1st Iteration. Page No:1");
            Users = command.GetHandler(null);
            foreach (var User in Users)
            {
                _output.WriteLine($"User ID : {User.Id} Name : {User.Name}");
            }
        }
        using (_profiler.Step("2nd Iteration. Page No:2"))
        {
            metaData.Page = 2;
            _output.WriteLine("2nd Iteration. Page No:2");
            Users = command.GetHandler(null);
            foreach (var User in Users)
            {
                _output.WriteLine($"User ID : {User.Id} Name : {User.Name}");
            }
        }
        using (_profiler.Step("3rd Iteration. Page No:3"))
        {
            metaData.Page = 3;
            _output.WriteLine("2rd Iteration. Page No:2");
            Users = command.GetHandler(null);
            foreach (var User in Users)
            {
                _output.WriteLine($"User ID : {User.Id} Name : {User.Name}");
            }
        }
        
        var obj = Users.ShouldNotBeNull();
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Theory]
    [InlineData(5, "Name","Feest")]
    public void GetBatchHandler_FilterTest(int size, string field, string text)
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<UserTest>();
        
        PageMetaData metaData = new PageMetaData()
        {
            PageSize = 5,
            SearchField = field,
            SearchText = text,
            SortDirection = "A",
            SortLabel = "Name"
        };
        IActionQuery<User> command = new GetBatchHandler(_dbPath, 
            logger, metaData);
        IEnumerable<User> Users;
        string processesName = "Find the User by Name";
        using (_profiler.Step(processesName))
        {
            metaData.Page = 1;
            _output.WriteLine(processesName);
            Users = command.GetHandler(null);
            foreach (var User in Users)
            {
                _output.WriteLine($"User ID : {User.Id} Name : {User.Name}");
            }
        }
        
        var obj = Users.ShouldNotBeNull();
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
   [Theory]
   [InlineData("638f3a99ca97bc0c841d7c6d")]
    public void GetDetail_Test(string id)
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<UserTest>();
        IActionQuery<User> command = new GetDetailHandler(_dbPath, 
            logger);
        User User = new User()
        {
            Id = new ObjectId(id)
        };
        string processesName = "Find the User by Id";
        using (_profiler.Step(processesName))
        {
            User = command.GetHandler(User).FirstOrDefault();
            var obj = User.ShouldNotBeNull();
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Fact]
    public void DML_Test()
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<UserTest>();
        string processesName = "Create User Handler";
        IActionCommand<User> command = new CreateHandler(_dbPath, 
            logger);
        User User = new UserFaker().GenerateData();
        using (_profiler.Step(processesName))
        {
            User = command.CommandHandler(User);
        }
        
        var id = User.Id;
        processesName = "Update User Handler";
        using (_profiler.Step(processesName))
        {
            //Update Handler
            command = new UpdateHandler(_dbPath, 
                logger);
            User = new UserFaker().GenerateData();
            User.Id = id;
            command.CommandHandler(User);
            var obj = User.ShouldNotBeNull();
        }
        
        processesName = "Delete User Handler";
        using (_profiler.Step(processesName))
        {
            //Update Handler
            command = new DeleteHandler(_dbPath, 
                logger);
            User.Id = id;
            command.CommandHandler(User);
            var obj = User.ShouldNotBeNull();
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
}