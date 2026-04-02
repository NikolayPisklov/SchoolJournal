<script setup>
import {ref, inject, onMounted} from 'vue'
import { useUserStore } from './stores/user';
import { useRouter } from 'vue-router';
import { userStatusEnum } from './enums/userStatuses';

const session = useUserStore();
const api = inject('$api');
const router = useRouter();

const logoutFromSystem = async () =>{
  session.logout();
  const response = await api.logout();
  //console.log(response.data);
  router.push('/');
}
</script>

<template>
  <div class="grid grid-cols-[auto_1fr_auto] items-center bg-blue-950 p-5 gap-5">
    <div class="w-auto">
      <img src="../src/assets/Logo-light.svg" alt="Logo" class="w-30"/>
    </div>
    <div>
      <h1 class="text-gray-300 text-5xl">School Journal</h1>
    </div>
    <div v-if="!session.role">
      <router-link to="/" class="bg-gray-300 p-2 text-blue-950 rounded-sm">Главная</router-link>
      <router-link to="/login" class="bg-gray-300 p-2 text-blue-950 rounded-sm ms-2">Войти</router-link>
    </div>
    <div v-if="session.role === 'admin'">
      <router-link to="/adminHome" class="bg-gray-300 p-2 text-blue-950 rounded-sm mb-2 no-wrap">Страница администратора</router-link>
      <a @click="logoutFromSystem" style="cursor: pointer" class="bg-gray-300 p-2 text-blue-950 rounded-sm ms-2">Выйти</a>
    </div>
    <div v-if="session.role === 'teacher'">
      <router-link to="/teacherHome" class="bg-gray-300 p-2 text-blue-950 rounded-sm">
        Журналы учителя</router-link>
      <a @click="logoutFromSystem" style="cursor: pointer" class="bg-gray-300 p-2 text-blue-950 rounded-sm ms-2">Выйти</a>
    </div>
    <div v-if="session.role === 'student'">
      <router-link to="/studentHome" class="bg-gray-300 p-2 text-blue-950 rounded-sm">
        Журналы ученика</router-link>
      <a @click="logoutFromSystem" style="cursor: pointer" class="bg-gray-300 p-2 text-blue-950 rounded-sm ms-2">Выйти</a>
    </div>
  </div>
  <router-view/>
</template>

<style>
.no-wrap{
  white-space: nowrap;
}
</style>
