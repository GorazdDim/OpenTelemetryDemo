using OpenTelemetryDemo.EF.Entities;
using OpenTelemetryDemo.Services.Interfaces;
using OpenTelemetryDemo.Shared;
using System.Diagnostics;
using OpenTelemetryDemo.Repositories.Interfaces;

namespace OpenTelemetryDemo.Services
{
    public class ProfessorService : IProfessorService
    {
        private readonly IProfessorRepository _professorRepository;

        public ProfessorService(IProfessorRepository professorRepository)
        {
            _professorRepository = professorRepository;
        }

        public async Task<List<Professor>> GetAllProfessors()
        {
            using var activity = CustomTraces.Default.StartActivity("GetAllProfessorsServiceMethod");
            activity?.SetTag("instance.type", GetType());
            Activity.Current?.AddBaggage("sample.Source", ToString());
            return await _professorRepository.GetAllProfessors();
        }

        public async Task<Professor> GetProfessorById(int id) => await _professorRepository.GetProfessorById(id);

        public async Task<bool> SaveProfessor(Professor professor) => await _professorRepository.SaveProfessor(professor);

        public async Task<bool> UpdateProfessor(Professor professor) => await _professorRepository.UpdateProfessor(professor);
        public async Task<bool> DeleteProfessor(int id) => await _professorRepository.DeleteProfessor(id);
    }
}
