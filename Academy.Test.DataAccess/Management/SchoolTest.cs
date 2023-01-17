using Academy.DataAccess;
using Academy.DataAccess.GenericHandler;
using Academy.DataAccess.Interface;
using Academy.Entity.DataBogus.Management;
using Academy.Entity.Management;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Shouldly;
using StackExchange.Profiling;
using Xunit.Abstractions;

namespace Academy.Test.DataAccess.Management;

public class SchoolTest
{
    private ITestOutputHelper _output;
    private MiniProfiler _profiler;
    private string _dbPath;
    private ILogger<object> _logger;
    private School _model;

    public SchoolTest(ITestOutputHelper output)
    {
        _output = output;
        _profiler = MiniProfiler.StartNew("Academy Profiler");
        _dbPath = @"mongodb://localhost:27017";
        _logger  = new LoggerFactory().CreateLogger<SchoolTest>();
    }
    
    [Fact]
    public async Task Insert_Test()
    {
        string processesName = "Insert School";
        using (_profiler.Step(processesName))
        {
            IActionCommand<School> command = new CreateHandler<School>
            (_dbPath,
                _logger);
            School model = new SchoolFaker().GenerateData();
            model = await command.CommandHandlerAsync(model);
            _model = model;
            if (command.IsError)
            {
                _output.WriteLine($"Error Info : {command.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {model.Name}");
            }
            var obj = model.ShouldNotBeNull();
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Fact]
    public async Task DML_Test()
    {
        #region Insert

        string processesName = "DML Insert Test - School";
        School model;
        using (_profiler.Step(processesName))
        {
            IActionCommand<School> command = new CreateHandler<School>
                (_dbPath, _logger);
            model = new SchoolFaker().GenerateData().Generate();
            model = await command.CommandHandlerAsync(model);
            _output.WriteLine($"Inserted Document : {model.Name} IsDeleted: {model.IsDeleted}");
            command.IsError.ShouldBeFalse();
            if (command.IsError)
            {
                _output.WriteLine($"Error Info : {command.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {model.Name}");
            }
        }

        #endregion

        #region Update
        processesName = "DML Update Test - School";
        using (_profiler.Step(processesName))
        {
            IActionCommand<School> command = new UpdateHandler<School>
                (_dbPath, _logger);
            var upModel = new SchoolFaker().GenerateData().Generate();
            //Update filter
            var builder = Builders<School>.Filter;
            var filter = builder.Eq(u => u.Id, model.Id);
            upModel.Id = model.Id; 
            upModel = await command.CommandHandlerAsync(filter, upModel);
            _output.WriteLine($"Updated Document : {upModel.Name} IsDeleted: {upModel.IsDeleted}");
            command.IsError.ShouldBeFalse();
            if (command.IsError)
            {
                _output.WriteLine($"Error Info : {command.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {upModel.Name}");
            }
        }
        #endregion
        
        #region Soft Delete
        processesName = "DML Soft Delete Test - School";
        using (_profiler.Step(processesName))
        {
            IActionCommand<School> command = new DeleteHandler<School>
                (_dbPath, _logger, true);
            
            //Soft Delete update 'IsDeleted = true'
            var builderUpdate = Builders<School>.Update;
            var updateFields = builderUpdate.Set(u => u.IsDeleted, true);
            
            //Soft Delete filter
            var builder = Builders<School>.Filter;
            var filter = builder.Eq(u => u.Id, model.Id);

            var softModel = await command.CommandHandlerAsync(filter, updateFields, model);
            
            _output.WriteLine($"Soft Delete Document : {model.Name} IsDeleted: {model.IsDeleted}");
            command.IsError.ShouldBeFalse();
            if (command.IsError)
            {
                _output.WriteLine($"Error Info : {command.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {model.Name}");
            }
        }
        #endregion
        
        #region Hard Delete
        processesName = "DML Hard Delete Test - School";
        using (_profiler.Step(processesName))
        {
            IActionCommand<School> command = new DeleteHandler<School>
                (_dbPath, _logger, false);

            //Hard Delete filter
            var builder = Builders<School>.Filter;
            var filter = builder.Eq(u => u.Id, model.Id);

            var softModel = await command.CommandHandlerAsync(filter, null, model);
            
            _output.WriteLine($"Hard Delete Document : {softModel.Name} IsDeleted: {softModel.IsDeleted}");
            command.IsError.ShouldBeFalse();
            if (command.IsError)
            {
                _output.WriteLine($"Error Info : {command.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {softModel.Name}");
            }
        }
        #endregion
        
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Theory]
    [InlineData("63944668857e6e82fdbb2607")]
    public async Task GetById_Test(string id)
    {
        string processesName = "Find the School by Id";
        using (_profiler.Step(processesName))
        {
            IActionQuery<School> getDocument = new GetHandler<School>
                (_dbPath, _logger);

            #region Filter by ID
            var builder = Builders<School>.Filter;
            // Filter by field
            var idFilter = builder.Eq(u => u.Id, MongoDB.Bson.ObjectId.Parse(id));
            #endregion

            #region  Sort
            var sort = Builders<School>.Sort.Ascending(a => a.Id);
            #endregion
            
            var model = await getDocument.GetHandlerAsync(idFilter, null, null);
            _output.WriteLine($"Find by Id: {model.FirstOrDefault().Id} Name: {model.FirstOrDefault().Name} ");
            
            getDocument.IsError.ShouldBe(false);
            if (getDocument.IsError)
            {
                _output.WriteLine($"Error Info : {getDocument.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {model.ToList().Count}");
            }
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }

    [Fact]
    public async Task EQIn_Filter_Test()
    {
        string processesName = "Compare with 'IN', a full text compare.";
        using (_profiler.Step(processesName))
        {
            IActionQuery<School> getCommand = new GetHandler<School>
                (_dbPath, _logger);
            #region Filter
            var builder = Builders<School>.Filter;
            // Filter by field with list of desired values
            var nameListFilter = builder.In(u => u.IsActive, new[] { true, false });
            #endregion

            var model = await getCommand.GetHandlerAsync(nameListFilter, null, null);

            if (getCommand.IsError)
            {
                _output.WriteLine($"Error Info : {getCommand.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {model.ToList().Count}");
            }
            
            getCommand.IsError.ShouldBeFalse();
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Fact]
    public async Task GreaterThan_Filter_Test()
    {
        string processesName = "Greater then the given value'.";
        using (_profiler.Step(processesName))
        {
            IActionQuery<School> getCommand = new GetHandler<School>
                (_dbPath, _logger);
            #region Filter
            var builder = Builders<School>.Filter;
            // Filter by field with list of desired values
            var greaterFilter = builder.Gt(s => s.FoundedOn, new DateTime(2015, 1, 1));
            #endregion

            var model = await getCommand.GetHandlerAsync(greaterFilter, null, null);
            getCommand.IsError.ShouldBe(false);
            if (getCommand.IsError)
            {
                _output.WriteLine($"Error Info : {getCommand.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {model.ToList().Count}");
            }
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Fact]
    public async Task RangeBetween_Filter_Test()
    {
        string processesName = "Range Between then the given value'.";
        using (_profiler.Step(processesName))
        {
            IActionQuery<School> getCommand = new GetHandler<School>
                (_dbPath, _logger);
            #region Range Filter
            FilterDefinition<School> rangeFilter;
            var builder = Builders<School>.Filter;
            var fromDate = new DateTime(1980, 01, 01);
            var toDate = new DateTime(2012,12, 31);
            rangeFilter = builder.Gt("StartsOn", fromDate) &
                              builder.Lt("StartsOn", toDate);
            #endregion

            var model = await getCommand.GetHandlerAsync(rangeFilter, null, null);
            
            getCommand.IsError.ShouldBe(false);
            if (getCommand.IsError)
            {
                _output.WriteLine($"Error Info : {getCommand.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {model.ToList().Count}");
            }
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Fact]
    public async Task Regex_Filter_Test()
    {
        string processesName = "Regex Filter.";
        using (_profiler.Step(processesName))
        {
            IActionQuery<School> getCommand = new GetHandler<School>
                (_dbPath, _logger);
            #region Regex Filter
            var builder = Builders<School>.Filter;
            var pattern = new BsonRegularExpression("kozey", "i"); //"i" Indicates case insensitive.
            // Filter the student who's name contains "Casper".
            var regexFilter = builder.Regex(u => u.Name, pattern);
            
            #endregion

            var model = await getCommand.GetHandlerAsync(regexFilter, null, null);
            
            getCommand.IsError.ShouldBeFalse();
            if (getCommand.IsError)
            {
                _output.WriteLine($"Error Info : {getCommand.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {model.ToList().Count}");
            }
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Fact]
    public async Task No_Filter_Test()
    {
        string processesName = "No Filter.";
        using (_profiler.Step(processesName))
        {
            IActionQuery<School> getCommand = new GetHandler<School>
                (_dbPath, _logger);
            #region No Filter
            var builder = Builders<School>.Filter;
            var noFilter = builder.Empty;
            #endregion

            var model = await getCommand.GetHandlerAsync(noFilter, null, null);
            
            getCommand.IsError.ShouldBeFalse();
            if (getCommand.IsError)
            {
                _output.WriteLine($"Error Info : {getCommand.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {model.ToList().Count}");
            }
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 5)]
    [InlineData(3, 5)]
    public async Task Pagination_Test(int page, int size)
    {
        string processesName = "Pagination Test";
        using (_profiler.Step(processesName))
        {
            IActionQuery<School> getCommand = new GetHandler<School>
                (_dbPath, _logger);
            #region No Filter
            var builder = Builders<School>.Filter;
            var noFilter = builder.Empty;
            #endregion
            
            //Sorting
            var sort = Builders<School>.Sort
                .Ascending(u => u.Name)
                .Descending(u => u.Created);

            var model = await getCommand.GetHandlerAsync(noFilter, sort, 
                new Pagination()
            {
                Page = page, PageSize = size
            });
            _output.WriteLine($"Result Data : {model.ToList().Count}");
            foreach (var ac in model)
            {
                _output.WriteLine($"School Data : {ac.Name}");
            }
           
            getCommand.IsError.ShouldBeFalse();
            if (getCommand.IsError)
            {
                _output.WriteLine($"Error Info : {getCommand.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {model.ToList().Count}");
            }
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
}
