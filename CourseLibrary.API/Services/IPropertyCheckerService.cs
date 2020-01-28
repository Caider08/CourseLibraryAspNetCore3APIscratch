namespace CourseLibrary.API.Services
{
    public interface IPropertyCheckerService
    {
        bool TypehasProperties<T>(string fields);
    }
}