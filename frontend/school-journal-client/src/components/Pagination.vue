<template>
    <div class="pagination">
        <button @click="goToPreviousPage" :disabled="currentPage === 1"
            type="submit" style="cursor: pointer"
            class="rounded-sm px-0.5 py-0.5 bg-gray-300
            text-black font-medium hover:bg-indigo-650 active:scale-[.99] 
            focus:outline-none focus-visible:ring-2 focus-visible:ring-indigo-300
            transition me-1">
            Назад
        </button>

        <span v-for="(page, index) in pages" :key="index">
            <button
                v-if="page !== '...'"
                :class="{ active: page === currentPage }"
                style="cursor: pointer;"
                @click="$emit('change', page)"
            >
                {{ page }}
            </button>

            <span v-else class="dots">…</span>

            <span v-if="index < pages.length - 1">, </span>
        </span>

        <button @click="goToNextPage" :disabled="currentPage === numberOfPages"
        type="submit" style="cursor: pointer"
            class="rounded-sm px-0.5 py-0.5 bg-gray-300
            text-black font-medium hover:bg-indigo-650 active:scale-[.99] 
            focus:outline-none focus-visible:ring-2 focus-visible:ring-indigo-300
            transition ms-1">
            Вперёд
        </button>
    </div>
</template>

<script setup>
    import { computed } from 'vue';

    const props = defineProps({
        currentPage: Number,
        numberOfPages: Number
    })

    const emit = defineEmits(['change'])

    const pages = computed(() => {
        const total = props.numberOfPages
        const current = props.currentPage

        if(total <= 9){
            return Array.from({length: total}, (_, i) => i + 1)
        }

        if(current <= 3){
            return [1, 2, 3, ' ... ', total]
        }
        if(current >= total - 2){
            return [1, ' ... ', total - 2, total - 1, total]
        }
        return[
            1, 
            ' ... ',
            current - 1,
            current,
            current + 1,
            ' ... ',
            total
        ]
    })

    function goToPreviousPage() {
        if (props.currentPage > 1) {
            emit('change', props.currentPage - 1)
        }
    }

    function goToNextPage(){
        if (props.currentPage < props.numberOfPages) {
            emit('change', props.currentPage + 1)
        }
    }
</script>