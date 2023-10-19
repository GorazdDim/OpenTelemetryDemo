using Microsoft.EntityFrameworkCore;
using OpenTelemetryDemo.EF.Entities;
using OpenTelemetryDemo.Repositories.Interfaces;
using System.Diagnostics;
using OpenTelemetryDemo.Shared;

namespace OpenTelemetryDemo.Repositories
{
    public class ProfessorRepository : IProfessorRepository
    {

        private readonly ApplicationDbContext _applicationDbContext;
        public ProfessorRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<Professor>> GetAllProfessors()
        {
            using var activity = CustomTraces.Default.StartActivity("GetAllProfessorsRepositoryMethod");
            activity?.SetTag("instance.type", GetType());
            var source = Activity.Current?.GetBaggageItem("sample.Source");
            activity?.SetTag("instance.parentSource", string.IsNullOrWhiteSpace(source) ? string.Empty : source);
            return await _applicationDbContext.Professors.ToListAsync();
        }

        public async Task<Professor> GetProfessorById(int id)
        {
            return await _applicationDbContext.Professors.SingleAsync(s => s.Id == id);
        }
        public async Task<bool> SaveProfessor(Professor professor)
        {
            try
            {
                await _applicationDbContext.Professors.AddAsync(professor);
                await _applicationDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateProfessor(Professor professor)
        {
            try
            {
                var existingProfessor = await _applicationDbContext.Professors.FindAsync(professor.Id);

                if (existingProfessor is null) return false;

                existingProfessor.FirstName = professor.FirstName;
                existingProfessor.LastName = professor.LastName;
                existingProfessor.DateOfBirth = professor.DateOfBirth;
                existingProfessor.Email = professor.Email;
                existingProfessor.Department = professor.Department;

                await _applicationDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DeleteProfessor(int id)
        {
            try
            {
                var existingProfessor = await _applicationDbContext.Professors.FindAsync(id);

                if (existingProfessor is null) return false;

                _applicationDbContext.Professors.Remove(existingProfessor);
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
