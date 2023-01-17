using Academy.Entity.Management;
using Bogus;

namespace Academy.Entity.DataBogus.Management;

public class UserRoleFaker : IGenerateFake <UserRole>
{
    public Faker<UserRole> GenerateData()
    {
        #region Faker
        var bogusData = new Faker<UserRole>()
            .RuleFor(u => u.Name, f => f.Person.FullName)
            
            .RuleFor(u => u.Description, f => f.Person.UserName)
            .RuleFor(u => u.AllowedModules, new PermissionFaker().GenerateData().Generate(5));
        #endregion

        return bogusData;
    }
}

public class PermissionFaker : IGenerateFake <Permissions>
{
    public Faker<Permissions> GenerateData()
    {
        #region Faker
        var bogusData = new Faker<Permissions>()
            .RuleFor(u => u.AllowedModule,  new  ModulesFaker().GenerateData().Generate(1).FirstOrDefault())
            
            .RuleFor(u => u.CanRead, f => f.Random.Bool())
            .RuleFor(u => u.CanWrite, f => f.Random.Bool());
        #endregion

        return bogusData;
    }
    
}