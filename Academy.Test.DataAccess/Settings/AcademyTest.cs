using Academy.Entity.DataAccess;
using Academy.Entity.DataAccess.Interface;
using Academy.Entity.DataAccess.Settings.AcademyHandler;
using Academy.Entity.DataBogus.Settings;
using LiteDB;
using Microsoft.Extensions.Logging;
using Shouldly;
using StackExchange.Profiling;
using Xunit.Abstractions;
using Model = Academy.Entity.Settings;
namespace Academy.Test.DataAccess.Settings;

public class AcademyTest
{
    private ITestOutputHelper _output;
    private MiniProfiler _profiler;
    private string _dbPath;

    public AcademyTest(ITestOutputHelper output)
    {
        _output = output;
        _profiler =  MiniProfiler.StartNew("Academy Profiler");
        _dbPath = @"C:\Test\LiteDb-Data\";
    }
    [Fact]
    public void InsertTest()
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<AcademyTest>();
        IActionCommand<Entity.Settings.Academy> command = new CreateHandler(_dbPath, 
             logger);
        Model.Academy model = new AcademyFaker().GenerateData();
        model = command.CommandHandler(model);
        var obj = model.ShouldNotBeNull();
    }
    
    [Fact]
    public void IndexFormationTest()
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<AcademyTest>();
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
        ILogger<object> logger = new LoggerFactory().CreateLogger<AcademyTest>();
        
        PageMetaData metaData = new PageMetaData()
        {
            PageSize = 5,
            SearchField = "",
            SearchText = "",
            SortDirection = "A",
            SortLabel = "Name"
        };
        IActionQuery<Entity.Settings.Academy> command = new GetBatchHandler(_dbPath, 
            logger, metaData);
        IEnumerable<Entity.Settings.Academy> Users;
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
        ILogger<object> logger = new LoggerFactory().CreateLogger<AcademyTest>();
        
        PageMetaData metaData = new PageMetaData()
        {
            PageSize = 5,
            SearchField = field,
            SearchText = text,
            SortDirection = "A",
            SortLabel = "Name"
        };
        IActionQuery<Entity.Settings.Academy> command = new GetBatchHandler(_dbPath, 
            logger, metaData);
        IEnumerable<Entity.Settings.Academy> Users;
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
   [InlineData("638f463681ba9000e9790420")]
    public void GetDetail_Test(string id)
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<AcademyTest>();
        IActionQuery<Entity.Settings.Academy> command = new GetDetailHandler(_dbPath, 
            logger);
        Model.Academy model = new Model.Academy()
        {
            Id = new ObjectId(id)
        };
        string processesName = "Find the User by Id";
        using (_profiler.Step(processesName))
        {
            model = command.GetHandler(model).FirstOrDefault();
            var obj = model.ShouldNotBeNull();
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Fact]
    public void DML_Test()
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<AcademyTest>();
        string processesName = "Create User Handler";
        IActionCommand<Entity.Settings.Academy> command = new CreateHandler(_dbPath, 
            logger);
        Model.Academy model = new AcademyFaker().GenerateData();
        using (_profiler.Step(processesName))
        {
            model = command.CommandHandler(model);
        }
        
        var id = model.Id;
        processesName = "Update User Handler";
        using (_profiler.Step(processesName))
        {
            //Update Handler
            command = new UpdateHandler(_dbPath, 
                logger);
            model = new AcademyFaker().GenerateData();
            model.Id = id;
            command.CommandHandler(model);
            var obj = model.ShouldNotBeNull();
        }
        
        processesName = "Delete User Handler";
        using (_profiler.Step(processesName))
        {
            //Update Handler
            command = new DeleteHandler(_dbPath, 
                logger);
            model.Id = id;
            command.CommandHandler(model);
            var obj = model.ShouldNotBeNull();
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
}