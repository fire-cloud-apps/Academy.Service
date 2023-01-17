namespace Academy.Entity.DataBogus.Common;
using Academy.Entity.Common;
using Bogus;

public class ContactFaker : IGenerateFake <Contact>
{
    public Faker<Contact> GenerateData()
    {
        #region Contact Faker
        var bogusContact = new Faker<Contact>()
            .RuleFor(u => u.AddressLine1, f => f.Address.StreetAddress())
            .RuleFor(u => u.AddressLine2, f => f.Address.FullAddress())
            .RuleFor(u => u.BuildingNo, f => f.Address.BuildingNumber())
            .RuleFor(u => u.PinCode, f => f.Address.ZipCode())
            .RuleFor(u => u.City, f => f.Address.City())
            .RuleFor(u => u.Country, f => f.Address.Country())
            .RuleFor(u => u.PrimaryPhoneNo, f => f.Random.Replace("+91-#####-#####"))
            .RuleFor(u => u.SecondaryPhoneNo, f => f.Random.Replace("+91-#####-#####"))
            .RuleFor(u => u.Email, f => f.Person.Email)
            .RuleFor(u => u.Notes, f => f.Internet.UserAgent());
        
        #endregion

        return bogusContact;
    }
}