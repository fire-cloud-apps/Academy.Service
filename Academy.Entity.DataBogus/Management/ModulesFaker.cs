using Academy.Entity.Common;
using Academy.Entity.Management;
using Bogus;

namespace Academy.Entity.DataBogus.Management;

public class ModulesFaker :IGenerateFake <MenuModule>
{
    public Faker<MenuModule> GenerateData()
    {
        #region Document Faker

        var moduleTypes = new[]
        {
            "Account", "Students", "Teachers", "Courses",
            "Examination", "Attendance", "Timetable", "Fees", "Reports", "Settings",
            "Transport", "Library", "Downloads", "Import", "Export", "Notify", "Purchase", "HR", "Visitor"
        };
        var bogusModules = new Faker<MenuModule>()
            .RuleFor(u => u.Name, f => f.PickRandom(moduleTypes))
            .RuleFor(u => u.Description, 
                f => f.Lorem.Lines(2));
            
        #endregion

        return bogusModules;
    }
}