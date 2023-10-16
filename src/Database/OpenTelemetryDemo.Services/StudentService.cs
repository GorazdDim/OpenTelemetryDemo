using OpenTelemetryDemo.EF.Entities;
using OpenTelemetryDemo.Repositories.Interfaces;
using OpenTelemetryDemo.Services.Interfaces;

namespace OpenTelemetryDemo.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }


        public async Task<List<Student>> GetAllStudents() => await _studentRepository.GetAllStudents();

        public async Task<Student> GetStudentById(int id) => await _studentRepository.GetStudentById(id);

        public async Task<bool> SaveStudent(Student student) => await _studentRepository.SaveStudent(student);

        public async Task<bool> UpdateStudent(Student student) => await _studentRepository.UpdateStudent(student);

        public async Task<bool> DeleteStudent(int id) => await _studentRepository.DeleteStudent(id);
    }
}
