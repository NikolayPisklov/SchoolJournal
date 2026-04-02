<template>
    <div class="grid grid-cols-3 mt-3">
        <div class="flex justify-center col-span-3">
            <h3 class="text-3xl">Введите логин и пароль</h3><br/>              
        </div>
        <div class="flex justify-center col-span-3">
            <h5 class="text-xl text-red-700">{{errorMessage}}</h5> 
        </div>
        <div class="flex justify-center col-span-3 text-xl mt-3">
            <form @submit.prevent="submitForm" class="w-full max-w-lg min-w-min">
                <label for="login" class="block font-medium text-gray-700">Логин</label>
                <input id="login" v-model="login" name="login" type="text" required autocomplete="off"
                        class="mt-1 mb-6 block w-full text-box"/>

                <label for="password" class="block font-medium text-gray-700">Пароль</label>
                <input id="password" v-model="password" name="password" type="password" required
                        class="mt-1 mb-6 block w-full text-box"/>

                <button type="submit" @click="logInTheAccaunt" style="cursor: pointer"
                        class="mx-auto block items-center justify-center btn-primary" aria-label="Войти">
                    Войти
                </button>
            </form>
        </div>  
    </div>
</template>

<script setup>
    import {ref, inject} from 'vue'
    import { useRouter } from 'vue-router'
    import {useUserStore} from '../stores/user'

    const router = useRouter();
    const api = inject('$api');
    const login = ref('');
    const password = ref('');
    const errorMessage = ref('');
    const session = useUserStore();

    const logInTheAccaunt = async () =>{
        try{
            const response = await api.signIn(login.value, password.value);
            if(response.data){
                const userDataResponse = await api.getLoggedInUserDetails();
                session.setUser(userDataResponse.data);
                redirectUserBasedOnRole(session);
            }                     
        }
        catch(error){
            if(error.response){
                console.log(error.response.data);
                errorMessage.value = error.response.data;
            }
            else{
                console.error('Неизвестная ошибка:', error)
                console.error('Текст ошибки:', error.message)
                console.error('Стек вызовов:', error.stack)
            }
        }                
    }
    const redirectUserBasedOnRole = (sessionData) => {
        if(sessionData.role === 'admin'){
            router.push('/adminHome');
        }
        else if(sessionData.role === 'teacher'){
            router.push('/teacherHome');
        }
        else{
            router.push('/studentHome');
        }
    }
</script>
