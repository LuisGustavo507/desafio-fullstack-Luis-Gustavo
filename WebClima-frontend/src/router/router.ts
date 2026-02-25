import { createRouter, createWebHistory } from 'vue-router';
import HistoryView from '../views/HistoryView.vue';
import HomeView from '../views/ConsultView.vue';
import RegisterView from '../views/RegisterView.vue';
import LoginView from '../views/LoginView.vue';
import AppLayout from '../layouts/AppLayout.vue';
import AuthLayout from '../layouts/AuthLayout.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/login'
    },
    {
      path: '/auth',
      component: AuthLayout,
      children: [
        {
          path: '',
          redirect: '/login'
        },
        {
          path: '/login',
          name: 'login',
          component: LoginView
        },
        {
          path: '/registrar',
          name: 'registrar',
          component: RegisterView
        }
      ]
    },
    {
      path: '/app',
      meta: { requiresAuth: true },
      component: AppLayout,
      children: [
        {
          path: '',
          redirect: '/consulta',
          meta: { requiresAuth: true }
        },
        {
          path: '/consulta',
          name: 'consulta',
          meta: { requiresAuth: true },
          component: HomeView
        },
        {
          path: '/historico',
          name: 'historico',
          meta: { requiresAuth: true },
          component: HistoryView
        }
      ]
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/login'
    }
  ]
});

// Guard de navegação para proteger rotas autenticadas
router.beforeEach((to, from, next) => {
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth);
  const token = localStorage.getItem('token');

  if (requiresAuth && !token) {
    // Rota requer autenticação e não possui token
    next('/login');
  } else if ((to.path === '/login' || to.path === '/registrar') && token) {
    // Se o usuário está logado e tenta acessar login/registrar, redireciona para consulta
    next('/consulta');
  } else {
    next();
  }
});

export default router;