<template>
    <div v-if="journalTitleData" class="flex justify-center mt-3 text-2xl">
        Успеваемость по предмету "{{ journalTitleData.subjectTitle }}"
    </div>
    <div class="flex justify-center mt-3 mb-3" @click="onStatisticWindowOpenClick"
        v-if="lessons.length > 0">
        <button class="cursor-pointer btn-gray">
            Просмотр статистики
        </button>
    </div>
    <div class="m-3 flex justify-center" v-if="lessons.length > 0">
        <table class="border w-full">
            <thead>
                <tr class="border bg-gray-200">
                    <th class="border w-[10%]">Дата урока</th>
                    <th class="border w-[10%]">Оценка</th>
                    <th class="border w-[15%]">Посещаемость</th>
                    <th class="border w-[25%]">Тема</th>
                    <th class="border w-[40%]">Д/З</th>
                </tr>                
            </thead>
            <tbody>
                <tr v-for="l in lessons">
                    <td class="border p-1">
                        <div class="flex justify-center">
                            {{ new Date(l.lessonDate).toLocaleDateString(
                                    'ru-RU', {
                                    day: '2-digit',
                                    month: '2-digit',
                                    year: 'numeric', }
                                )
                            }}
                        </div>                        
                    </td>
                    <td class="border">
                        <div class="flex justify-center" v-if="progressMap[l.id] 
                            && progressMap[l.id].markValue !== null">
                            {{ progressMap[l.id].markValue }}
                        </div>
                        <div v-else class="flex justify-center text-gray-400 ms-2 me-2">
                            -
                        </div>
                    </td>
                    <td class="border">
                        <div class="flex justify-center" v-if="progressMap[l.id]">
                            {{progressMap[l.id].attendanceValue }}
                        </div>
                        <div v-else class="flex justify-center text-gray-400 ms-2 me-2">
                            -
                        </div>
                    </td>
                    <td class="border">
                        <div class="ms-2 me-2">
                            <p v-if="l.theme">
                                {{ l.theme }}
                            </p>
                            <p v-else class="text-gray-400">
                                Тема ещё не назначена
                            </p>
                        </div>          
                    </td>
                    <td class="border">
                        <div class="ms-2 me-2">
                            <p v-if="l.homework">
                                {{ l.homework }}
                            </p>
                            <p v-else class="text-gray-400">
                                Домашнее задание ещё не назначено
                            </p>
                            
                        </div>                       
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <div class="flex justify-center" v-else>
        <p class="text-gray-400 text-xl">Уроков по этому предмету пока нет.</p>
    </div>
    <Transition
        enter-active-class="transition duration-200 ease-out"
        enter-from-class="opacity-0"
        enter-to-class="opacity-100"
        leave-active-class="transition duration-200 ease-in"
        leave-from-class="opacity-100"
        leave-to-class="opacity-0"
    >
        <div v-if="isStatisticWindowOpen" class="fixed inset-0 flex items-center justify-center bg-black/70">
            <div class="bg-white w-[70%] rounded-sm justify-center p-3">
                <div class="flex justify-center text-xl">
                    <p>Ваша статистика успеваемости по предмету "{{ journalTitleData.subjectTitle }}"</p>
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
    import { onMounted, inject, ref, computed } from 'vue';
    import { useUserStore } from '../stores/user';
    import { useRoute } from 'vue-router';
    import MyChart from '../components/Chart.vue'

    const route = useRoute()
    const api = inject('$api')
    const session = useUserStore()

    const journalId = route.params.id
    const journalTitleData = ref(null)

    const lessons = ref([])
    const progresses = ref([])

    const progressMap = computed(() => {
        const map = {}
        progresses.value.forEach(p => {
            map[p.lessonId] = p
        })
        return map
    })

    const chartData = ref({
        labels: null, 
        datasets: null 
    })
    const chartOptions = {
        responsive: true
    }

    const isStatisticWindowOpen = ref(false)

    onMounted(async()=>{
        const titleResponse = await api.getJournalTitle(journalId)
        journalTitleData.value = titleResponse.data
        const detailsResponse = await api.getJournalDetailsForStudent(journalId, session.user.id)
        lessons.value = detailsResponse.data.lessons
        progresses.value = detailsResponse.data.progresses
    })
    const onStatisticWindowOpenClick = async() =>{
        try{
            const response = await api.getStudentStatistic(session.user.id, journalId)
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
            isStatisticWindowOpen.value = true
        }
        catch(error){
            console.log(error)
        }        
    }
    function onStatisticWindowCloseClick(){
        isStatisticWindowOpen.value = false
    }
</script>