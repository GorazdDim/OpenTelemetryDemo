using Microsoft.EntityFrameworkCore;
using OpenTelemetryDemo.EF.Entities;
using OpenTelemetryDemo.Repositories.Interfaces;

namespace OpenTelemetryDemo.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public StudentRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<Student>> GetAllStudents()
        {
            return await _applicationDbContext.Students.ToListAsync();
        }

        public async Task<Student> GetStudentById(int id)
        {
            return await _applicationDbContext.Students.SingleAsync(s => s.Id == id);
        }
        public async Task<bool> SaveStudent(Student student)
        {
            try
            {
                await _applicationDbContext.Students.AddAsync(student);
                await _applicationDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateStudent(Student student)
        {
            try
            {
                var existingStudent = await _applicationDbContext.Students.FindAsync(student.Id);

                if (existingStudent is null) return false;

                existingStudent.Name = student.Name;
                existingStudent.Phone = student.Phone;

                await _applicationDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DeleteStudent(int id)
        {
            try
            {
                var existingStudent = await _applicationDbContext.Students.FindAsync(id);

                if (existingStudent is null) return false;

                _applicationDbContext.Students.Remove(existingStudent);
                await _applicationDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
