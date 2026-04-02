import axios from "axios";
import { educationalLevels } from "../enums/educationalLevels";

export const schoolJournalClient = axios.create({
  baseURL: "https://localhost:7062/api/",
  withCredentials: true
});

//Админ
export async function adminOnlyEndpoint(){
  try{
    const response = await schoolJournalClient.get('Home/admin-only-home');
    return response;
  }
  catch(error){
    throw(error);
  }
}
export async function getUserDetails(userId){
  try{
    const response = await schoolJournalClient.get('Admin/get-user-details', {params:{id: userId}})
    return response
  }
  catch{
    throw(error)
  }
}
export async function updateUserDetails(updatedUserDto){
  try{
    const response = await schoolJournalClient.put('Admin/update-user-details', updatedUserDto)
    return response
  }
  catch(error){
    throw(error)
  }
}
export async function getUserStatuses(){
  try{
    const response = await schoolJournalClient.get('Admin/get-user-statuses')
    return response
  }
  catch(error){
    throw error
  }
}
export async function getUsersOnPage(statusId, search, pageSize, page = 1){
  try{
    const response = await schoolJournalClient.get('Admin/get-users-on-page', {params: 
      {status: statusId, search: search, pageSize: pageSize, page: page}})
    return response
  }
  catch(error){
    throw error
  }
}
export async function transferStudentToAnotherClass(transferStudentDto){
  try{
    const response = await schoolJournalClient.post('Admin/transfer-student', transferStudentDto)
    return response
  }
  catch(error){
    throw error
  }
}
export async function addUser(userDto){
  try{
    const response = await schoolJournalClient.post('Admin/add-user', userDto)
    return response
  }
  catch(error){
    throw error
  }
}
export async function deleteUser(userId){
  try{  
    const response = await schoolJournalClient.delete('Admin/delete-user', {params: {userId: userId}})
    return response
  }
  catch(error){
    throw error
  }
}

