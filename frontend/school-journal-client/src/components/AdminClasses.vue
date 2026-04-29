<template>
    <div class="m-3">
        <div class="flex justify-center">
            <h1 class="text-2xl">Редактирование классов и их журналов</h1>
        </div>
        <div class="grid grid-cols-7 gap-3 mt-3">
            <div class="col-span-2">
                <label>Фильтр по уровню образования:</label>
                <select class="text-box ms-1" @change="onEduLevelChange" v-model="selectedEducationalLevel">
                    <option value="">Нет фильтров</option>
                    <option :value=educationalLevels.Junior>Начальные классы</option>
                    <option :value=educationalLevels.Middle>Средние классы</option>
                    <option :value=educationalLevels.Senior>Старшые классы</option>
                </select>
                <button type="submit" style="cursor: pointer" @click="onAddFormShowClick"
                    class="mt-1 btn-gray block" aria-label="Добавить пользователя">
                    Добавить класс
                </button>
                <span class="error mt-3" v-if="classesSearchError !== ''">{{ classesSearchError }}</span>
                <ul class="user-list mt-3">
                    <li v-for="theClass in classes" :key="theClass"
                        :class="{selected: theClass.id === selectedClassId}"
                        @click="selectClass(theClass.id)">
                        {{ new Date().getFullYear() - theClass.year }}-{{ theClass.title }}
                        | Набор {{ theClass.year }} г.
                    </li>
                </ul>
                <Pagination
                    :current-page="currentPage"
                    :number-of-pages="numberOfPages"
                    @change="onPageChange"
                />
            </div>
            <span v-if="classSuccessMessage != ''" class="text-green-700 block">{{ classSuccessMessage }}</span>
            <div v-if="isClassFormVisible" class="col-span-2 ms-3">
                <span v-if="isEditFormVisible">Форма редактирования выбранного класса</span>
                <span v-if="isAddingFormVisible">Форма добавления класса</span>
                
                <span v-if="classErrorMessage != ''" class="error block">{{ classErrorMessage }}</span>
                <form @submit.prevent="submitForm">
                    <label for="title" class="block">Название класса</label>
                    <span class="error block" v-if="errors.Title !== ''">{{ errors.Title }}</span>
                    <input class="text-box" v-model="selectedClass.title"/>
                    <div v-if="isAddingFormVisible">
                        <label for="title" class="block">Год набора класса</label>
                        <span class="error block" v-if="errors.Year !== ''">{{ errors.Year }}</span>
                        <input class="text-box" v-model="selectedClass.year"/>
                    </div>
                    <div v-if="isEditFormVisible">
                        <label for="title" class="block">Год набора класса</label>
                        <input :disabled="true" class="text-box text-gray-400" v-model="selectedClass.year"/>
                    </div>
                </form>
                <div v-if="isAddingFormVisible">
                    <button @click="onAddClassClick" type="submit" style="cursor: pointer"
                        class="mt-3 mx-auto btn-primary" 
                        aria-label="Добавить класс">
                        Добавить класс
                    </button>
                </div>
                <div v-if="isEditFormVisible">
                    <button @click="onEditClassClick" type="submit" style="cursor: pointer"
                        class="mt-3 mx-auto btn-primary" 
                        aria-label="Применить">
                        Применить
                    </button>
                    <button v-if="isEditFormVisible" type="submit" style="cursor: pointer" 
                        @click="onClassDeleteClick" class="ms-1 btn-red">
                        Удалить класс
                    </button>
                </div>
            </div>
            <div v-if="isClassFormVisible && selectedClassId" class="col-span-3 ms-3">
                <p>Журналы выбранного класса:</p>
                <span class="error block">{{ journalsError }}</span>
                <form @submit.prevent="submitForm" class="">
                    <ul class="list list-disc ms-5">
                        <li v-for="journal in journals"
                            :key="journal" class="mb-1">
                            {{ journal.subjectTitle }} - 
                            {{ journal.teacherLastName }} {{  journal.teacherFirstName[0]}}.
                            {{  journal.teacherMiddleName[0]}}. |
                            {{ journal.journalYear }} г.
                            <button type="submit" style="cursor: pointer" 
                                @click="onJournalDeleteClick(journal.id)" class="ms-1 btn-red p-0 ps-1 pe-1">
                                Удалить
                            </button>
                            <button class="cursor-pointer btn-gray ms-1" 
                                @click="navigateToClassJournal(journal.id)">
                                Просмотр 
                            </button>
                        </li>
                    </ul>
                    <div>
                        <div>
                            <span>Добавить журнал для выбранного класса:</span>
                        </div>
                        <div>
                            <span>Выберите предмет:</span>
                            <select class="text-box ms-1 mt-2" v-model="selectedTeacherSubjectId">
                                <option disabled value="">Выберите предмет</option>
                                <option v-for="ts in allTeacherSubjects"
                                    :key="ts"
                                    :value="ts.id">
                                    {{ ts.subjectTitle }} - 
                                    {{ ts.teacherLastName }} {{  ts.teacherFirstName[0]}}.
                                    {{ ts.teacherMiddleName[0]}}.
                                </option>
                            </select>
                        </div>
                        <button class="btn-primary block mt-2"
                            type="submit"
                            @click="onJournalAddClick">
                            Добавить журнал
                        </button>
                    </div>
                </form>             
            </div>
        </div>
    </div>
