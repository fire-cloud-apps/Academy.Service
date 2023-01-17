using Academy.DataAccess;
using Academy.DataAccess.GenericHandler;
using Academy.DataAccess.Interface;
using Academy.DataAccess.Management.UserHandler;
using Academy.Entity.DataBogus.Management;
using Academy.Entity.DataBogus.Settings;
using Academy.Entity.Management;
using Academy.Test.DataAccess.Settings;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Shouldly;
using StackExchange.Profiling;
using Xunit.Abstractions;

namespace Academy.Test.DataAccess.Management;


public class UserTest
{
    private ITestOutputHelper _output;
    private MiniProfiler _profiler;
    private string _dbPath;
    private ILogger<object> _logger;

    public UserTest(ITestOutputHelper output)
    {
        _output = output;
        _profiler = MiniProfiler.StartNew("Academy Profiler");
        _dbPath = @"mongodb://localhost:27017";
        _logger  = new LoggerFactory().CreateLogger<AcademyMongoTest>();
    }

    #region Business UserCase

    [Theory]
    [InlineData("Eddie33@hotmail.com","66PDP6-7BI0B", true, false)]
    [InlineData("Lance35@yahoo.com","14PJW0-7JS8J", true, false)]
    public async Task UserLogin_Test(string email,string passcode, bool isActive, bool isDeleted)
    {
        string processesName = "User Login Test.";
        using (_profiler.Step(processesName))
        {
            IActionQuery<User> getCommand = new LoginHandler(_dbPath, _logger);
            
            var login = await getCommand.GetHandlerAsync(new User()
            {
                Email = email,
                Passcode = passcode,
                IsActive = isActive,
                IsDeleted = isDeleted
            });

            getCommand.IsError.ShouldBeFalse();
            if (getCommand.IsError)
            {
                _output.WriteLine($"Error Info : {getCommand.ErrorMessage}");
            }
            else
            {
                var users = login.ToList();
                _output.WriteLine($"Login User Data : {users[0].Email}  {users[0].Passcode} ");
                _output.WriteLine($"Result Data : {users.Count}");
            }
        }
        _output.WriteLine(Environment.NewLine);
        _output.WriteLine(_profiler.RenderPlainText());
    }

    #endregion
    
    [Fact]
    public async Task Insert_Test()
    {
        string processesName = "Insert Academy";
        using (_profiler.Step(processesName))
        {
            IActionCommand<User> command = new CreateHandler<User>
            (_dbPath,
                _logger);
            User model = new UserFaker().GenerateData();
            model = await command.CommandHandlerAsync(model);
            if (command.IsError)
            {
                _output.WriteLine($"Error Info : {command.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {model.GivenName}");
            }
            var obj = model.ShouldNotBeNull();
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Fact]
    public async Task DML_Test()
    {
        #region Insert

        string processesName = "DML Insert Test - Academy";
        User model;
        using (_profiler.Step(processesName))
        {
            IActionCommand<User> command = new CreateHandler<User>
                (_dbPath, _logger);
            model = new UserFaker().GenerateData().Generate();
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
        processesName = "DML Update Test - Academy";
        using (_profiler.Step(processesName))
        {
            IActionCommand<User> command = new UpdateHandler<User>
                (_dbPath, _logger);
            var upModel = new UserFaker().GenerateData().Generate();
            //Update filter
            var builder = Builders<User>.Filter;
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
        processesName = "DML Soft Delete Test - Academy";
        using (_profiler.Step(processesName))
        {
            IActionCommand<User> command = new DeleteHandler<User>
                (_dbPath, _logger, true);
            
            //Soft Delete update 'IsDeleted = true'
            var builderUpdate = Builders<User>.Update;
            var updateFields = builderUpdate.Set(u => u.IsDeleted, true);
            
            //Soft Delete filter
            var builder = Builders<User>.Filter;
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
        processesName = "DML Hard Delete Test - Academy";
        using (_profiler.Step(processesName))
        {
            IActionCommand<User> command = new DeleteHandler<User>
                (_dbPath, _logger, false);

            //Hard Delete filter
            var builder = Builders<User>.Filter;
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
    [InlineData("639331642c19714409b963e5")]
    public async Task GetById_Test(string id)
    {
        string processesName = "Find the Academy by Id";
        using (_profiler.Step(processesName))
        {
            IActionQuery<User> getDocument = new GetHandler<User>
                (_dbPath, _logger);

            #region Filter by ID
            var builder = Builders<User>.Filter;
            // Filter by field
            var idFilter = builder.Eq(u => u.Id, MongoDB.Bson.ObjectId.Parse(id));
            #endregion

            #region  Sort
            var sort = Builders<User>.Sort.Ascending(a => a.Id);
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
            IActionQuery<User> getCommand = new GetHandler<User>
                (_dbPath, _logger);
            #region Filter
            var builder = Builders<User>.Filter;
            // Filter by field with list of desired values
            var nameListFilter = builder.In(u => u.IsActive, new[] { true, false });
            #endregion

            var academy = await getCommand.GetHandlerAsync(nameListFilter, null, null);

            if (getCommand.IsError)
            {
                _output.WriteLine($"Error Info : {getCommand.ErrorMessage}");
            }
            else
            {
                _output.WriteLine($"Result Data : {academy.ToList().Count}");
            }
            
            getCommand.IsError.ShouldBeFalse();
        }
        _output.WriteLine(_profiler.RenderPlainText());
    }
    
    [Fact]
    public async Task RangeBetween_Filter_Test()
    {
        string processesName = "Range Between then the given value'.";
        using (_profiler.Step(processesName))
        {
            IActionQuery<User> getCommand = new GetHandler<User>
                (_dbPath, _logger);
            #region Range Filter
            FilterDefinition<User> rangeFilter;
            var builder = Builders<User>.Filter;
            var fromDate = new DateTime(1980, 01, 01);
            var toDate = new DateTime(2012,12, 31);
            rangeFilter = builder.Gt("Created", fromDate) &
                              builder.Lt("Created", toDate);
            #endregion

            var academy = await getCommand.GetHandlerAsync(rangeFilter, null, null);
            
            getCommand.IsError.ShouldBe(false);
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
    public async Task Regex_Filter_Test()
    {
        string processesName = "Regex Filter.";
        using (_profiler.Step(processesName))
        {
            IActionQuery<User> getCommand = new GetHandler<User>
                (_dbPath, _logger);
            #region Regex Filter
            var builder = Builders<User>.Filter;
            var pattern = new BsonRegularExpression("kozey", "i"); //"i" Indicates case insensitive.
            // Filter the student who's name contains "Casper".
            var regexFilter = builder.Regex(u => u.FirstName, pattern);
            
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
            IActionQuery<User> getCommand = new GetHandler<User>
                (_dbPath, _logger);
            #region No Filter
            var builder = Builders<User>.Filter;
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
            IActionQuery<User> getCommand = new GetHandler<User>
                (_dbPath, _logger);
            #region No Filter
            var builder = Builders<User>.Filter;
            var noFilter = builder.Empty;
            #endregion
            
            //Sorting
            var sort = Builders<User>.Sort
                .Ascending(u => u.Name)
                .Descending(u => u.Created);

            var academy = await getCommand.GetHandlerAsync(noFilter, sort, 
                new Pagination()
            {
                Page = page, PageSize = size
            });
            _output.WriteLine($"Result Data : {academy.ToList().Count}");
            foreach (var ac in academy)
            {
                _output.WriteLine($"Academy Data : {ac.Name}");
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