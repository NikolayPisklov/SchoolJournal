import { createRouter, createWebHistory } from "vue-router"
import Home from '../components/Home.vue'
import LogIn from '../components/LogIn.vue'
import AdminHome from "../components/AdminHome.vue"
import StudentHome from "../components/StudentHome.vue"
import TeacherHome from "../components/TeacherHome.vue"
import Unauth from "../components/Unauth.vue"
import AdminUsers from "../components/AdminUsers.vue"
import ErrorPage from "../components/ErrorPage.vue"
import { useUserStore } from "../stores/user"
import AdminClasses from "../components/AdminClasses.vue"
import Journal from "../components/Journal.vue"
import StudentJournal from "../components/StudentJournal.vue"

const routes = [
    {
        path: '/',
        name: 'Home',
        component: Home
    },
    {
        path: '/login',
        name: 'Login',
        component: LogIn
    },
    {
        path: '/unauthorized',
        name: 'unauthorized',
        component: Unauth
    },
    {
      path: '/errorPage',
      name: 'ErrorPage',
      component: ErrorPage
    },
    {
        path: '/adminHome',
        name: 'AdminHome',
        component: AdminHome,
        meta:{
            requiresAuth: true,
            roles: ['admin']
        }
    },
    {
      path: '/adminUsers',
      name: 'AdminUsers',
      component: AdminUsers,
      meta:{
        requiresAuth: true,
        roles: ['admin']
      }
    },
    {
      path: '/adminClasses',
      name: 'AdminClasses',
      component: AdminClasses,
      meta:{
        requiresAuth: true,
        roles: ['admin']
      }
    },
    {
        path: '/teacherHome',
        name: 'TeacherHome',
        component: TeacherHome,
        meta:{
            requiresAuth: true,
            roles: ['teacher']
        }
    },
    {
        path: '/studentHome',
        name: 'StudentHome',
        component: StudentHome,
        meta:{
            requiresAuth: true,
            roles: ['student']
        }
    },
    {
      path:'/journal/:id',
      name: 'Journal',
      component: Journal,
      meta:{
        requiresAuth: true
      }
    },
    {
      path: '/studentJournal/:id',
      name: 'StudentJournal',
      component: StudentJournal,
      meta:{
        requiresAuth: true
      }
    }
] 

const router = createRouter({
    history: createWebHistory(),
    routes
});

// Глобальная проверка перед каждым переходом
router.beforeEach((to, from, next) => {
  const userStore = useUserStore()
  
  if(to.path === '/'){
    if(!userStore.user){
      return next('/login')
    }
    switch(userStore.user.role){
      case 'admin':
        return next('/adminHome')
      case 'teacher':
        return next('/teacherHome')
      case 'student':
        return next('/studentHome')
      default:
        return next('/')
    }
  }
  // Проверяем, требует ли маршрут авторизации
  if (to.meta.requiresAuth) {
    // Если пользователь не авторизован
    if (!userStore.user) {
      return next('/unauthorized')
    }
    // Проверяем роли, если они указаны для маршрута
    const routeRoles = to.meta.roles
    if (routeRoles && routeRoles.length > 0) {
      const userRole = userStore.role
      // '*' означает любую авторизованную роль
      if (routeRoles.includes('*')) {
        return next()
      }
      // Проверяем есть ли у пользователя нужная роль
      if (!routeRoles.includes(userRole)) {
        return next('/unauthorized')
      }
    }
  }
  next()
})

export default router