import * as authApiClient from '../clients/auth'
import { attachInterceptors } from '../clients/interceptor'
import * as schoolJournalApiClient from '../clients/schoolJournal'

export default {
    install(app){
        attachInterceptors(authApiClient.authApiClient)
        attachInterceptors(schoolJournalApiClient.schoolJournalClient)

        const api = {
            ... authApiClient,
            ... schoolJournalApiClient
        }

        app.config.globalProperties.$api = api
        app.provide('$api', api)
    }
}