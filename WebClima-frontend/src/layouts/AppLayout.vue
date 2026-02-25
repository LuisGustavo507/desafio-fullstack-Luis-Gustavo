<script setup lang="ts">
import { useRouter } from 'vue-router';
import Toast from 'primevue/toast';
import Button from 'primevue/button';

const router = useRouter();

const handleLogout = () => {
  localStorage.removeItem('token');
  router.push('/login');
};
</script>

<template>
  <div id="app-layout">
    <Toast position="top-right" />
    <header class="app-header">
      <div class="header-content">
        <h1 class="logo">
          <i class="pi pi-globe"></i>
          Web Clima API
        </h1>
        
        <nav class="nav-buttons">
          <Button 
            label="Consultar Clima" 
            class="buttonHeader"
            icon="pi pi-search"
            @click="router.push('/consulta')"
            :severity="$route.path === '/consulta' ? 'success' : 'secondary'"
            :outlined="$route.path !== '/consulta'"
          />
          <Button 
            label="Consultar Histórico" 
            class="buttonHeader"
            icon="pi pi-chart-line"
            @click="router.push('/historico')"
            :severity="$route.path === '/historico' ? 'success' : 'secondary'"
            :outlined="$route.path !== '/historico'"
          />
          <Button 
            label="Sair" 
            class="buttonHeader"
            icon="pi pi-sign-out"
            @click="handleLogout"
            severity="danger"
            outlined
          />
        </nav>
      </div>
    </header>

    <main class="app-main">
      <RouterView />
    </main>
  </div>
</template>

<style scoped>
.app-header {
  background-color: var(--surface-color);
  border-bottom: 1px solid var(--border-color);
  padding: 16px 24px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  max-width: 1200px;
  margin: 0 auto;
}

.logo {
  display: flex;
  align-items: center;
  color: var(--text-main);
  font-size: 2.5rem;
  font-weight: bold;
}

.logo i {
  margin-right: 8px;
  font-size: 2.5rem;
}

.nav-buttons {
  display: flex;
  gap: 16px;
}

.buttonHeader {
  font-size: 1rem;
  font-weight: 600;
  border-radius: var(--radius, 4px);
  transition: background-color 0.3s ease, box-shadow 0.3s ease;
}

.buttonHeader:hover {
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.app-main {
  padding: 24px;
  background-color: var(--bg-color);
  min-height: calc(100vh - 80px);
}
</style>
