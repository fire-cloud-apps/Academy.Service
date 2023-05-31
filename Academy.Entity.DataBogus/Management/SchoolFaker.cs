using Academy.Entity.DataBogus.Common;
using Academy.Entity.DataBogus.Settings;
using Academy.Entity.Management;
using Bogus;

namespace Academy.Entity.DataBogus.Management;

public class SchoolFaker : IGenerateFake <School>
{
    public Faker<School> GenerateData()
    {
        #region Faker
        var bogusData = new Faker<School>()
                .RuleFor(u => u.Name, f => f.Person.FullName)
                .RuleFor(u => u.RegistrationNo, f => f.Random.Replace("###-????-?##?**##"))
                .RuleFor(u => u.FoundedOn, f => f.Date.Between(new DateTime(1900, 01, 01), DateTime.Now))
            
                //.RuleFor(u => u.Address, f=> new ContactFaker().GenerateData().Generate(1).FirstOrDefault() )
                //.RuleFor(u => u.Logo, f => new FileDocumentFaker().GenerateData().Generate(1).FirstOrDefault())
                //.RuleFor(u => u.FavIcon, f => new FileDocumentFaker().GenerateData().Generate(1).FirstOrDefault())
                //.RuleFor(u => u.Attachments, f => new FileDocumentFaker().GenerateData().Generate(2))
            
                .RuleFor(u => u.TotalStaffs, f => f.Random.Int(10, 300))
                .RuleFor(u => u.TotalStudents, f => f.Random.Int(100, 3000))
                
                .RuleFor(u => u.TotalFees, f => f.Random.Double(10000, 150000))
                .RuleFor(u => u.TotalDues, f => f.Random.Double(5000, 50000))

                .RuleFor(u => u.Principal, f=> new QuickViewFaker().GenerateData().Generate(1).FirstOrDefault())
                .RuleFor(u => u.AcademyYear, f=> new QuickViewFaker().GenerateData().Generate(1).FirstOrDefault())
                
                .RuleFor(u => u.AccountId, f => "")
                .RuleFor(u => u.EnteredBy, f => "")
                .RuleFor(u => u.IsActive, f=> f.Random.Bool())
                .RuleFor(u => u.IsDeleted, f=> f.Random.Bool())
            
                .RuleFor(u => u.Created, f => f.Date.Between(new DateTime(1900, 01, 01), DateTime.Now))
                .RuleFor(u => u.LastModified, f=> DateTime.Now)
            ;
        #endregion

        return bogusData;
    }
}