<template>
    <div class="grid grid-cols-1 mt-3 gap-3">
        <div class="flex justify-center">
            <p class="text-red-700 text-2xl ">Ошибка! {{ errorMessage }}</p>
        </div>
        <div class="flex justify-center" v-if="session.role === 'admin'">
            <router-link to="/adminHome" class="bg-gray-300 p-2 text-blue-950 rounded-sm mb-2 no-wrap">
                Вернуться на главную страницу администратора</router-link>
        </div>    
        <div class="flex justify-center" v-if="session.role === 'teacher'">
            <router-link to="/teacherHome" class="bg-gray-300 p-2 text-blue-950 rounded-sm">
                Вернуться на главную страницу учителя</router-link>
        </div> 
        <div class="flex justify-center" v-if="session.role === 'student'">
            <router-link to="/studentHome" class="bg-gray-300 p-2 text-blue-950 rounded-sm">
                Вернуться на главную страницу ученика</router-link>
        </div> 
        <div class="flex justify-center" v-if="!session.role">
            <router-link to="/" class="bg-gray-300 p-2 text-blue-950 rounded-sm">
                Вернуться на страницу авторизации</router-link>
        </div>
    </div>
</template>

<script setup>
    import { useGlobalErrorMessage } from '../stores/globalErrorMessage';
    import { ref } from 'vue';
    import { useUserStore } from '../stores/user';
    const session = useUserStore();
    const errorMessageStorage = useGlobalErrorMessage()
    const errorMessage = ref(errorMessageStorage.message)
</script>