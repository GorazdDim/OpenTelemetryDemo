using OpenTelemetryDemo.EF.Entities;
using OpenTelemetryDemo.Repositories.Interfaces;
using OpenTelemetryDemo.Services.Interfaces;

namespace OpenTelemetryDemo.Services
{
    public class ProfessorService : IProfessorService
    {
        private readonly IProfessorRepository _professorRepository;

        public ProfessorService(IProfessorRepository professorRepository)
        {
            _professorRepository = professorRepository;
        }

        public async Task<List<Professor>> GetAllProfessors() => await _professorRepository.GetAllProfessors();

        public async Task<Professor> GetProfessorById(int id) => await _professorRepository.GetProfessorById(id);

        public async Task<bool> SaveProfessor(Professor professor) => await _professorRepository.SaveProfessor(professor);

        public async Task<bool> UpdateProfessor(Professor professor) => await _professorRepository.UpdateProfessor(professor);
        public async Task<bool> DeleteProfessor(int id) => await _professorRepository.DeleteProfessor(id);
    }
}
