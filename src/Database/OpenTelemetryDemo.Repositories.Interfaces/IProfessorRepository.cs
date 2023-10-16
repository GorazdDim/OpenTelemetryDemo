using OpenTelemetryDemo.EF.Entities;

namespace OpenTelemetryDemo.Repositories.Interfaces
{
    public interface IProfessorRepository
    {
        public Task<List<Professor>> GetAllProfessors();
        public Task<Professor> GetProfessorById(int id);
        public Task<bool> SaveProfessor(Professor professor);
        public Task<bool> UpdateProfessor(Professor professor);
        public Task<bool> DeleteProfessor(int id);
    }
}
