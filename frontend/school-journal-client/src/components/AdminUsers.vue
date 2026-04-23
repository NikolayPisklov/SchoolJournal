<template>
    <div class="m-3">
        <div class="flex justify-center">
            <h1 ref="focus" class="text-2xl">Редактирование пользователей</h1>
        </div>
        <div class="grid grid-cols-7 gap-3 mt-3">
            <div class="col-span-3">
                <div class="flex items-center gap-1">
                    <label class="whitespace-nowrap">Поиск по ФИО: </label>
                    <input id="searchField" name="searchField" type="text" autocomplete="off" v-model="search"
                            class="block w-full text-box"/>
                    <button type="submit" style="cursor: pointer" @click="onSearchButtonClick"
                            class="block btn-gray" aria-label="Найти">Найти</button>                    
                </div>
                <div class="m-2 ms-0">
                    <label for="roles">Фильтр по статусу:</label>
                    <select name="roles" id="roles" v-model="selectedRole" @change="onStatusDropdownChange"
                        class="text-box ms-1">
                        <option value="">Все пользователи</option>
                        <option :value="userStatusEnum.Admin">Администраторы</option>
                        <option :value="userStatusEnum.Teacher">Учителя</option>
                        <option :value="userStatusEnum.Student">Ученики</option>
                    </select>
                    <div class="flex">
                        <button type="submit" style="cursor: pointer" @click="onAddingUserButtonClick()"
                                class="mt-1 btn-gray block" aria-label="Добавить пользователя">
                            Добавить пользователя
                        </button>
                    </div>                    
                </div>

                <div v-if="noUsersFoundErrorMessage">
                    <span class="error">{{ noUsersFoundErrorMessage }}</span>
                </div>
                <ul class="user-list">
                    <li v-for="user in users" :key="user.id"
                        :class="{selected: user.id === selectedUserId}"
                        @click="selectUser(user.id)"
                    >
                        {{ user.lastName }} {{ user.firstName }} {{ user.middleName }} | {{ formAdditionalInfo(user) }}
                    </li>
                </ul>
                <Pagination
                    :current-page="currentPage"
                    :number-of-pages="numberOfPages"
                    @change="onPageChangeClick"
                />
               
            </div>
            <div v-if="successMessage" class="col-span-4 flex m-3">
                <span  class="text-emerald-700">{{ successMessage }}</span>
            </div>
            <div class="col-span-4 ps-3" v-if="isEditFormVisible || isAddFormVisible">
                <p v-if="isEditFormVisible">Форма редактирования пользователя {{ user.lastName }} 
                    {{ user.firstName }} {{ user.middleName }}</p>
                <p v-if="isAddFormVisible">Форма добавления пользователя</p>
                <span v-if="deleteErrorMessage" class="error">{{ deleteErrorMessage }}</span>   
                <form @submit.prevent="submitForm" class="w-full max-w-lg min-w-min mt-3">
                    

                    <div v-if="isAddFormVisible">
                        <label for="newUserRole" class="font-medium block">Статус пользователя</label>
                        <span v-if="errors.StatusId" class="error">{{ errors.StatusId }}</span>
                        <select name="newUserRole" id="newUserRole" v-model="user.statusId"
                            class="mt-1 mb-6 font-medium block text-box">   
                            <option value="" disabled>Выберите роль</option> 
                            <option :value="userStatusEnum.Admin">Администратор</option>
                            <option :value="userStatusEnum.Teacher">Учитель</option>
                            <option :value="userStatusEnum.Student">Ученик</option>
                        </select>
                    </div>
                    
                    <label for="login" class="block text-gray-700">Логин</label>
                    <span v-if="errors.Login" class="error">{{ errors.Login }}</span>
                    <input id="login" v-model="user.login" name="login" type="text" required autocomplete="off"
                            class="mt-1 mb-6 block w-full text-box"/>

                    <label for="lastName" class="block text-gray-700">Фамилия</label>
                    <span v-if="errors.LastName" class="error">{{ errors.LastName }}</span>
                    <input id="lastName" v-model="user.lastName" name="login" type="text" required autocomplete="off"
                            class="mt-1 mb-6 block w-full text-box"/>

                    <label for="name" class="block font-medium text-gray-700">Имя</label>
                    <span v-if="errors.FirstName" class="error">{{ errors.FirstName }}</span>
                    <input id="name" v-model="user.firstName" name="login" type="text" required autocomplete="off"
                            class="mt-1 mb-6 block w-full text-box"/>

                    <label for="middleName" class="block font-medium text-gray-700">Отчество</label>
                    <span v-if="errors.MiddleName" class="error">{{ errors.MiddleName }}</span>
                    <input id="middleName" v-model="user.middleName" name="login" type="text" required autocomplete="off"
                            class="mt-1 mb-6 block w-full text-box"/>

                    <label for="email" class="block font-medium text-gray-700">Электронная почта</label>
                    <span v-if="errors.Email" class="error">{{ errors.Email }}</span>
                    <input id="email" v-model="user.email" name="login" type="text" required autocomplete="off"
                            class="mt-1 mb-6 block w-full text-box"/>

                    <div class="flex" v-if="isEditFormVisible">
                        <input type="checkbox" id="isPasswordEditing" class="me-1" style="accent-color: blue;"
                            @change="onPasswordCheckboxChange"></input>
                        <label for="isPasswordEditing" class="block font-medium text-gray-700">Редактировать пароль?</label>           
                    </div>
                    <div v-if="isPasswordInputVisible">
                        <label for="password" class="block font-medium text-gray-700">Пароль</label>
                        <span v-if="errors.Password" class="error">{{ errors.Password }}</span>
                        <input id="password" v-model="user.password" name="password" type="password" required
                                class="mt-1 mb-6 block w-full text-box"/>
                    </div>
                    
                    <div class="flex mt-3">
                        <button v-if="isEditFormVisible" @click="onUpdateClick" type="submit" style="cursor: pointer"
                            class="btn-primary" aria-label="Применить">
                            Применить
                        </button>
                        <button v-if="isEditFormVisible" type="submit" style="cursor: pointer" @click="onUserDeleteClick"
                                class="ms-1 btn-red">
                                Удалить пользователя
                            </button>
                        <button v-if="isAddFormVisible" @click="onAddClick" type="submit" style="cursor: pointer"
                                class="mt-3 block btn-primary" aria-label="Добавить пользователя">
                            Добавить пользователя
                        </button>
                    </div>                    
                </form>
                <div v-if="selectedUserStatus === userStatusEnum.Student && !isAddFormVisible" class="mt-3">
                    <p>Форма редактирование класса ученика</p>
                    <form @submit.prevent="submitForm">
                        <div v-if="!classOfStudent">
                            <span clss="error">Ученику ещё не назначен класс. Выберете его ниже:</span>
                        </div>
                        <div v-if="classOfStudent">
                            <span clss="error">Текущий класс ученика {{formClassInfo()}}.
                                Вы можете изменить его ниже:</span>
                        </div>
                        <div v-if="classErrorMessage != ''">
                            <span class="error">{{ classErrorMessage }}</span>
                        </div>
                        <div>
                            <select name="roles" id="roles" v-model="newClassId"
                                class="text-box ms-1">
                                <option disabled value="">Выберете новый класс</option>
                                <option v-for="theClass in classes"
                                    :key="theClass"
                                    :value="theClass.id">
                                    {{ new Date().getFullYear() - theClass.year }}-{{ theClass.title }}
                                </option>
                            </select>
                        </div>
                        <div>
                            <button @click="onTransferStudentButtonClick" type="submit" style="cursor: pointer"
                                class="mt-3 mx-auto btn-primary" 
                                aria-label="Изменить класс">
                                Изменить класс
                            </button>
                        </div>
                    </form>
                </div>
                <div v-if="selectedUserStatus === userStatusEnum.Teacher && !isAddFormVisible" class="mt-3">
                    <p>Форма редактирование предметов учителя.<br/> Предметы, которые преподаёт учитель:</p>
                    <span class="error" v-if="teacherErrorMessage!==''">{{ teacherErrorMessage }}</span>                    
                    <form @submit.prevent="submitForm" class="">
                        <div>
                            <ul class="list list-disc ms-5" v-if="teacherSubjects">
                                <li v-for="ts in teacherSubjects" 
                                    :key="ts" :value="ts.id" class="mb-2">
                                {{ ts.subjectTitle }}
                                <span v-if="ts.subjectEducationalLevelId ===
                                    educationalLevels.Junior">(Начальные классы)</span>
                                <span v-if="ts.subjectEducationalLevelId === 
                                    educationalLevels.Middle">(Средные классы)</span>
                                <span v-if="ts.subjectEducationalLevelId === 
                                    educationalLevels.Senior">(Старшые классы)</span>
                                <button v-if="isEditFormVisible" type="submit" style="cursor: pointer" 
                                    @click="onSubjectDeleteClick(ts.id)" class="ms-1 btn-red p-0 ps-1 pe-1">
                                    Открепить от учителя
                                </button>
                                </li>
                            </ul>
                        </div>
                        <div>
                            <select name="roles" id="roles" v-model="selectedSubjectId"
                                class="text-box ms-1 mt-2">
                                <option disabled value="">Выберите новый предмет</option>
                                <option v-for="s in allSubjects"
                                    :key="s"
                                    :value="s.id">
                                {{s.title}}
                                <span v-if="s.educationalLevelId ===
                                    educationalLevels.Junior">(Начальные классы)</span>
                                <span v-if="s.educationalLevelId === 
                                    educationalLevels.Middle">(Средные классы)</span>
                                <span v-if="s.educationalLevelId === 
                                    educationalLevels.Senior">(Старшые классы)</span>
                                </option>
                            </select>
                        </div>
                        <div>
                            <button @click="onSubjectAttachingClick" type="submit" 
                                style="cursor: pointer"
                                class="mt-3 mx-auto btn-primary" 
                                aria-label="Закрепить класс">
                                Закрепить предмет
                            </button>
                        </div>                       
                    </form>                    
                </div>
            </div>
        </div>        
    </div>
