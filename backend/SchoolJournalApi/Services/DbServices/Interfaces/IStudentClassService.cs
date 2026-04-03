using SchoolJournalApi.Dtos.User;

namespace SchoolJournalApi.Services.DbServices.Interfaces
{
    public interface IStudentClassService
    {
        Task<List<ListedStudentDto>> GetStudentsInClassAsync(int classId);
        Task DeleteStudentClassAsync(int studentClassId);
        Task TransferStudentToAnotherCLassAsync(int studentId, int newClassId, int? oldClassId);
    }
}