</template>
<script setup>
    import { educationalLevels } from '../enums/educationalLevels';
    import { inject, ref, onMounted, nextTick, computed } from 'vue';
    import Pagination from './Pagination.vue';
    import { useRouter } from 'vue-router';

    const api = inject('$api')
    const router = useRouter() 

    const isClassFormVisible = ref(false)
    const isAddingFormVisible = ref(false)
    const isEditFormVisible = ref(false)
    const classSuccessMessage = ref('')

    const selectedEducationalLevel = ref('')
    const selectedClassId = ref(null)
    const selectedTeacherSubjectId = ref('')

    const selectedClass = ref({
        id: null,
        educationalLevelId: null,
        year: null,
        title: ''
    })
    const addDto = computed(() => ({
        year: selectedClass.value.year,
        title: selectedClass.value.title
    }))
    const updateDto = computed(() => ({
        id: selectedClass.value.id,
        year: selectedClass.value.year,
        title: selectedClass.value.title
    })) 

    const currentPage = ref(1)
    const pageSize = 5
    const numberOfPages = ref()

    const classes = ref([])
    const journals = ref([])
    const allTeacherSubjects = ref([])

    const classErrorMessage = ref('')
    const journalsError = ref('')
    const errors = ref({Title: '', Year: ''})

    const classesSearchError = ref('')
    onMounted(async () => {
        try{
            const response = await api.getClassesOnPage(pageSize, selectedEducationalLevel.value, currentPage.value)
            assignClassesOnChange(response)
        }
        catch(error){
            if(error.response.status === 404){
                classes.value = null
                classesSearchError.value = 'Классов по вашему запросу не найдено.'
            }
            else{
                console.log(error.response)
            }           
        }
    })
    const onEduLevelChange = async () => {
        try{
            isClassFormVisible.value = false
            clearMessages()
            const response = await api.getClassesOnPage(pageSize, selectedEducationalLevel.value, currentPage.value)
            assignClassesOnChange(response)
        }
        catch(error){
            if(error.response.status === 404){
                classes.value = null
                classesSearchError.value = 'Классов по вашему запросу не найдено.'
            }
            else{
                console.log(error.response)
            }
        }      
    }
    const onPageChange = async (page) => {
        try{
            clearMessages()
            currentPage.value = page
            const response = await api.getClassesOnPage(pageSize, selectedEducationalLevel.value, currentPage.value)
            assignClassesOnChange(response)
        }
        catch(error){
            if(error.response.status === 404){
                classes.value = null
                classesSearchError.value = 'Классов по вашему запросу не найдено.'
            }
            else{
                console.log(error.response)
            }
        }
    }
    const selectClass = async (id) =>{
        try{
            clearMessages()
            isAddingFormVisible.value = false
            selectedClassId.value = id
            isClassFormVisible.value = true
            const classResponse = await api.getClassDetails(selectedClassId.value)
            Object.assign(selectedClass.value, classResponse.data)
            isEditFormVisible.value = true
            await getJournalsForClass(id)
            await getAllTeacherSubject()
        }
        catch(error){
            console.log(error.response)
        }       
    }
    const onClassDeleteClick = async() =>{
        try{
            clearMessages()
            await api.deleteClass(selectedClassId.value)
            const response = await api.getClassesOnPage(pageSize, selectedEducationalLevel.value, currentPage.value)
            assignClassesOnChange(response)
            classSuccessMessage.value = 'Класс успешно удалён!'
            isClassFormVisible.value = false
        }
        catch(error){
            if(error.response.status === 404){
                classErrorMessage.value = 'Выбранный класс не найден!'
            }
            else if(error.response.status === 409){
                if(error.response.data.Message === "There are students attached to the class."){
                    classErrorMessage.value = 'Класс не может быть удалён пока за ним закреплены ученики.'
                }
                else{
                    classErrorMessage.value = 'Класс не может быть удалён пока за ним закреплены журналы.'
                }
            }
            else{
                console.log(error.response)
            }
        }
    }
    const onAddClassClick = async () =>{
        try{
            clearMessages()
            await api.addClass(addDto.value)
            const response = await api.getClassesOnPage(pageSize, selectedEducationalLevel.value, currentPage.value)
            assignClassesOnChange(response)
            isClassFormVisible.value = false
            classSuccessMessage.value = 'Класс успешно добавлен!'
        }
        catch(error){
            if(error.status === 400){
                const modelStateErrors = error.response.data.errors
                for (const field in modelStateErrors){
                    if(errors.value.hasOwnProperty(field)){
                        errors.value[field] = modelStateErrors[field][0]
                    }
                }
            }  
            if(error.status === 409){
                classErrorMessage.value = 'Произошла ошибка при добавлении данных! Повторите попытку позже.'
            }
            else{
                console.log(error.response) 
            }  
        }
    }
    const onEditClassClick = async () =>{
        try{
            clearMessages()
            await api.updateClass(updateDto.value)
            const response = await api.getClassesOnPage(pageSize, selectedEducationalLevel.value,
                currentPage.value)
            assignClassesOnChange(response)
            isClassFormVisible.value = false    
            classSuccessMessage.value = 'Данные класса успешно обновлены!'
            await getJournalsForClass()
        }
        catch(error){
            if(error.status === 400){
                const modelStateErrors = error.response.data.errors
                for (const field in modelStateErrors){
                    if(errors.value.hasOwnProperty(field)){
                        errors.value[field] = modelStateErrors[field][0]
                    }
                }
            }   
            if(error.status === 409){
                classErrorMessage.value = 'Произошла ошибка при обновлении данных! Повторите попытку позже.'
            }
            console.log(error.response)
        }
    }
    const getJournalsForClass = async () =>{
        try{
            const response = await api.getJournalsForClass(selectedClassId.value)
            journals.value = response.data
        }
        catch(error){
            if(error.response.status === 404){
                journals.value = []
                journalsError.value = 'Для выбранного класса не найдено журналов.'
            }
            else{
                console.log(error.response)
            }
        }        
    }
    const getAllTeacherSubject = async () =>{
        try{           
            const response = await api.getAllTeacherSubjects(selectedClass.value.educationalLevelId)
            allTeacherSubjects.value = response.data
        }
        catch(error){
            console.log(error)
        }
    }
    const onJournalAddClick = async () =>{
        try{
            clearMessages()
            let dto = {
                teacherSubjectId: selectedTeacherSubjectId.value,
                classId: selectedClassId.value
            }
            await api.addJournal(dto)
            await getJournalsForClass(selectedClassId.value)
        }
        catch(error){
            const responseData = error.response.data
            if(responseData.StatusCode === 409){
                if(responseData.ErrorCode === 'ENTITY_ALREADY_EXISTS_ERROR'){
                    journalsError.value = 'Журнал с такими параметрами уже существует! Выберите другой предмет.'
                }
                if(responseData.ErrorCode === 'ENTITY_ADDING_ERROR'){
                    journalsError.value = 'Ошибка при добавлении журнала! Повторите попытку позже.'
                }
            }
            console.log(error)
        }
    }
    const onJournalDeleteClick = async(journalId) =>{
        try{
            const response = await api.deleteJournal(journalId)
            await getJournalsForClass(selectedClassId.value)
        }
        catch(error){
            if(error.status === 404){
                journalsError.value = 'Ошибка. Выбранынй журнал не найден!'
            }
            if(error.status === 409){
                journalsError.value = 'Выбранный журнал уже используется учителем. Его удаление невозможно.'
            }
            console.log(error)
        }
    }
    function clearMessages(){
        for (const key in errors.value) {
            errors.value[key] = '';
        }
        classesSearchError.value = ''
        classSuccessMessage.value = ''
        classErrorMessage.value = ''
        journalsError.value = ''
    }
    function assignClassesOnChange(response){
        classes.value = response.data.items
        numberOfPages.value = response.data.numberOfPages
    }
    function onAddFormShowClick(){
        selectedClassId.value = null
        clearMessages()
        isClassFormVisible.value = true
        isAddingFormVisible.value = true
        isEditFormVisible.value = false
        selectedClass.value.title = ''
        selectedClass.value.year = null
    }
    function navigateToClassJournal(journalId){
        try{
            router.push(`/journal/${journalId}`)
        }
        catch(error){
            console.log(error)
        }
    }
</script>
