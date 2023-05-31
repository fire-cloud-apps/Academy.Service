using Academy.Entity.Management;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Entity.DataBogus.Management;

    public class AcademicalYearFaker : IGenerateFake<AcademicalYear>
{
    public Faker<AcademicalYear> GenerateData()
    {
        #region Document Faker
                
        var bogusModules = new Faker<AcademicalYear>()
             .RuleFor(u => u.Name, f => f.Person.FullName)
             .RuleFor(u => u.Description, f => f.Lorem.Lines(2))
             .RuleFor(u => u.YearStart, f => f.Date.Between(new DateTime(2023, 01, 01), DateTime.Now))
             .RuleFor(u => u.YearEnd, f => f.Date.Between(new DateTime(2023, 12, 31), DateTime.Now))
            ;

        #endregion

        return bogusModules;
    }
}

