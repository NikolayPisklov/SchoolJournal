import { useGlobalErrorMessage } from "../stores/globalErrorMessage";
import router from "../router"
import { refreshTokens } from "./auth";
import { useUserStore } from "../stores/user";

export function attachInterceptors(client){
    client.interceptors.request.use(
      (config) => {
        return config;
      },
      (error) => Promise.reject(error)
    );

    client.interceptors.response.use(
        (response) => response,
        async (error) => {
            //Server is not responding
            if (!error.response){
                console.log(error)
                const errorMessageStorage = useGlobalErrorMessage();
                errorMessageStorage.globalErrorMessage = "Сервер не отвечает. Мы уже работаем над устранением причин."
                router.push({name: "ErrorPage"})
                return createFakeResponse(error)
            }
            //Server error
            if (error.response?.status >= 500){
                console.log(error)
                const errorMessageStorage = useGlobalErrorMessage();
                errorMessageStorage.globalErrorMessage = "Внутренняя ошибка сервера. Мы уже работаем над устранением причин."
                router.push({name: "ErrorPage"})
                return createFakeResponse(error)
            }
            if (error.response?.status === 401 && !error.config._retry) {
            error.config._retry = true
            try {
                await refreshTokens()
                console.log('REFRESH SUCCESSFULL')
                return client.request(error.config)
            } 
            catch (refreshError) {
                const userStore = useUserStore()
                userStore.$reset()
                console.error("Token refresh failed:", refreshError)
                router.push({name: "Login"})
            }
        }    
        return Promise.reject(error)
    });
}

function createFakeResponse(error){
    return {
        data: null,
        status: 0,
        statusText: 'Network Error',
        headers: {},
        config: error.config,
        handled: true 
    };
}