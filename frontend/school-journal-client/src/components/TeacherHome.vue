<template>
<div class="flex justify-center mt-3 text-2xl">
    <h1>Список ваших журналов</h1>
</div>
<div>
    <div v-for="classGroup in journalsGroupedByClass" class="m-5 mt-0 text-lg">
        <div class="border-b border-gray-300 mb-2">
            Журналы класса {{ calculateClassYear(classGroup.classYear) }}-{{ classGroup.classTitle }}
        </div>
        <div v-for="journal in classGroup.journalsOfClass" class="mt-1 ps-3 grid grid-cols-8">
            <div class="col-span-2">
                {{ journal.subjectTitle }} | {{ journal.journalYear }} г.
            </div>
            <div class="col-span-6">
                <button class="btn-primary ms-2" style="cursor: pointer;"
                    @click="onJournalViewClick(journal.id)">
                    Просмотр журнала
                </button>
            </div>           
        </div>
    </div>
</div>
</template>
<script setup>
    import { ref, onMounted, inject } from 'vue';
    import { useUserStore } from '../stores/user';
    import { useRouter } from 'vue-router';

    const api = inject('$api')
    const session = useUserStore()
    const router = useRouter()

    const journalsGroupedByClass = ref([])

    onMounted(async () =>{
        try{
            const teacherId = session.user.id
            const response = await api.getJournalsForTeacher(teacherId)
            journalsGroupedByClass.value = response.data
        }
        catch(error){
            console.log(error)
        }
    })
    function calculateClassYear(classYear){
        return new Date().getFullYear() - classYear
    }
    function onJournalViewClick(journalId){
        try{
            router.push(`/journal/${journalId}`)
        }
        catch(error){
            console.log(error)
        }
    }
</script>