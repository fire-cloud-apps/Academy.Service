using Academy.DataAccess;
using Academy.DataAccess.GenericHandler;
using Academy.DataAccess.Interface;
using Academy.Entity.DataBogus.Management;
using Academy.Entity.Management;
using Academy.Test.DataAccess.Settings;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Shouldly;
using StackExchange.Profiling;
using Xunit.Abstractions;

namespace Academy.Test.DataAccess.Management;


public class ModulesTest
{
    private ITestOutputHelper _output;
    private MiniProfiler _profiler;
    private string _dbPath;
    private ILogger<object> _logger;

    public ModulesTest(ITestOutputHelper output)
    {
        _output = output;
        _profiler = MiniProfiler.StartNew("Module Profiler");
        _dbPath = @"mongodb://localhost:27017";
        _logger  = new LoggerFactory().CreateLogger<ModulesTest>();
    }
    [Fact]
    public async Task Insert_Test()
    {
        string processesName = "Insert Module";
        using (_profiler.Step(processesName))
        {
            IActionCommand<MenuModule> command = new CreateHandler<MenuModule>
            (_dbPath,
                _logger);
            MenuModule model = new ModulesFaker().GenerateData();
            model = await command.CommandHandlerAsync(model);
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

        string processesName = "DML Insert Test - Module";
        MenuModule model;
        using (_profiler.Step(processesName))
        {
            IActionCommand<MenuModule> command = new CreateHandler<MenuModule>
                (_dbPath, _logger);
            model = new ModulesFaker().GenerateData().Generate();
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
        processesName = "DML Update Test - Module";
        using (_profiler.Step(processesName))
        {
            IActionCommand<MenuModule> command = new UpdateHandler<MenuModule>
                (_dbPath, _logger);
            var upModel = new ModulesFaker().GenerateData().Generate();
            //Update filter
            var builder = Builders<MenuModule>.Filter;
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
        processesName = "DML Soft Delete Test - Module";
        using (_profiler.Step(processesName))
        {
            IActionCommand<MenuModule> command = new DeleteHandler<MenuModule>
                (_dbPath, _logger, true);
            
            //Soft Delete update 'IsDeleted = true'
            var builderUpdate = Builders<MenuModule>.Update;
            var updateFields = builderUpdate.Set(u => u.IsDeleted, true);
            
            //Soft Delete filter
            var builder = Builders<MenuModule>.Filter;
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
        processesName = "DML Hard Delete Test - Module";
        using (_profiler.Step(processesName))
        {
            IActionCommand<MenuModule> command = new DeleteHandler<MenuModule>
                (_dbPath, _logger, false);

            //Hard Delete filter
            var builder = Builders<MenuModule>.Filter;
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
    [InlineData("6392c32a092306f54d53bf46")]
    public async Task GetById_Test(string id)
    {
        string processesName = "Find the Module by Id";
        using (_profiler.Step(processesName))
        {
            IActionQuery<MenuModule> getDocument = new GetHandler<MenuModule>
                (_dbPath, _logger);

            #region Filter by ID
            var builder = Builders<MenuModule>.Filter;
            // Filter by field
            var idFilter = builder.Eq(u => u.Id, MongoDB.Bson.ObjectId.Parse(id));
            #endregion

            #region  Sort
            var sort = Builders<MenuModule>.Sort.Ascending(a => a.Id);
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
            IActionQuery<MenuModule> getCommand = new GetHandler<MenuModule>
                (_dbPath, _logger);
            #region Filter
            var builder = Builders<MenuModule>.Filter;
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
    public async Task Regex_Filter_Test()
    {
        string processesName = "Regex Filter.";
        using (_profiler.Step(processesName))
        {
            IActionQuery<MenuModule> getCommand = new GetHandler<MenuModule>
                (_dbPath, _logger);
            #region Regex Filter
            var builder = Builders<MenuModule>.Filter;
            var pattern = new BsonRegularExpression("Notify", "i"); //"i" Indicates case insensitive.
            // Filter the student who's name contains "Casper".
            var regexFilter = builder.Regex(u => u.Name, pattern);
            
            #endregion

            var academy = await getCommand.GetHandlerAsync(regexFilter, null, null);
            
            getCommand.IsError.ShouldBeFalse();
            if (getCommand.IsError)
            {
                _output.WriteLine($"Error Info : {getCommand.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {academy.ToList().Count}");
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
            IActionQuery<MenuModule> getCommand = new GetHandler<MenuModule>
                (_dbPath, _logger);
            #region No Filter
            var builder = Builders<MenuModule>.Filter;
            var noFilter = builder.Empty;
            #endregion

            var academy = await getCommand.GetHandlerAsync(noFilter, null, null);
            
            getCommand.IsError.ShouldBeFalse();
            if (getCommand.IsError)
            {
                _output.WriteLine($"Error Info : {getCommand.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {academy.ToList().Count}");
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
            IActionQuery<MenuModule> getCommand = new GetHandler<MenuModule>
                (_dbPath, _logger);
            #region No Filter
            var builder = Builders<MenuModule>.Filter;
            var noFilter = builder.Empty;
            #endregion
            
            //Sorting
            var sort = Builders<MenuModule>.Sort
                .Ascending(u => u.Name)
                .Descending(u => u.IsActive);

            var academy = await getCommand.GetHandlerAsync(noFilter, sort, 
                new Pagination()
            {
                Page = page, PageSize = size
            });
            _output.WriteLine($"Result Data : {academy.ToList().Count}");
            foreach (var ac in academy)
            {
                _output.WriteLine($"Module Data : {ac.Name}");
            }
           
            getCommand.IsError.ShouldBeFalse();
            if (getCommand.IsError)
            {
                _output.WriteLine($"Error Info : {getCommand.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {academy.ToList().Count}");
            }
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
}