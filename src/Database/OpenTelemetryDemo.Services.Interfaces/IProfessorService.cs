using OpenTelemetryDemo.EF.Entities;

namespace OpenTelemetryDemo.Services.Interfaces
{
    public interface IProfessorService
    {
        public Task<List<Professor>> GetAllProfessors();
        public Task<Professor> GetProfessorById(int id);
        public Task<bool> SaveProfessor(Professor professor);
        public Task<bool> UpdateProfessor(Professor professor);
        public Task<bool> DeleteProfessor(int id);
    }
}
