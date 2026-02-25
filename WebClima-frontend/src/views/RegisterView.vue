<template>
  <div class="register-container">
    <div class="register-card">
      <h1>Registrar Usuário</h1>
      
      <form @submit.prevent="handleRegister">
        <div class="form-group">
          <label for="nome">Nome de Usuário</label>
          <input
            id="nome"
            v-model="formData.nome"
            type="text"
            placeholder="Digite seu nome"
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

        <div class="form-group">
          <label for="confirmarSenha">Confirmar Senha</label>
          <input
            id="confirmarSenha"
            v-model="formData.confirmarSenha"
            type="password"
            placeholder="Confirme sua senha"
            required
          />
          <span v-if="errors.confirmarSenha" class="error">{{ errors.confirmarSenha }}</span>
        </div>

        <button type="submit" class="btn-register" :disabled="isLoading">
          {{ isLoading ? 'Registrando...' : 'Registrar' }}
        </button>
      </form>

      <p class="login-link">
        Já tem conta? <router-link to="/">Faça login aqui</router-link>
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useWeatherApi } from '@/composables/useWeatherApi';
import type { UserRequestDTO } from '@/interfaces/UserRequestDTO';

const router = useRouter();
const { registrarUsuario } = useWeatherApi();
const isLoading = ref(false);

const formData = reactive({
  nome: '',
  senha: '',
  confirmarSenha: ''
});

const errors = reactive({
  nome: '',
  senha: '',
  confirmarSenha: ''
});

const validateForm = (): boolean => {
  let isValid = true;
  
  // Limpar erros anteriores
  errors.nome = '';
  errors.senha = '';
  errors.confirmarSenha = '';

  // Validar nome
  if (!formData.nome.trim()) {
    errors.nome = 'Nome de usuário é obrigatório';
    isValid = false;
  } else if (formData.nome.length < 3) {
    errors.nome = 'Nome deve ter pelo menos 3 caracteres';
    isValid = false;
  }

  // Validar senha
  if (!formData.senha) {
    errors.senha = 'Senha é obrigatória';
    isValid = false;
  } else if (formData.senha.length < 6) {
    errors.senha = 'Senha deve ter pelo menos 6 caracteres';
    isValid = false;
  }

  // Validar confirmação de senha
  if (!formData.confirmarSenha) {
    errors.confirmarSenha = 'Confirmação de senha é obrigatória';
    isValid = false;
  } else if (formData.senha !== formData.confirmarSenha) {
    errors.confirmarSenha = 'As senhas não coincidem';
    isValid = false;
  }

  return isValid;
};

const handleRegister = async () => {
  if (!validateForm()) {
    return;
  }

  try {
    isLoading.value = true;

    const userData: UserRequestDTO = {
      nome: formData.nome.trim(),
      senha: formData.senha
    };

    await registrarUsuario(userData);

    // Limpar formulário após sucesso
    formData.nome = '';
    formData.senha = '';
    formData.confirmarSenha = '';

    // Redirecionar para login após 2 segundos
    setTimeout(() => {
      router.push('/');
    }, 2000);
  } catch (error) {
    console.error('Erro ao registrar:', error);
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>

/* Scoped CSS for RegisterView.vue */

/* ========================= */
/*    Register Container     */
/* ========================= */
.register-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: var(--bg-color);
}

/* ========================= */
/*      Register Card        */
/* ========================= */
.register-card {
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

.register-card h1 {
  color: var(--text-main);
  font-size: 1.5rem;
  margin-bottom: 16px;
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
/*      Register Button      */
/* ========================= */
.btn-register {
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

.btn-register:hover {
  background-color: var(--primary-hover);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.btn-register:disabled {
  background-color: var(--border-color);
  cursor: not-allowed;
}

/* ========================= */
/*       Login Link          */
/* ========================= */
.login-link {
  margin-top: 16px;
}

.login-link p {
  color: var(--text-muted);
  font-size: 0.875rem;
}

.login-link a {
  color: var(--primary);
  text-decoration: none;
  transition: color 0.3s ease;
}

.login-link a:hover {
  color: var(--primary-hover);
}
</style>
