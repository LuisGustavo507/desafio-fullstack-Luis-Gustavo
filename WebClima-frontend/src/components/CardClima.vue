<script setup lang="ts">
import { computed } from 'vue';
import Card from 'primevue/card';
import Divider from 'primevue/divider';
import Chip from 'primevue/chip';
import type { ClimaResponseDTO } from '@/interfaces/ClimaResponseDTO';

interface Props {
  clima: ClimaResponseDTO | null;
}

const props = defineProps<Props>();

const dataFormatada = computed(() => {
  if (!props.clima) return '';
  return new Date(props.clima.dataHora).toLocaleDateString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
});

</script>

<template>
  <Card v-if="clima" class="clima-card">
    <template #header>
      <div class="card-header">
        <div class="location-info">
          <h2>
            <i class="pi pi-map-marker"></i>
            {{ clima.cidade.nome }}
          </h2>
          <Chip :label="clima.cidade.pais" icon="pi pi-flag" class="country-chip" />
        </div>
      </div>
    </template>
    <template #content>
      <div class="clima-content">
        <div class="temperatura-principal">
          <div class="temp-value">
            <span class="temp-atual">{{ clima.temperatura }}°C</span>
            <span class="condicao">{{ clima.condicao }}</span>
          </div>
        </div>

        <Divider />

        <div class="temp-range">
          <div class="temp-item">
            <i class="pi pi-arrow-down" style="color: #00bbd5;"></i>
            <span class="label">Mínima</span>
            <span class="value">{{ clima.temperaturaMin }}°C</span>
          </div>
          <div class="temp-item">
            <i class="pi pi-arrow-up" style="color: #ff8c42;"></i>
            <span class="label">Máxima</span>
            <span class="value">{{ clima.temperaturaMax }}°C</span>
          </div>
        </div>

        <Divider />

        <div class="coordenadas-info">
          <div class="info-item">
            <i class="pi pi-compass"></i>
            <div>
              <span class="label">Coordenadas</span>
              <span class="value">{{ clima.latitude }}, {{ clima.longitude }}</span>
            </div>
          </div>
          <div class="info-item">
            <i class="pi pi-calendar"></i>
            <div>
              <span class="label">Data do Registro</span>
              <span class="value">{{ dataFormatada }}</span>
            </div>
          </div>
        </div>
      </div>
    </template>
  </Card>
</template>

<style scoped>
.clima-card {
  background-color: var(--surface-color);
  border: 1px solid var(--border-color);
  border-radius: var(--radius, 12px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.18);
  padding: 24px;
  max-width: 1200px;
  width: 100%;
  margin: 0 auto;
  margin-top: 12px;
  display: flex;
  flex-direction: column;
}

.location-info {
  display: flex;
  align-items: center;
  gap: 16px;
  flex-wrap: wrap;
}

.location-info h2 {
  color: var(--text-main);
  font-size: 2.25rem;
  font-weight: bold;
  display: flex;
  align-items: center;
  gap: 8px;
}

.location-info h2 i {
  color: var(--primary);
  font-size: 2.5rem;
}

.country-chip {
  background-color: var(--bg-surface);
  border: 1px solid var(--primary);
  color: var(--primary);
  font-weight: 600;
  border-radius: var(--radius, 16px);
  padding: 4px 12px;
  display: flex;
  font-size: 1.5rem;
  align-items: center;
  gap: 6px;
}

.country-chip .pi {
  color: var(--primary);
}

.temperatura-principal {
  display: flex;
  justify-content: center;
  margin-bottom: 8px;
}

.temp-value {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
}

.temp-atual {
  font-size: 3.5rem;
  font-weight: bold;
  color: var(--primary);
  letter-spacing: -1px;
}

.condicao {
  color: var(--text-muted);
  font-size: 2rem;
  text-transform: uppercase;
  font-weight: 500;
  letter-spacing: 1px;
}

.p-divider {
  border-top: 1px solid var(--border-color) !important;
  margin: 30px 0 !important;
}
.temp-range {
  display: flex;
  gap: 24px;
  justify-content: center;
  flex-wrap: wrap;
}

.temp-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
  min-width: 100px;
}

.temp-item i {
  font-size: 2.25rem;
}

.temp-item .pi-arrow-down {
  color: var(--primary);
}

.temp-item .pi-arrow-up {
  color: #ff8c42;
}

.temp-item .label {
  font-size: 1rem;
  color: var(--text-muted);
  font-weight: 500;
}

.temp-item .value {
  font-size: 1.5rem;
  color: var(--text-main);
  font-weight: bold;
}

.coordenadas-info {
  display: flex;
  gap: 24px;
  justify-content: center;
  flex-wrap: wrap;
}

.info-item {
  display: flex;
  align-items: center;
  gap: 8px;
  min-width: 140px;
}

.info-item i {
  font-size: 1.25rem;
  color: var(--primary);
}

.info-item .label {
  font-size: 1.25rem;
  margin-right: 10px;
  color: var(--text-muted);
  font-weight: 500;
}

.info-item .value {
  font-size: 1.1rem;
  color: var(--text-main);
  font-weight: bold;
}

</style>