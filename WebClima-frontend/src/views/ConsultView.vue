<script setup lang="ts">
import { ref } from 'vue';
import { useWeatherApi } from '@/composables/useWeatherApi';
import ConsultaToggle from '@/components/ConsultaToggle.vue';
import CardClima from '@/components/CardClima.vue';
import type { ClimaResponseDTO } from '@/interfaces/ClimaResponseDTO';
import { useToast } from 'primevue/usetoast';

const { buscarClimaPorCidade, buscarClimaPorCoordenadas } = useWeatherApi();
const climaAtual = ref<ClimaResponseDTO | null>(null);
const loading = ref(false);
const toast = useToast();

const handleBuscarPorCidade = async (nome: string) => {
  loading.value = true;
  let summary = '';
  let detail = '';

  try {
    // Remove espaços extras
    const nomeTrimado = nome?.trim();

    // Se nome for vazio
    if (!nomeTrimado) {
      summary = 'Campo obrigatório';
      detail = 'O nome da cidade deve ser informado.';
    }

    // Se nome possui caracteres especiais ou número
    else if (!/^[A-Za-zÀ-ÖØ-öø-ÿ\s]+$/.test(nomeTrimado)) {
      summary = 'Nome inválido';
      detail = 'O nome da cidade não pode conter números ou caracteres especiais.';
    }

    // Se nome for menor que 3 caracteres
    else if (nomeTrimado.length < 3) {
      summary = 'Nome muito curto';
      detail = 'O nome da cidade deve ter pelo menos 3 caracteres.';
    }

    // Se nome for maior que 100 caracteres
    else if (nomeTrimado.length > 100) {
      summary = 'Nome muito longo';
      detail = 'O nome da cidade deve ter no máximo 100 caracteres.';
    }

    // Se houve erro de validação
    if (summary) {
      toast.add({ 
        severity: 'warn', 
        summary, 
        detail, 
        life: 3000 
      });
      return;
    }

    // Se passou nas validações
    climaAtual.value = await buscarClimaPorCidade({ nomeCidade: nomeTrimado });

    toast.add({ 
      severity: 'success', 
      summary: 'Sucesso',
      detail: 'Clima consultado com sucesso.',
      life: 3000 
    });

  } catch (error: any) {

    toast.add({ 
      severity: 'error', 
      summary: 'Erro na consulta',
      detail: error?.message || 'Não foi possível buscar o clima.',
      life: 3000 
    });

  } finally {
    loading.value = false;
  }
};

const handleBuscarPorCoordenadas = async (latitude: number, longitude: number) => {
  loading.value = true;
  let summary = '';
  let detail = '';

  try {
    // Verifica se ambas foram informadas
    if (latitude === null || latitude === undefined ||
        longitude === null || longitude === undefined) {
      summary = 'Campos obrigatórios';
      detail = 'Latitude e longitude devem ser informadas.';
    }

    // Verifica se são números válidos
    else if (isNaN(latitude) || isNaN(longitude)) {
      summary = 'Valor inválido';
      detail = 'Latitude e longitude devem ser números válidos.';
    }

    // Validação da latitude (-90 até 90)
    else if (latitude < -90 || latitude > 90) {
      summary = 'Latitude inválida';
      detail = 'A latitude deve estar entre -90 e 90.';
    }

    // Validação da longitude (-180 até 180)
    else if (longitude < -180 || longitude > 180) {
      summary = 'Longitude inválida';
      detail = 'A longitude deve estar entre -180 e 180.';
    }

    // Se houve erro de validação
    if (summary) {
      toast.add({
        severity: 'warn',
        summary,
        detail,
        life: 3000
      });
      return;
    }

    // Se passou nas validações
    climaAtual.value = await buscarClimaPorCoordenadas({ latitude, longitude });

    toast.add({
      severity: 'success',
      summary: 'Sucesso',
      detail: 'Clima consultado com sucesso.',
      life: 3000
    });

  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: 'Erro na consulta',
      detail: error?.message || 'Não foi possível buscar o clima.',
      life: 3000
    });

  } finally {
    loading.value = false;
  }
};
</script>

<template>
  <div class="consult-view">
    <ConsultaToggle 
      contexto="consulta"
      @buscar-por-cidade="handleBuscarPorCidade"
      @buscar-por-coordenadas="handleBuscarPorCoordenadas"
    />

    <div v-if="loading" class="loading">
      <i class="pi pi-spin pi-spinner" style="font-size: 3rem;"></i>
      <p>Buscando informações climáticas...</p>
    </div>

    <CardClima v-else :clima="climaAtual" />
  </div>
</template>

<style scoped>
.loading {
  display: flex;
  flex-direction: column; /* Coloca o ícone em cima e o texto embaixo */
  align-items: center;    /* Centraliza horizontalmente */
  justify-content: center; /* Centraliza verticalmente */
  min-height: 200px;      /* Dá um espaço para ele não ficar "esmagado" */
  width: 100%;
  gap: 1.5rem;            /* Espaço entre o spinner e o texto */
}

.loading i {
  font-size: 3rem;
  color: var(--primary-color); /* Usa o seu Ciano #00bbd5 */
  /* Opcional: Adiciona um leve brilho neon que combina com o Dark Mode */
  filter: drop-shadow(0 0 10px var(--primary-low));
}

.loading p {
  color: var(--text-muted);
  font-weight: 500;
  letter-spacing: 1px;
  /* Animação simples de pulsação no texto */
  animation: pulse 1.5s infinite ease-in-out;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}
</style>