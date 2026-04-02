<template>
    <div  class="mt-3">
        <div class="flex justify-center">
            <h1 class="text-2xl">Меню администратора</h1>
        </div>
        <div class="grid grid-cols-1 gap-2 mt-3">
            <div class="flex justify-center">
                <router-link to="/adminUsers" style="cursor: pointer"
                    class="mx-auto block items-center justify-center btn-gray">Редактирование пользователей</router-link>
            </div>
            <div class="flex justify-center">
                <router-link to="/adminClasses" style="cursor: pointer"
                    class="mx-auto block items-center justify-center btn-gray">Редактирование классов и их журналов</router-link>
            </div>
        </div>
    </div>
</template>