export async function getUserStatusId(userId){
  try{
    const response = await schoolJournalClient.get('Admin/get-user-status-id', {params: {id: userId}})
    return response
  }
  catch(error){
    throw error
  }
}
export async function getClassOfStudent(userId){
  try{
    const response = await schoolJournalClient.get('Admin/get-class-of-student', {params: {userId: userId}})
    return response
  }
  catch(error){
    throw error
  }
}
export async function getAllClasses(){
  try{
    const response = await schoolJournalClient.get('Admin/get-classes')
    return response
  }
  catch(error){
    throw error
  }
}
//Admin-Classes----------------------------------------------------------------------------------------------------
export async function getClassesOnPage(pageSize, educationalLevelId, page = 1){
  try{
    const response = await schoolJournalClient.get('Admin/get-classes-on-page', {params: 
      {pageSize: pageSize, educationalLevel: educationalLevelId, page: page}})
    return response
  }
  catch(error){
    throw error
  }
}
export async function getClassDetails(classId){
  try{
    const response = await schoolJournalClient.get('Admin/get-class-details', {params: {id: classId}})
    return response
  }
  catch(error){
    throw error
  }
}
export async function addClass(classDto){
  try{
    const response = await schoolJournalClient.post('Admin/add-class', classDto)
    return response
  }
  catch(error){
    throw error
  }
}
export async function updateClass(classDto){
  try{
    const response = await schoolJournalClient.put('Admin/update-class', classDto)
    return response    
  }
  catch(error){
    throw error
  }
}
export async function deleteClass(classId){
  try{
    const response = await schoolJournalClient.delete('Admin/delete-class', {params:{id:classId}})
    return response
  }
  catch(error){
    throw(error)
  }
}
//Admin-Teachers-and-Subjects--------------------------------------------------------------------------------------
export async function getSubjectsForTeacher(userId){
  try{
    const response = await schoolJournalClient.get('Admin/get-teacher-subject', {params: {id: userId}})
    return response 
  }
  catch(error){
    throw error
  }
}
export async function getAllSubjects(eduLevelId){
  try{
    const response = await schoolJournalClient.get('Admin/get-subjects', 
      {params:{eduLevelId: eduLevelId}})
    return response
  }
  catch(error){
    throw error
  }
}
export async function deleteTeacherSubject(teacherSubjectId){
  try{
    const response = await schoolJournalClient.delete('Admin/delete-teacher-subject', 
      {params:{id: teacherSubjectId}})
    return response
  }
  catch(error){
    throw error
  }
}
export async function addTeacherSubject(teacherSubjectDto){
  try{
    const response = await schoolJournalClient.post('Admin/add-teacher-subject', teacherSubjectDto)
    return response
  }
  catch(error){
    throw error
  }
}
export async function getAllTeacherSubjects(eduLevelId){
  try{
    const response = await schoolJournalClient.get('Admin/get-all-teachers-subjects', 
      {params:{eduLevelId: eduLevelId}})
    return response
  }
  catch(error){
    throw error
  }
}
//Admin Journals--------------------------------------------------------------------------------------------------
export async function getJournalsForClass(classId){
  try{
    const response = await schoolJournalClient.get('Admin/get-journals-for-class', {params:{classId:classId}})
    return response
  }
  catch(error){
    throw error
  }
}
export async function addJournal(journalDto){
  try{
    const response = await schoolJournalClient.post('Admin/add-journal', journalDto)
    return response
  }
  catch(error){
    throw error
  }
}
export async function deleteJournal(journalId){
  try{
    const response = await schoolJournalClient.delete('Admin/delete-journal', {params:{journalId: journalId}})
    return response
  }
  catch(error){
    throw(error)
  }
}
//TeacherJournals-------------------------------------------------------------------------------------------------
export async function getJournalsForTeacher(teacherId){
  try{
    const response = await schoolJournalClient.get('Teacher/get-journals-for-teacher', 
      {params:{teacherId:teacherId}})
    return response
  }
  catch(error){
    throw(error)
  }
}
export async function getJournalTitle(journalId){
  try{
    const response = await schoolJournalClient.get('JournalCommon/get-journal-title',
      {params:{journalId: journalId}})
    return response
  }
  catch(error){
    throw(error)
  }
}
export async function getJournalDetails(journalId){
  try{
    const response = await schoolJournalClient.get('Teacher/get-journal-details'
      ,{params:{journalId: journalId}})
    return response
  }
  catch(error){
    throw(error)
  }
}
//Progresses
export async function getAllMarks(){
  try{
    const response = await schoolJournalClient.get('Teacher/get-all-marks')
    return response
  }
  catch(error){
    throw error
  }
}
export async function getAllAttendances(){
  try{
    const response = await schoolJournalClient.get('Teacher/get-all-attendances')
    return response
  }
  catch(error){
    throw error
  }
}
export async function addProgress(progressDto){
  try{
    const response = await schoolJournalClient.post('Teacher/add-progress', progressDto)
    return response
  }
  catch(error){
    throw error
  }
}
export async function updateProgress(progressDto){
  try{
    const response = await schoolJournalClient.put('Teacher/update-progress', progressDto)
    return response
  }
  catch(error){
    throw error
  }
}
export async function getProgressesForJournal(journalId){
  try{
    const response = await schoolJournalClient.get('Teacher/get-progresses-of-journal',
      {params:{journalId: journalId}})
    return response
  }
  catch(error){
    throw error
  }
}
export async function getProgressDetails(progressId){
  try{
    const response = await schoolJournalClient.get('Teacher/get-progress-details',
      {params:{progressId:progressId}})
    return response
  }
  catch(error){
    throw error
  }
}
export async function addLesson(addLessonDto){
  try{
    const response = await schoolJournalClient.post('Teacher/add-lesson', addLessonDto)
    return response
  }
  catch(error){
    throw error
  }
}
export async function getLessonsForJournal(journalId, month, journalYear){
  try{
    const response = await schoolJournalClient.get('Teacher/get-lessons-for-journal',
      {params: {journalId: journalId, month: month, journalYear: journalYear}}
    )
    return response
  }
  catch(error){
    throw error
  }
}
export async function updateLessonDetails(lessonId, theme, homework){
  try{
    const response = await schoolJournalClient.patch(`Teacher/${lessonId}`, {theme: theme, homework: homework})
    return response
  }
  catch(error){
    throw error
  }
}
export async function getStudentStatistic(studentId, journalId){
  try{
    const response = await schoolJournalClient.get('JournalCommon/get-student-statictic',
      {params: {studentId: studentId, journalId: journalId}}
    )
    return response
  }
  catch(error){
    throw error
  }
}
//Student----------------------------------------------------------------------------------------------------
export async function getJournalsForStudent(studentId){
  try{
    const response = await schoolJournalClient.get('Student/get-journals-for-student',
      {params: {studentId: studentId}}
    )
    return response
  }
  catch(error){
    throw error
  }
}
export async function getJournalDetailsForStudent(journalId, studentId){
  try{
    const response = await schoolJournalClient.get('Student/get-journal-details-for-student',
      {params: {journalId: journalId, studentId: studentId}}
    )
    return response
  }
  catch(error){
    throw error
  }
}
