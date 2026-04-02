<template>
    <div class="flex justify-center mt-3">
        <h1 class="text-2xl" v-if="journalTitleInfo">Журнал класса 
            {{calculateClassYear(journalTitleInfo.classYear)}}-{{ journalTitleInfo.classTitle }}
            по предмету "{{ journalTitleInfo.subjectTitle }}"
        </h1>
    </div>
    <div class="m-3">
        <table>
            <thead>
                <tr>
                    <th class="p-1 border border-gray-500 w-auto bg-gray-100">Ученики</th>
                    <th v-for="l in lessons" class="p-1 border border-gray-500 w-[60px] bg-gray-100">
                        {{ new Date(l.lessonDate).getDate() }}.{{ new Date(l.lessonDate).getMonth() + 1}}
                    </th>
                    <th v-if="session.role === 'teacher'" class="border border-gray-500 p-1 w-[60px]">
                        <button class="h-full w-full block cursor-pointer rounded-sm"
                            @click="openLessonWindow">
                            +
                        </button>
                    </th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="s in students" class="">
                    <td class="border border-gray-500 p-1">
                        <button class="cursor-pointer" @click="openStatisticWindow(s.id)">
                            {{ s.lastName }} {{ s.firstName[0] }}. {{ s.middleName[0] }}.
                        </button>
                    </td>
                    <td v-for="l in lessons" class="border border-gray-500 p-0.5 h-1">
                        <div v-if="session.role === 'teacher'" class="w-full h-full">
                            <button v-if="getProgress(s.id, l.id)"
                                class="h-full w-full block cursor-pointer rounded-sm"
                                @click="openUpdateProgressWindow(getProgress(s.id, l.id).id)">
                                <span v-if="getProgress(s.id, l.id).markValue">
                                    {{ getProgress(s.id, l.id).markValue }}    
                                </span>
                                <span v-else>
                                    {{ getProgress(s.id, l.id).attendanceValue }}
                                </span>
                            </button>
                            <button v-else class="h-full w-full block cursor-pointer
                                bg-red-100/30 rounded-sm"
                                @click="openAddingProgressWindow(s.id, l.id)">
                            </button>
                        </div>
                        <div v-if="session.role === 'admin'">
                            <div v-if="getProgress(s.id, l.id)" class="flex justify-center items-center">
                                <span v-if="getProgress(s.id, l.id).markValue">
                                    {{ getProgress(s.id, l.id).markValue }}    
                                </span>
                                <span v-else>
                                    {{ getProgress(s.id, l.id).attendanceValue }}
                                </span>
                            </div>
                            <div v-else>

                            </div>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <div v-if="isProgressWindowOpen"
        class="fixed inset-0 flex items-center justify-center bg-black/70">
        <div class="bg-white w-[40%] rounded-md flex justify-center">         
            <form @submit.prevent="submitForm" class="m-3">
                <div class="text-xl mb-2">
                    Форма редактирования оценки
                </div>
                <div>
                    Ученик(ца): {{ selectedStudent.lastName }} {{ selectedStudent.firstName }} {{ selectedStudent.middleName }}<br/>
                    Дата урока: {{ progressLesson.lessonDate }}
                </div>
                <div class="mt-2">
                    <div>
                        <span class="error" v-if="progressError !== ''">{{ progressError }}</span>
                    </div>    
                    <div>
                        <p class="error" v-if="progressValidationErrors.ProgressUpdateDate !== ''">
                            {{ progressValidationErrors.ProgressUpdateDate }}
                        </p>
                    </div>
                    <div>
                        <label class="mr-1">Оценка:</label>
                    </div>
                    <div>
                        <select class="text-box" v-model="progress.markId">
                            <option :value="null">Выберите оценку</option>
                            <option :value="null">Без оценки</option>
                            <option v-for="m in allMarks"
                                :key="m"
                                :value="m.id">
                                {{ m.value }}
                            </option>
                        </select>
                    </div>
                </div>
                <div class="mt-2">
                    <div>
                        <label class="mr-1">Посещаемость:</label>
                    </div>
                    <div>
                        <span class="error" v-if="progressValidationErrors.AttendanceId !== ''">{{ progressValidationErrors.AttendanceId }}</span>
                    </div>
                    <div>
                        <select class="text-box" v-model="progress.attendanceId">
                            <option :value=null>Выберите посещаемость</option>
                            <option v-for="a in allAttendances"
                                :key="a"
                                :value="a.id">
                                {{ a.value }}
                            </option>
                        </select>
                    </div>
                </div>
                <div class="mt-2">
                    <button v-if="isProgressAddingForm" class="btn-primary mr-2 cursor-pointer"
                        @click="onProgressAddFormSubmitClick" type="button">
                        Сохранить изменения
                    </button>
                    <button v-if="isProgressEditForm" class="btn-primary mr-2 cursor-pointer"
                        @click="onProgressUpdateFormSubmitClick" type="button">
                        Сохранить изменения
                    </button>
                    <button @click="closeProgressWindow" class="btn-gray p-1 cursor-pointer"
                        type="button">
                        Отмена
                    </button>
                </div>
                <div v-if="isProgressEditForm" class="mt-2">
                    <div class="text-xl mb-2">
                        История изменения
                    </div>
                    <div v-if="progress.progressChangeHistory.length > 0">
                        <ul>
                            <li v-for="p in progress.progressChangeHistory">
                                Оценка: {{ p.markValue }},
                                Посещаемость: {{ p.attendanceValue }},
                                Время:
                                {{ new Date(p.progressUpdateTime).toLocaleDateString(
                                    'ru-RU', {
                                    day: '2-digit',
                                    month: '2-digit',
                                    year: 'numeric',
                                    hour: '2-digit',
                                    minute: '2-digit'
                                 })}}
                            </li>
                        </ul>
                    </div> 
                    <div v-else>
                        Нет истории изменений успеваемости за данный урок.
                    </div>
                </div>
            </form>           
        </div>
    </div>
    <div v-if="isLessonWindowOpen" class="fixed inset-0 flex items-start justify-center bg-black/70 pt-50">
        <div class="bg-white w-[30%] rounded-md flex justify-center"> 
            <form @submit.prevent="submitForm" class="m-3">
                <div>
                    <label>
                        Выберите дату урока
                    </label>
                </div>
                <div>
                    <span class="error" v-if="lessonError !== ''">
                        {{ lessonError }}
                    </span>
                </div>
                <div>
                    <VueDatePicker 
                        :time-config="{enableTimePicker: false}"
                        :locale="ru"
                        :action-row="{
                            selectBtnLabel: 'Выбрать',
                            cancelBtnLabel: 'Отмена'
                        }"
                        :formats="{input: 'dd.MM.yyyy', preview: 'dd.MM.yyyy'}"
                        v-model="newLessonDate"
                    />
                </div>
                <div class="mt-2">
                    <button class="cursor-pointer btn-primary me-2" @click="onLessonAddSubmitClick"
                        type="button">
                        Добавить урок
                    </button>
                    <button class="cursor-pointer btn-gray p-1" @click="closeLessonWindow"
                        type="button">
                        Отмена
                    </button>
                </div>
            </form>
        </div>
    </div>
    <div class="ms-3 mt-2">
        <span>Выберите месяц:</span>
        <select v-model="selectedMonth" @change="onMonthSelectChange" class="text-box ms-2">
            <option v-for="m in months"
            :value="m.monthNumber">
                {{ m.month }} - {{ m.year }}
            </option>
        </select>
    </div>
    <div class="mt-3 mb-3">
        <div class="grid grid-cols-10 border-b border-b-gray-300 
            ps-3 pe-3 gap-3" v-for="l in lessons">
            <div class="col-span-1 border-r border-gray-300 pt-2">
                {{ new Date(l.lessonDate).toLocaleDateString('ru') }}
            </div>
            <div class="col-span-3 border-r border-gray-300 pt-2">
                <p>Тема: {{l.theme}}</p>
            </div>
            <div v-if="session.role === 'teacher'" class="col-span-5 border-r border-gray-300 pt-2">
                <p class="wrap-break-word">Д/З: {{ l.homework }}</p>
            </div>
            <div class="col-span-1" v-if="session.role === 'teacher'">
                <button class="btn-gray p-0.5 m-1" @click="openLessonDetailsWindow(l)">
                    Редактировать
                </button>
            </div>
            <div v-if="session.role === 'admin'" class="col-span-6 border-gray-300 pt-2">
                <p class="wrap-break-word">Д/З: {{ l.homework }}</p>
            </div>  
        </div>
    </div>
    <div v-if="isLessonDetailsWindowOpen" class="fixed inset-0 flex items-center justify-center bg-black/70">
        <div class="bg-white w-[50%] rounded-md flex justify-center">
            <form @submit.prevent="submitForm" class="m-3 w-[80%]">
                <span>Форма редактирования описания урока</span>
                <div class="mt-3">
                    <label>Тема урока:</label>
                </div>
                <div class="flex">
                    <input class="text-box w-2xl" v-model="newLessonTheme"/>
                </div>
                <div class="mt-3">
                    <label>Домашнее задание:</label>
                </div>
                <div class="flex">
                    <textarea class="text-box w-2xl h-40" v-model="newLessonHomework"></textarea>
                </div>
                <div class="mt-3">
                    <button type="button" class="btn-primary me-2"
                        @click="onLessonDetailsSubmitClick">
                        Сохранить изменения
                    </button>
                    <button type="button" @click="closeLessonDetailsWindow" class="btn-gray p-1">
                        Отмена
                    </button>
                </div>               
            </form>
        </div>
    </div>
    <Transition
        enter-active-class="transition duration-200 ease-out"
        enter-from-class="opacity-0"
        enter-to-class="opacity-100"
        leave-active-class="transition duration-200 ease-in"
        leave-from-class="opacity-100"
        leave-to-class="opacity-0"
    >
    <div v-if="isChartWindowOpen" class="fixed inset-0 flex items-center justify-center bg-black/70">
        <div class="bg-white w-[70%] rounded-sm justify-center p-3">
            <div>
                <p>Статистика успеваемости ученика(цы): {{ selectedStudent.lastName }} {{ selectedStudent.firstName }} {{ selectedStudent.middleName }}</p>
            </div>
            <div class="m-3" v-if="chartData.datasets && chartData.labels">
                <MyChart :chartData="chartData" :options="chartOptions" />
            </div>
            <div>
                <button class="btn-gray" @click="onStatisticWindowCloseClick">
                    Закрыть
                </button>
            </div>
        </div>
    </div> 
