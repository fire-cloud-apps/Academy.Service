using Bogus;

namespace Academy.Entity.DataBogus.Settings;

public class AcademyFaker : IGenerateFake <Entity.Settings.Academy>
{
    public Faker<Entity.Settings.Academy> GenerateData()
    {
        #region Faker
        var bogusData = new Faker<Entity.Settings.Academy>()
                .RuleFor(u => u.Name, f => f.Random.Replace("Academy Year - 202#"))
                .RuleFor(u => u.Description, f => f.Company.CompanyName())
                .RuleFor(u => u.StartsOn, f => f.Date.Between(new DateTime(2010, 01, 01), DateTime.Now))
                .RuleFor(u => u.EndsOn, f => f.Date.Between(new DateTime(2010, 01, 01), DateTime.Now))
                
                .RuleFor(u => u.AccountId, f => new MongoDB.Bson.ObjectId())
                .RuleFor(u => u.EnteredBy, f => new MongoDB.Bson.ObjectId())
                .RuleFor(u => u.IsActive, f=> f.Random.Bool())
                .RuleFor(u => u.IsDeleted, f=> f.Random.Bool())
            
                .RuleFor(u => u.Created, f => f.Date.Between(new DateTime(1900, 01, 01), DateTime.Now))
                .RuleFor(u => u.LastModified, f=> DateTime.Now)
            ;
        #endregion

        return bogusData;
    }
}