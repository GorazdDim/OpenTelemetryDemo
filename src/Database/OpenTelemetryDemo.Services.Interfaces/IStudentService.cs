using OpenTelemetryDemo.EF.Entities;

namespace OpenTelemetryDemo.Services.Interfaces
{
    public interface IStudentService
    {
        public Task<List<Student>> GetAllStudents();
        public Task<Student> GetStudentById(int id);
        public Task<bool> SaveStudent(Student student);
        public Task<bool> UpdateStudent(Student student);
        public Task<bool> DeleteStudent(int id);
    }
}
