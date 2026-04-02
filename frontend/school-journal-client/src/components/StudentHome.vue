<template>
<div class="flex justify-center mt-3 mb-3 text-2xl">
    <h1>Список ваших журналов</h1>
</div>
<div>
    <div v-for="j in journals" class="m-5 mt-0 text-lg">
        <div class="border-b border-gray-300 mb-2 p-1">
            {{ j.subjectTitle }}, {{ calculateClassYear(j.classYear) }}-{{ j.classTitle }},
             {{ j.teacherLastName }} {{ j.teacherFirstName[0] }}. {{ j.teacherMiddleName[0] }}.
            <button class="btn-gray p-0.5 ps-1 pe-1 ms-3" @click="onJournalSelectClick(j.id)">
                Посмотреть успеваемость по предмету
            </button>
        </div>
    </div>
</div>
</template>
<script setup>
import { inject, onMounted, ref } from 'vue';
import { useUserStore } from '../stores/user';
import { useRouter } from 'vue-router';

const session = useUserStore();
const api = inject('$api')
const journals = ref([])
const router = useRouter()

onMounted(async () =>{
    try{
        const journalsResponse = await api.getJournalsForStudent(session.user.id)
        journals.value  =journalsResponse.data
    }
    catch(error){
        console.log(error.data)
    }
})
const onJournalSelectClick = async(journalId) =>{
    try{
        router.push(`/studentJournal/${journalId}`)
    }
    catch(error){
        console.log(error)
    }
}
function calculateClassYear(classYear){
        return new Date().getFullYear() - classYear
}
</script>