</template>

<script setup>
    import {ref, inject, onMounted, nextTick, computed} from 'vue'
    import Pagination from './Pagination.vue'
    import { userStatusEnum } from '../enums/userStatuses'
    import { educationalLevels } from '../enums/educationalLevels'
    
    const api = inject('$api')
    const selectedRole = ref('')
    const selectedSubjectId = ref('')
    const focus = ref()
    const users = ref([])
    const classes = ref([])
    const teacherSubjects = ref([])
    const allSubjects = ref([])
    const selectedUserId = ref(null)
    const newClassId = ref(null)
    const search = ref('')

    const currentPage = ref(1)
    const numberOfPages = ref(0)
    const pageSize = 10

    const isEditFormVisible = ref(false)
    const isPasswordInputVisible = ref(false)
    const isAddFormVisible = ref(false)
    
    const classOfStudent = ref(null)
    const user = ref({
        id: null,
        statusId: '',
        login: '',
        email: '',
        firstName: '',
        lastName: '',
        middleName: '',
        password: null    
    })
    const selectedUserStatus = ref(null)

    const updateDto = computed(() => ({
        id: user.value.id,
        login: user.value.login,
        email: user.value.email,
        firstName: user.value.firstName,
        lastName: user.value.lastName,
        middleName: user.value.middleName,
        password: user.value.password
    }))
    const addDto = computed(() => ({
        statusId: user.value.statusId,
        login: user.value.login,
        firstName: user.value.firstName,
        lastName: user.value.lastName,
        middleName: user.value.middleName,
        email: user.value.email,
        password: user.value.password
    }))
    const transferStudentDto = ref({
        studentId: null, 
        newClassId: null, 
        oldClassId: null
    })

    const errors = ref({Login: '', Email: '', FirstName: '', LastName: '', 
        MiddleName: '', Password: '', StatusId: ''})
    const successMessage = ref('')
    const noUsersFoundErrorMessage = ref('')
    const deleteErrorMessage = ref('')
    const classErrorMessage = ref('')
    const teacherErrorMessage = ref('')

    function clearMessages(){
        selectedSubjectId.value = ''
        teacherErrorMessage.value = ''
        classErrorMessage.value = ''
        deleteErrorMessage.value = ''
        successMessage.value = ''
        noUsersFoundErrorMessage.value = ''
        for (const key in errors.value) {
            errors.value[key] = '';
        }
    }
    
    onMounted(async () => {
        try{
            const response = await api.getUsersOnPage(null, null, pageSize, 1)
            assignUsersOnChange(response)
            const classesListResponse = await api.getAllClasses()
            classes.value = classesListResponse.data
        }
        catch(error){
            console.log(error)
        }       
    })
    const onAddClick = async () => {
        try{
            clearMessages()
            if(addDto.value.statusId === ''){
                addDto.value.statusId = null
            }
            await api.addUser(addDto.value)
            successMessage.value = 'Пользователь успешно добавлен!'
            isAddFormVisible.value = false
            const response = await api.getUsersOnPage(selectedRole.value, search.value, pageSize, currentPage.value)
            assignUsersOnChange(response)
        }
        catch(error){
            userErrorHandler(error.response)
        }
    }
    const onUpdateClick = async () => {
        try{
            clearMessages()
            await api.updateUserDetails(updateDto.value)
            isEditFormVisible.value = false
            successMessage.value = 'Данные пользователя упешно обновлены!'
            const response = await api.getUsersOnPage(selectedRole.value, search.value, pageSize, currentPage.value)
            assignUsersOnChange(response)
        }
        catch(error){
            userErrorHandler(error.response)
        }
    }
    const onStatusDropdownChange = async () => {
        try{
            clearMessages()
            if(selectedRole.value === ''){
                const response = await api.getUsersOnPage(null, search.value, pageSize, 1)
                assignUsersOnChange(response)
                currentPage.value = 1
                return
            }
            const response = await api.getUsersOnPage(selectedRole.value, search.value, pageSize, 1)
            assignUsersOnChange(response)
            currentPage.value = 1
        }
        catch(error){
            if(error.response.status === 404){
                noUsersFoundHandler()
            }
            else{
                console.log(error.response)
            }
        }
    }
    const selectUser = async (id) => {
        try{
            clearValuesOnUserSelect()
            selectedUserId.value = id
            const response = await api.getUserDetails(id)
            Object.assign(user.value, response.data) 
            isEditFormVisible.value = true
            await nextTick()
            focus.value?.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            })
            await assignUserStatus(id)
            if(selectedUserStatus.value === userStatusEnum.Student){               
                const classResponse = await api.getClassOfStudent(id)
                classOfStudent.value = classResponse.data
            }  
            if(selectedUserStatus.value === userStatusEnum.Teacher){
                const allSubjectsResponse = await api.getAllSubjects()
                allSubjects.value = allSubjectsResponse.data
                const response = await api.getSubjectsForTeacher(selectedUserId.value)
                teacherSubjects.value = response.data
            }         
        }
        catch(error){
            if(error.response.status === 404){
                teacherErrorMessage.value = 'Преподаватель пока не преподаёт ни один предмет.'
                classErrorMessage.value = 'Ученику пока не назначен класс.'
            }
            else{
                console.log(error.response)
            }
        }       
    }
    const onSearchButtonClick = async() => {
        try{
            clearMessages()
            const response = await api.getUsersOnPage(selectedRole.value, search.value, pageSize, currentPage.value)
            assignUsersOnChange(response)
            currentPage.value = 1
        }
        catch(error){
            if(error.response.status === 404){
                noUsersFoundHandler()
            }
            else{
                console.log(error.response)
            }
        }
    }
    const onPageChangeClick = async(page) => {
        try{
            clearMessages()
            currentPage.value = page
            const response = await api.getUsersOnPage(selectedRole.value, search.value, pageSize, currentPage.value)
            assignUsersOnChange(response)
        }
        catch(error){
            if(error.response.status === 404){
                noUsersFoundHandler()
            }
            else{
                console.log(error.response)
            }
        }        
    }
    const assignUserStatus = async() =>{
        try{
            var response = await api.getUserStatusId(selectedUserId.value)
            selectedUserStatus.value = response.data
        }
        catch(error){
            console.log(error)
        }
    }
    const onUserDeleteClick = async() =>{
        try{
            var response = await api.deleteUser(selectedUserId.value)
            isEditFormVisible.value = false
            successMessage.value = 'Пользователь успешно удалён из системы.'
            response = await api.getUsersOnPage(selectedRole.value, search.value, pageSize, currentPage.value)
            assignUsersOnChange(response)
        }
        catch(error){
            if(error.response.status === 404){
                deleteErrorMessage.value = "Пользователь не найден!"
            }
            if(error.response.status === 409){
                deleteErrorMessage.value = "Пользователь связан с другими предметами или классами. Его удаление невозможно, пока существуют эти зависимочти."
            }
            else{
                console.log(error)
            }
        }
    }
    const onSubjectAttachingClick = async() => {
        try{
            const dto = {
                userId: selectedUserId.value,
                subjectId: selectedSubjectId.value
            }
            console.log(dto)
            const addingResponse = await api.addTeacherSubject(dto)
            const subjectsResponse = await api.getSubjectsForTeacher(selectedUserId.value)
            teacherSubjects.value = subjectsResponse.data
            clearMessages()
        }
        catch(error){
            if(error.response.status === 409){
                teacherErrorMessage.value = 'Пользователь не является учителем!'
            }
            else if(error.response.status === 400){
                teacherErrorMessage.value = 'Выберите предмет!'
            }
        }
    }
    const onSubjectDeleteClick = async(id) => {
        try{
            clearMessages() 
            const response = await api.deleteTeacherSubject(id)
            const subjectsResponse = await api.getSubjectsForTeacher(selectedUserId.value)
            teacherSubjects.value = subjectsResponse.data
        }
        catch(error){
            if(error.response.status === 404){
                teacherSubjects.value = null
                teacherErrorMessage.value = 'Преподаватель пока не преподаёт ни один предмет.'
            }
            else if(error.response.status === 409){
                teacherErrorMessage.value = 'Ошибка при удалении. \n Преподаватель закреплён за журналом по этому предмету!'
            }
        }
    }
    const onTransferStudentButtonClick = async() =>{
        try{
            clearMessages()
            transferStudentDto.value.studentId = selectedUserId.value 
            transferStudentDto.value.newClassId = newClassId.value 
            transferStudentDto.value.oldClassId = classOfStudent.value?.id ?? null
            await api.transferStudentToAnotherClass(transferStudentDto.value)
            const classResponse = await api.getClassOfStudent(selectedUserId.value)
            classOfStudent.value = classResponse.data
            const response = await api.getUsersOnPage(selectedRole.value, search.value, pageSize, currentPage.value)
            assignUsersOnChange(response)
        }
        catch(error){
            var repsonseData = error.response.data
            if(repsonseData.StatusCode === 409){
                if(repsonseData.Message === `Student can't be transfered to the same class!`){
                    classErrorMessage.value = 'Нельзя перевести ученика в тот же класс.'
                }
                else if(repsonseData.Message === 'Selected user is not a student!'){
                    classErrorMessage.value = 'Выбранный пользователь не является учеником'
                }
            }
            else if(error.status === 400){
                classErrorMessage.value = 'Выберите класс для перевода ученика!'
            }
        }
    }
    function formAdditionalInfo(user){
        switch(user.statusId){
            case userStatusEnum.Admin:
                return 'Администратор'
            case userStatusEnum.Teacher:
                return 'Учитель'
            case userStatusEnum.Student:
                if(user.classTitle === null){
                    return 'Ученик (класс не назначен)'
                }
                else{
                    var currentYear = new Date().getFullYear()
                    return 'Ученик, ' + `${currentYear - user.classYear}-${user.classTitle}`
                }
        }
    }
    function formClassInfo()
    {
        var currentYear = new Date().getFullYear()
        return `${currentYear - classOfStudent.value.year}-${classOfStudent.value.title}`
    }
    function onAddingUserButtonClick(){
        clearUserFields()
        selectedUserId.value = null
        isPasswordInputVisible.value = true
        isAddFormVisible.value = true
        isEditFormVisible.value = false
    }
    function assignUsersOnChange(response){
        users.value = response.data.items
        numberOfPages.value = response.data.numberOfPages
    }
    function noUsersFoundHandler(){
        noUsersFoundErrorMessage.value = 'Пользователь с предоставлеенными данными не найден!'
        users.value = null
        numberOfPages.value = 1
    }
    function clearUserFields(){
        user.value.id = null
        user.value.statusId = ''
        user.value.login = ''
        user.value.email = ''
        user.value.firstName = ''
        user.value.lastName = ''
        user.value.middleName = ''
        user.value.password = null 
    }
    function onPasswordCheckboxChange (){
        if(isPasswordInputVisible.value === false){
            isPasswordInputVisible.value = true
            return
        }
        isPasswordInputVisible.value = false
        user.value.password = null
    }
    function clearValuesOnUserSelect(){
        newClassId.value = ''
        classOfStudent.value = null
        allSubjects.value = null
        teacherSubjects.value = null
        clearMessages()
        isAddFormVisible.value = false
        isPasswordInputVisible.value = false
    }
    function userErrorHandler(response)
    {
        if(response.status === 400){
            const modelStateErrors = response.data.errors
            for (const field in modelStateErrors){
                if(errors.value.hasOwnProperty(field)){
                    errors.value[field] = modelStateErrors[field][0]
                }
            }
        }
        else if(response.status === 409){
            if(response.data.Message === 'User with the same login already exists!'){
                errors.value.Login = 'Пользователь с таким логином уже существует!'
            }
            else if(response.data.Message === 'User with the same email already exists!'){
                errors.value.Email = 'Пользователь с такой почтой уже существует!'
            }
            else{
                console.log(response)
            }
        }
        else{
            console.log(response)
        }
    }
</script>
