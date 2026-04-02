import { defineStore } from "pinia";

export const useGlobalErrorMessage = defineStore('globalErrorMessage', {
    state: () => ({globalErrorMessage: null}),
    getters:{
        message: (state) => state.globalErrorMessage
    },
    actions:{
        clearErrorMessage(){
            this.message = null
        }
    }
})