<template>
  <div class="login-container">
    <div class="login-card">
      <h1>Web Clima API</h1>
      <p class="subtitle">Faça login para continuar</p>
      
      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label for="nome">Nome de Usuário</label>
          <input
            id="nome"
            v-model="formData.nome"
            type="text"
            placeholder="Digite seu nome de usuário"
            required
          />
          <span v-if="errors.nome" class="error">{{ errors.nome }}</span>
        </div>

        <div class="form-group">
          <label for="senha">Senha</label>
          <input
            id="senha"
            v-model="formData.senha"
            type="password"
            placeholder="Digite sua senha"
            required
          />
          <span v-if="errors.senha" class="error">{{ errors.senha }}</span>
        </div>

        <button type="submit" class="btn-login" :disabled="isLoading">
          {{ isLoading ? 'Entrando...' : 'Entrar' }}
        </button>
      </form>

      <div class="login-footer">
        <p>Não tem conta? <router-link to="/registrar">Registre-se aqui</router-link></p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useWeatherApi } from '@/composables/useWeatherApi';
import type { UserRequestDTO } from '@/interfaces/UserRequestDTO';

const router = useRouter();
const { logarUsuario } = useWeatherApi();
const isLoading = ref(false);

const formData = reactive({
  nome: '',
  senha: ''
});

const errors = reactive({
  nome: '',
  senha: ''
});

const validateForm = (): boolean => {
  let isValid = true;
  errors.nome = '';
  errors.senha = '';

  if (!formData.nome.trim()) {
    errors.nome = 'Nome de usuário é obrigatório';
    isValid = false;
  }

  if (!formData.senha) {
    errors.senha = 'Senha é obrigatória';
    isValid = false;
  }

  return isValid;
};

const handleLogin = async () => {
  if (!validateForm()) {
    return;
  }

  try {
    isLoading.value = true;
    
    const userData: UserRequestDTO = {
      nome: formData.nome.trim(),
      senha: formData.senha
    };

    await logarUsuario(userData);
    
    // Limpar formulário
    formData.nome = '';
    formData.senha = '';

    // Redirecionar para consulta após sucesso
    setTimeout(() => {
      router.push('/consulta');
    }, 1500);
  } catch (error) {
    console.error('Erro ao fazer login:', error);
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>

/* Scoped CSS for LoginView.vue */

/* ========================= */
/*      Login Container      */
/* ========================= */
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: var(--bg-color);
}

/* ========================= */
/*        Login Card         */
/* ========================= */
.login-card {
  background-color: var(--surface-color);
  border: 1px solid var(--border-color);
  border-radius: var(--radius, 8px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
  padding: 24px;
  width: 100%;
  max-width: 400px;
  text-align: center;
  backdrop-filter: blur(10px);
}

.login-card h1 {
  color: var(--text-main);
  font-size: 1.5rem;
  margin-bottom: 8px;
}

.login-card .subtitle {
  color: var(--text-muted);
  font-size: 1rem;
  margin-bottom: 24px;
}

/* ========================= */
/*       Form Groups         */
/* ========================= */
.form-group {
  margin-bottom: 16px;
  text-align: left;
}

.form-group label {
  color: var(--text-muted);
  font-size: 0.875rem;
  margin-bottom: 8px;
  display: block;
}

.form-group input {
  width: 100%;
  padding: 10px;
  border: 1px solid var(--border-color);
  border-radius: var(--radius, 4px);
  background-color: var(--surface-color);
  color: var(--text-main);
  font-size: 1rem;
  transition: border-color 0.3s ease, box-shadow 0.3s ease;
}

.form-group input:focus {
  outline: none;
  border-color: var(--primary);
  box-shadow: 0 0 0 3px var(--primary-transparent);
}

.form-group .error {
  color: var(--primary);
  font-size: 0.875rem;
  margin-top: 4px;
}

/* ========================= */
/*       Login Button        */
/* ========================= */
.btn-login {
  width: 100%;
  padding: 12px;
  background-color: var(--primary);
  color: var(--text-main);
  border: none;
  border-radius: var(--radius, 4px);
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.3s ease, box-shadow 0.3s ease;
}

.btn-login:hover {
  background-color: var(--primary-hover);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.btn-login:disabled {
  background-color: var(--border-color);
  cursor: not-allowed;
}

/* ========================= */
/*       Login Footer        */
/* ========================= */
.login-footer {
  margin-top: 16px;
}

.login-footer p {
  color: var(--text-muted);
  font-size: 0.875rem;
}

.login-footer a {
  color: var(--primary);
  text-decoration: none;
  transition: color 0.3s ease;
}

.login-footer a:hover {
  color: var(--primary-hover);
}
</style>
