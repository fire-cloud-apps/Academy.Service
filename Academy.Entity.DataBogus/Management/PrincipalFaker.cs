using Academy.Entity.Management;
using Bogus;

namespace Academy.Entity.DataBogus.Management;

public class PrincipalFaker : IGenerateFake <Principal>
{
    public Faker<Principal> GenerateData()
    {
        #region Principal Faker
        var bogusPrincipal = new Faker<Principal>()
            .RuleFor(u => u.FullName, f => f.Person.FullName)
            .RuleFor(u => u.StaffId, "")
            .RuleFor(u => u.Email, f => f.Person.Email)
            .RuleFor(u => u.Mobile, f => f.Random.Replace("+91-#####-#####"))
            .RuleFor(u => u.Phone, f => f.Random.Replace("+91-#####-#####"));
        #endregion

        return bogusPrincipal;
    }

    
}

public class QuickViewFaker : IGenerateFake<QuickView>
{
    public Faker<QuickView> GenerateData()
    {
        #region Faker Rules
        var bogusFaker = new Faker<QuickView>()
            .RuleFor(u => u.Name, f => f.Person.FullName)
            .RuleFor(u => u.Description, f=> f.Person.Website);
        #endregion

        return bogusFaker;
    }

}