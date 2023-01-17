using Bogus;

namespace Academy.Entity.DataBogus;

public interface IGenerateFake<T> where T : class 
{
    public Faker<T> GenerateData();
}