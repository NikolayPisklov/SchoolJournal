import axios from "axios"

export const authApiClient = axios.create({
    baseURL: `https://localhost:7117/api/Auth/`,
    withCredentials: true
});

export async function logout(){
    try{
        const response = await authApiClient.post('logout');
        return response;
    }
    catch(error){
        throw(error);
    }
}
export async function getLoggedInUserDetails(){
    try{
        const response = await authApiClient.get('get-logged-in-user-details');
        return response;
    }
    catch(error){
        throw(error);
    }
}
export async function refreshTokens(){
    try{
        const response = await authApiClient.post('refresh-tokens');
        return response;
    }
    catch(error){
        throw(error);
    }
}
export async function signIn(login, password){
    try{
        const data = {
            login: login,
            password: password
        }
        const response = await authApiClient.post('login', data, {
            headers:{
                'Content-Type': 'application/json'
            }
        })
        return response;
    }
    catch(error){
        throw(error);
    }       
}
