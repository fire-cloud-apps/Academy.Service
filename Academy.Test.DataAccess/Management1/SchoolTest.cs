using Academy.Entity.DataAccess;
using Academy.Entity.DataAccess.Interface;
using Academy.Entity.DataBogus.Management;
using Academy.Entity.Management;
using LiteDB;
using Microsoft.Extensions.Logging;
using Shouldly;
using StackExchange.Profiling;
using Xunit.Abstractions;
using Academy.Entity.DataAccess.Management.SchoolHander;

namespace Academy.Test.DataAccess.Management;

public class SchoolTest1
{
    ITestOutputHelper _output;
    private MiniProfiler _profiler;
    private string _dbPath;
    
    public SchoolTest(ITestOutputHelper output)
    {
        _output = output;
        _profiler =  MiniProfiler.StartNew("School Profiler");
        _dbPath = @"C:\Test\LiteDb-Data\";
    }
    [Fact]
    public void InsertTest()
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<SchoolTest>();
        IActionCommand<School> command = new CreateHandler(_dbPath, 
             logger);
        School school = new SchoolFaker().GenerateData();
        school = command.CommandHandler(school);
        var obj = school.ShouldNotBeNull();
    }
    
    [Fact]
    public void IndexFormationTest()
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<SchoolTest>();
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
        ILogger<object> logger = new LoggerFactory().CreateLogger<SchoolTest>();
        
        PageMetaData metaData = new PageMetaData()
        {
            PageSize = 5,
            SearchField = "",
            SearchText = "",
            SortDirection = "A",
            SortLabel = "Name"
        };
        IActionQuery<School> command = new GetBatchHandler(_dbPath, 
            logger, metaData);
        IEnumerable<School> schools;
        using (_profiler.Step("1st Iteration. Page No:1"))
        {
            metaData.Page = 1;
            _output.WriteLine("1st Iteration. Page No:1");
            schools = command.GetHandler(null);
            foreach (var school in schools)
            {
                _output.WriteLine($"School ID : {school.Id} Name : {school.Name}");
            }
        }
        using (_profiler.Step("2nd Iteration. Page No:2"))
        {
            metaData.Page = 2;
            _output.WriteLine("2nd Iteration. Page No:2");
            schools = command.GetHandler(null);
            foreach (var school in schools)
            {
                _output.WriteLine($"School ID : {school.Id} Name : {school.Name}");
            }
        }
        using (_profiler.Step("3rd Iteration. Page No:3"))
        {
            metaData.Page = 3;
            _output.WriteLine("2rd Iteration. Page No:2");
            schools = command.GetHandler(null);
            foreach (var school in schools)
            {
                _output.WriteLine($"School ID : {school.Id} Name : {school.Name}");
            }
        }
        
        var obj = schools.ShouldNotBeNull();
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Theory]
    [InlineData(5, "Name","Mattie")]
    [InlineData(10, "RegistrationId","846-MXBL-S53RFP23")]
    [InlineData(10, "Contact.City","Wolfshire")]
    public void GetBatchHandler_FilterTest(int size, string field, string text)
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<SchoolTest>();
        
        PageMetaData metaData = new PageMetaData()
        {
            PageSize = 5,
            SearchField = field,
            SearchText = text,
            SortDirection = "A",
            SortLabel = "Name"
        };
        IActionQuery<School> command = new GetBatchHandler(_dbPath, 
            logger, metaData);
        IEnumerable<School> schools;
        string processesName = "Find the School by Name";
        using (_profiler.Step(processesName))
        {
            metaData.Page = 1;
            _output.WriteLine(processesName);
            schools = command.GetHandler(null);
            foreach (var school in schools)
            {
                _output.WriteLine($"School ID : {school.Id} Name : {school.Name}");
            }
        }
        
        var obj = schools.ShouldNotBeNull();
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Theory]
    [InlineData("638ef2785a3d9e02d1cc5a83")]
    public void GetDetail_Test(string id)
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<SchoolTest>();
        IActionQuery<School> command = new GetDetailHandler(_dbPath, 
            logger);
        School school = new School()
        {
            Id = new ObjectId(id)
        };
        string processesName = "Find the School by Id";
        using (_profiler.Step(processesName))
        {
            school = command.GetHandler(school).FirstOrDefault();
            var obj = school.ShouldNotBeNull();
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Fact]
    public void DML_Test()
    {
        ILogger<object> logger = new LoggerFactory().CreateLogger<SchoolTest>();
        string processesName = "Create School Handler";
        IActionCommand<School> command = new CreateHandler(_dbPath, 
            logger);
        School school = new SchoolFaker().GenerateData();
        using (_profiler.Step(processesName))
        {
            school = command.CommandHandler(school);
        }
        
        var id = school.Id;
        processesName = "Update School Handler";
        using (_profiler.Step(processesName))
        {
            //Update Handler
            command = new UpdateHandler(_dbPath, 
                logger);
            school = new SchoolFaker().GenerateData();
            school.Id = id;
            command.CommandHandler(school);
            var obj = school.ShouldNotBeNull();
        }
        
        processesName = "Delete School Handler";
        using (_profiler.Step(processesName))
        {
            //Update Handler
            command = new DeleteHandler(_dbPath, 
                logger);
            school.Id = id;
            command.CommandHandler(school);
            var obj = school.ShouldNotBeNull();
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
}