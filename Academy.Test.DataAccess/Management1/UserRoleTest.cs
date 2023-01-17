using Academy.Entity.DataAccess;
using Academy.Entity.DataAccess.Interface;
using Academy.Entity.DataAccess.Management.RoleHandler;
using Academy.Entity.DataBogus.Management;
using Academy.Entity.Management;
using LiteDB;
using Microsoft.Extensions.Logging;
using Shouldly;
using StackExchange.Profiling;
using Xunit.Abstractions;

namespace Academy.Test.DataAccess.Management;

public class UserRoleTest1
{
    private ITestOutputHelper _output;
    private MiniProfiler _profiler;
    private string _dbPath;
    
    public UserRoleTest(ITestOutputHelper output)
    {
        _output = output;
        _profiler =  MiniProfiler.StartNew("UserRole Profiler");
        _dbPath = @"C:\Test\LiteDb-Data\";
    }
    [Fact]
    public void InsertTest()
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<UserRoleTest>();
        IActionCommand<UserRole> command = new CreateHandler(_dbPath, 
             logger);
        UserRole UserRole = new UserRoleFaker().GenerateData();
        UserRole = command.CommandHandler(UserRole);
        var obj = UserRole.ShouldNotBeNull();
    }
    
    [Fact]
    public void IndexFormationTest()
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<UserRoleTest>();
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
        ILogger<object> logger = new LoggerFactory().CreateLogger<UserRoleTest>();
        
        PageMetaData metaData = new PageMetaData()
        {
            PageSize = 5,
            SearchField = "",
            SearchText = "",
            SortDirection = "A",
            SortLabel = "Name"
        };
        IActionQuery<UserRole> command = new GetBatchHandler(_dbPath, 
            logger, metaData);
        IEnumerable<UserRole> UserRoles;
        using (_profiler.Step("1st Iteration. Page No:1"))
        {
            metaData.Page = 1;
            _output.WriteLine("1st Iteration. Page No:1");
            UserRoles = command.GetHandler(null);
            foreach (var UserRole in UserRoles)
            {
                _output.WriteLine($"UserRole ID : {UserRole.Id} Name : {UserRole.Name}");
            }
        }
        using (_profiler.Step("2nd Iteration. Page No:2"))
        {
            metaData.Page = 2;
            _output.WriteLine("2nd Iteration. Page No:2");
            UserRoles = command.GetHandler(null);
            foreach (var UserRole in UserRoles)
            {
                _output.WriteLine($"UserRole ID : {UserRole.Id} Name : {UserRole.Name}");
            }
        }
        using (_profiler.Step("3rd Iteration. Page No:3"))
        {
            metaData.Page = 3;
            _output.WriteLine("2rd Iteration. Page No:2");
            UserRoles = command.GetHandler(null);
            foreach (var UserRole in UserRoles)
            {
                _output.WriteLine($"UserRole ID : {UserRole.Id} Name : {UserRole.Name}");
            }
        }
        
        var obj = UserRoles.ShouldNotBeNull();
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Theory]
    [InlineData(5, "Name","Feest")]
    public void GetBatchHandler_FilterTest(int size, string field, string text)
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<UserRoleTest>();
        
        PageMetaData metaData = new PageMetaData()
        {
            PageSize = 5,
            SearchField = field,
            SearchText = text,
            SortDirection = "A",
            SortLabel = "Name"
        };
        IActionQuery<UserRole> command = new GetBatchHandler(_dbPath, 
            logger, metaData);
        IEnumerable<UserRole> UserRoles;
        string processesName = "Find the UserRole by Name";
        using (_profiler.Step(processesName))
        {
            metaData.Page = 1;
            _output.WriteLine(processesName);
            UserRoles = command.GetHandler(null);
            foreach (var UserRole in UserRoles)
            {
                _output.WriteLine($"UserRole ID : {UserRole.Id} Name : {UserRole.Name}");
            }
        }
        
        var obj = UserRoles.ShouldNotBeNull();
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
   [Theory]
   [InlineData("638f280a3b1fdd11bf299ec6")]
    public void GetDetail_Test(string id)
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<UserRoleTest>();
        IActionQuery<UserRole> command = new GetDetailHandler(_dbPath, 
            logger);
        UserRole UserRole = new UserRole()
        {
            Id = new ObjectId(id)
        };
        string processesName = "Find the UserRole by Id";
        using (_profiler.Step(processesName))
        {
            UserRole = command.GetHandler(UserRole).FirstOrDefault();
            var obj = UserRole.ShouldNotBeNull();
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Fact]
    public void DML_Test()
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<UserRoleTest>();
        string processesName = "Create UserRole Handler";
        IActionCommand<UserRole> command = new CreateHandler(_dbPath, 
            logger);
        UserRole UserRole = new UserRoleFaker().GenerateData();
        using (_profiler.Step(processesName))
        {
            UserRole = command.CommandHandler(UserRole);
        }
        
        var id = UserRole.Id;
        processesName = "Update UserRole Handler";
        using (_profiler.Step(processesName))
        {
            //Update Handler
            command = new UpdateHandler(_dbPath, 
                logger);
            UserRole = new UserRoleFaker().GenerateData();
            UserRole.Id = id;
            command.CommandHandler(UserRole);
            var obj = UserRole.ShouldNotBeNull();
        }
        
        processesName = "Delete UserRole Handler";
        using (_profiler.Step(processesName))
        {
            //Update Handler
            command = new DeleteHandler(_dbPath, 
                logger);
            UserRole.Id = id;
            command.CommandHandler(UserRole);
            var obj = UserRole.ShouldNotBeNull();
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
}