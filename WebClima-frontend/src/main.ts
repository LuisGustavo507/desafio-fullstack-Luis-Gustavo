import { createApp } from 'vue';
import App from './App.vue';
import router from './router/router.ts';

// PrimeVue
import PrimeVue from 'primevue/config';
import ToastService from 'primevue/toastservice';
import Button from 'primevue/button';


// CSS do PrimeVue
import 'primevue/resources/themes/lara-dark-blue/theme.css';
import 'primevue/resources/primevue.min.css';
import 'primeicons/primeicons.css';

// CSS Global
import './assets/main.css';

const app = createApp(App);

app.use(router);
app.use(PrimeVue);
app.use(ToastService);
app.component('Button', Button);

app.mount('#app');