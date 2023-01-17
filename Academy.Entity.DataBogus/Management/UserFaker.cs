using Academy.Entity.Management;
using Bogus;
using MongoDB.Bson;

namespace Academy.Entity.DataBogus.Management;

public class UserFaker  : IGenerateFake <User>
{
    public Faker<User> GenerateData()
    {
        return null;
        // var schoolIds = new[]
        // {
        //    ObjectId.GenerateNewId(), ObjectId.GenerateNewId(), ObjectId.GenerateNewId()
        // };
        // #region Faker
        //
        // var bogusData = new Faker<User>()
        //         .RuleFor(u => u.GivenName, f => f.Person.FullName)
        //         .RuleFor(u => u.SchoolIds, f => new[] { f.PickRandom(schoolIds), f.PickRandom(schoolIds) })
        //         .RuleFor(u => u.Email, f => f.Person.Email)
        //
        //         .RuleFor(u => u.Passcode, f => f.Random.Replace("*#?***-#??#?"))
        //         .RuleFor(u => u.Mobile, f => f.Random.Replace("+91-#####-#####"))
        //         .RuleFor(u => u.IsSuperAdmin, f => f.Random.Bool())
        //         .RuleFor(u => u.FirstName, f => f.Person.FirstName)
        //
        //         .RuleFor(u => u.LastName, f => f.Person.LastName)
        //         .RuleFor(u => u.Role, f => new UserRoleFaker().GenerateData().Generate(1).FirstOrDefault())
        //         .RuleFor(u => u.AccountId, f => new MongoDB.Bson.ObjectId())
        //         .RuleFor(u => u.EnteredBy, f => new MongoDB.Bson.ObjectId())
        //         .RuleFor(u => u.IsActive, f => f.Random.Bool())
        //         .RuleFor(u => u.IsDeleted, f => f.Random.Bool())
        //
        //         .RuleFor(u => u.Created, f => f.Date.Between(new DateTime(1900, 01, 01), DateTime.Now))
        //         .RuleFor(u => u.LastModified, f => DateTime.Now)
        //     ;
        // #endregion
        //
        // return bogusData;
    }
}