</Transition>   
</template>
<script setup>
    import { computed, inject, onMounted, ref } from 'vue';
    import { useRoute } from 'vue-router';
    import {VueDatePicker} from '@vuepic/vue-datepicker'
    import '@vuepic/vue-datepicker/dist/main.css'
    import {ru} from 'date-fns/locale'
    import MyChart from '../components/Chart.vue'
    import { useUserStore } from '../stores/user';

    const session = useUserStore()
    const route = useRoute()
    const api = inject('$api')

    const journalTitleInfo = ref(null)
    const journalId = route.params.id
    const journalYear = ref(0)
    const students = ref([])
    const lessons = ref([])
    const progresses = ref([])
    const months = ref([])

    const selectedMonth = ref(9)
    const selectedLesson = ref(null)
    const selectedStudent = ref(null)

    const allMarks = ref([])
    const allAttendances = ref([])
    const progressLesson = ref(null)

    const isProgressWindowOpen = ref(false)
    const isLessonDetailsWindowOpen = ref(false)
    const isLessonWindowOpen = ref(false)
    const isProgressEditForm = ref(false)
    const isProgressAddingForm = ref(false)
    const isChartWindowOpen = ref(false)

    const lessonError = ref('')
    const progressValidationErrors = ref({AttendanceId: '', MarkId: '', LessonId: '', UserId: '', ProgressUpdateDate: ''})
    const progressError = ref('')

    const newLessonDate = ref(null)
    const newLessonTheme = ref('')
    const newLessonHomework = ref('')

    const progress = ref({
        id: null,
        userId: null,
        lessonId: null,
        markId: null,
        attendanceId: null,
        progressUpdateDate: null,
        lessonDate: null,
        progressChangeHistory: []
    })
    const addProgressDto = computed(() => ({
        userId: progress.value.userId,
        lessonId: progress.value.lessonId,
        markId: progress.value.markId,
        attendanceId: progress.value.attendanceId,
        progressUpdateDate: progress.value.progressUpdateDate,
        lessonDate: progress.value.lessonDate
    }))
    const updateProgressDto = computed(() => ({
        id: progress.value.id,
        userId: progress.value.userId,
        lessonId: progress.value.lessonId,
        markId: progress.value.markId,
        attendanceId: progress.value.attendanceId,
        progressUpdateDate: progress.value.progressUpdateDate,
        lessonDate: progress.value.lessonDate
    }))
    const getProgress = computed(() => {
        return (studentId, lessonId) => {
            return progresses.value.find(p =>
            p.userId === studentId && p.lessonId === lessonId
            );
        };
    });
    const chartData = ref({
        labels: null, 
        datasets: null 
    })

    const chartOptions = {
        responsive: true
    }

    onMounted (async () =>{
        try{
            const titleResponse = await api.getJournalTitle(journalId)
            journalTitleInfo.value = titleResponse.data
            const detailsResponse = await api.getJournalDetails(journalId)
            students.value = detailsResponse.data.students
            lessons.value = detailsResponse.data.lessons
            progresses.value = detailsResponse.data.progresses
            journalYear.value = detailsResponse.data.journalYear
            setMonths()
            await setDropdownListsArrays()
        }
        catch(error){
            console.log(error)
        }       
    })

    const setDropdownListsArrays = async () =>{
        try{
            const marksResponse = await api.getAllMarks()
            allMarks.value = marksResponse.data
            const attendanceResponse = await api.getAllAttendances()
            allAttendances.value = attendanceResponse.data  
        }
        catch(error){
            console.log(error)
        }       
    }
    const onProgressAddFormSubmitClick = async () =>{
        try{
            progress.value.progressUpdateDate = new Date().toISOString()
            progress.value.lessonDate = lessons.value.find(l => l.id === progress.value.lessonId).lessonDate
            await api.addProgress(addProgressDto.value)
            closeProgressWindow()
            clearMessages()
            const response = await api.getProgressesForJournal(journalId)
            progresses.value = response.data
        }
        catch(error){
            const responseData = error.response.data
            handleProgressChangingExceptions(responseData)          
        }
    }
    const onProgressUpdateFormSubmitClick = async () =>{
        try{
            progress.value.progressUpdateDate = new Date().toISOString()
            progress.value.lessonDate = lessons.value.find(l => l.id === progress.value.lessonId).lessonDate
            await api.updateProgress(updateProgressDto.value)
            closeProgressWindow()
            clearMessages()
            const response = await api.getProgressesForJournal(journalId)
            progresses.value = response.data
        }
        catch(error){
            const responseData = error.response.data
            handleProgressChangingExceptions(responseData)
        }
    }
    function calculateClassYear(classYear){
        return new Date().getFullYear() - classYear
    }
    const openUpdateProgressWindow = async(progressId) =>{
        try{
            const response = await api.getProgressDetails(progressId)
            Object.assign(progress.value, response.data)
            selectedStudent.value = students.value
                .find(s => s.id === progress.value.userId)
            progressLesson.value = lessons.value
                .find(l => l.id === progress.value.lessonId)
            isProgressEditForm.value = true
            isProgressAddingForm.value = false
            isProgressWindowOpen.value = true
        }
        catch(error){
            progressError.value = 'Ошибка при загркзке данных!'
            console.log(error)
        }
    }
    const onLessonAddSubmitClick = async () =>{
        try{
            const newLessonDto = {
                journalId: journalId,
                lessonDate: newLessonDate.value?.toISOString().split('T')[0]
            }
            await api.addLesson(newLessonDto)
            const response = await api.getLessonsForJournal(journalId, selectedMonth.value, journalYear.value)
            lessons.value = response.data
            closeLessonWindow()
        }
        catch(error){
            const responseData = error.response.data
            if(responseData.StatusCode === 409){
                if(responseData.ErrorCode === 'ENTITY_ADDING_ERROR')
                    lessonError.value = 'Ошибка при добаввлении урока! Повторите попытку позже.'
                if(responseData.ErrorCode === 'ENTITY_LOGIC_CONFLICT_ERROR')
                    if(responseData.Message === `Lessons can't be on weekends`){
                        lessonError.value = 'Урок нельзя поставить в выходной.'
                    }
                    else{
                        lessonError.value = `Дата урока выходит за пределы учебного года журнала 
                        ${journalYear.value.toString().slice(-2)}/${(journalYear.value + 1).toString().slice(-2)}`
                    }   
            }
            else if(responseData.StatusCode === 404){
                lessonError.value = 'Ошибка при привязке урока к журналу. Повторите попытку позже.'
            }
            else if(responseData.status === 400){
                lessonError.value = 'Дата урока обязательна для заполнения! '
            }
            else{
                console.log(error.response)
            }
        }
    }
    const onMonthSelectChange = async() =>{
        try{
            const response = await api.getLessonsForJournal(journalId, selectedMonth.value, journalYear.value)
            lessons.value = response.data
        }
        catch(error){
            console.log(error.response)
        }       
    }
    function openLessonDetailsWindow(lesson){
        selectedLesson.value = lesson
        isLessonDetailsWindowOpen.value = true
    }
    function closeLessonDetailsWindow(){
        isLessonDetailsWindowOpen.value = false
        clearMessages() 
    }
    function openAddingProgressWindow(studentId, lessonId){
        try{
            progress.value.lessonId = lessonId
            progress.value.userId = studentId
            selectedStudent.value = students.value.find(s => s.id === studentId)
            progressLesson.value = lessons.value.find(l => l.id === lessonId)
            isProgressEditForm.value = false
            isProgressAddingForm.value = true
            isProgressWindowOpen.value = true
        }
        catch(error){
            console.log(error)
        }
    }
    function openLessonWindow(){
        isLessonWindowOpen.value = true
    }

    function clearMessages(){
        progressError.value = ''
        lessonError.value = ''
        for (const key in progressValidationErrors.value) {
            progressValidationErrors.value[key] = '';
        }
    }
    function closeLessonWindow(){
        isLessonWindowOpen.value = false
        newLessonDate.value = null
        clearMessages()
    }
    function closeProgressWindow(){
        clearMessages() 
        isProgressWindowOpen.value = false
        progressLesson.value = null
        selectedStudent.value = null
        progress.value.id = null
        progress.value.markId = null
        progress.value.attendanceId = null
        progress.value.lessonId = null
        progress.value.userId = null
        progress.value.progressUpdateDate = null
        progress.value.lessonDate = null
    }
    function setMonths(){
        months.value = [
            {month: 'Сентябрь', year: journalYear.value, monthNumber: 9},
            {month: 'Октябрь', year: journalYear.value, monthNumber: 10},
            {month: 'Ноябрь', year: journalYear.value, monthNumber: 11},
            {month: 'Декабрь', year: journalYear.value, monthNumber: 12},
            {month: 'Январь', year: journalYear.value + 1, monthNumber: 1},
            {month: 'Февраль', year: journalYear.value + 1, monthNumber: 2},
            {month: 'Март', year: journalYear.value + 1, monthNumber: 3},
            {month: 'Апрель', year: journalYear.value + 1, monthNumber: 4},
            {month: 'Май', year: journalYear.value + 1, monthNumber: 5},
        ]
    }
    function handleProgressChangingExceptions(responseData){
        if(responseData.StatusCode === 409){
            if(responseData.ErrorCode === 'ENTITY_LOGIC_CONFLICT_ERROR'){
                progressError.value = 'Ученик не может получить оценку за занятие, которое он не посещал. Измените посещаемость ученика.'
            }
            if(responseData.ErrorCode === 'ENTITY_ADDING_ERROR'){
                progressError.value = 'Ошибка при сохранении изменений!'
            }
        }
        else if(responseData.status === 400){
            const modelStateErrors = responseData.errors
            for (const field in modelStateErrors){
                if(progressValidationErrors.value.hasOwnProperty(field)){
                    progressValidationErrors.value[field] = modelStateErrors[field][0]
                }
            }
        } 
        else{
            console.log(responseData)
        }
    }
    const onLessonDetailsSubmitClick = async() =>{
        try{
            await api.updateLessonDetails(selectedLesson.value.id, newLessonTheme.value, newLessonHomework.value)
            selectedLesson.value.homework = newLessonHomework.value
            selectedLesson.value.theme = newLessonTheme.value
            closeLessonDetailsWindow()
        }
        catch(error){
            console.log(error.response)
        }       
    }
    const openStatisticWindow = async(studentId) =>{
        try{
            selectedStudent.value = students.value.find(s => s.id === studentId)
            const response = await api.getStudentStatistic(studentId, journalId)
            const responseData = response.data
            chartData.value = {
                labels: responseData.dateLabels,
                datasets: [
                    {
                        label: 'Средняя оценка',
                        data: responseData.avgMarks,
                        borderColor: 'blue',
                        tension: 0.3
                    },
                    {
                        label: 'Фактичесая оценка',
                        data: responseData.factMarks,
                        borderColor: 'lightgray',
                        tension: 0.3
                    }
                ]
            }
            isChartWindowOpen.value = true
        }
        catch(error){
            console.log(error.response)
        }        
    }
    function onStatisticWindowCloseClick(){
        chartData.value = {
            labels: null,
            datasets: null
        }
        selectedStudent.value = null
        isChartWindowOpen.value = false
    }
</script>