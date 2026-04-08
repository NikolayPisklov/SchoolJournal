using SchoolJournalApi.Dtos.User;

namespace SchoolJournalApi.Services.AppServices.Interfaces
{
    public interface IStudentClassService
    {
        Task<List<ListedStudentDto>> GetStudentsInClassAsync(int classId);
        Task TransferStudentToAnotherCLassAsync(int studentId, int newClassId);
    }
}
