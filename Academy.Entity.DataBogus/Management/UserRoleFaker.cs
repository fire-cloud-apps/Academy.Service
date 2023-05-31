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
            .RuleFor(u => u.AllowedModules, new ModuleFaker().GenerateData().Generate(5));
        #endregion

        return bogusData;
    }
}

public class ModuleFaker : IGenerateFake <ModulePermission>
{
    public Faker<ModulePermission> GenerateData()
    {
        #region Faker
        var bogusData = new Faker<ModulePermission>()
            .RuleFor(u => u.Name, f => f.Database.Column())
            .RuleFor(u => u.Description, f => f.Lorem.Lines(2))
            .RuleFor(u => u.IconKey, "Config")
            .RuleFor(u => u.Read, f => f.Random.Bool())
            .RuleFor(u => u.Delete, f => f.Random.Bool())
            .RuleFor(u => u.Modify, f => f.Random.Bool());
        #endregion

        return bogusData;
    }
    
}