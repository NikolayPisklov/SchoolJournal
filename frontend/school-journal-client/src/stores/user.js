    import {defineStore} from 'pinia'

    export const useUserStore = defineStore('user', {
        state: () => ({user: null}),
        getters: {
            id: (state) => state.user?.id,
            role: (state) => state.user?.role,
            email: (state) => state.user?.email,
            firstName: (state) => state.user?.firstName,
            lastName: (state) => state.user?.lastName,
            middleName: (state) => state.user?.middleName
        },
        actions:{
            setUser(userData){
                this.user = userData;
            },
            logout(){
                this.user = null;               
                localStorage.removeItem('user');
            }
        },
        persist: true
    });