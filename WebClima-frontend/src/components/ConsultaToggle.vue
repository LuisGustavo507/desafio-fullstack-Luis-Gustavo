<script setup lang="ts">
import { ref, watch, computed } from 'vue';
import { useToast } from 'primevue/usetoast';
import Card from 'primevue/card';
import RadioButton from 'primevue/radiobutton';
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';

import InputNumber from 'primevue/inputnumber';
import { HistoricoRequestDTO } from '@/interfaces/HistoricoRequestDTO';

interface Props {
  contexto?: 'consulta' | 'historico';
}

const props = withDefaults(defineProps<Props>(), {
  contexto: 'consulta',
});

interface Emits {
  (e: 'buscarPorCidade', nomeCidade: string): void; 
  (e: 'buscarPorCoordenadas', latitude: number, longitude: number): void;
  (e: 'buscarPorHistorico', historicoResquest: HistoricoRequestDTO): void;
}

const emit = defineEmits<Emits>();
const toast = useToast();

const tipoBusca = ref<'cidade' | 'coordenadas'>('cidade');
const nomeCidade = ref('');
const latitude = ref<number | null>(null);
const longitude = ref<number | null>(null);

watch(tipoBusca, (novoValor) => {
  
  if (novoValor === 'cidade') {
    latitude.value = null;
    longitude.value = null;
  }

  if (novoValor === 'coordenadas') {
    nomeCidade.value = null;
  }
});

  

const handleBuscar = () => {
  const historicoRequest: HistoricoRequestDTO = {};

  if(props.contexto === "historico"){
        historicoRequest.Coordenadas = { 
        latitude: latitude?.value,
        longitude: longitude?.value,
      };
      historicoRequest.NomeCidade = nomeCidade.value;
    emit('buscarPorHistorico', historicoRequest);
  }

  if(props.contexto === "consulta" && tipoBusca.value === 'coordenadas'){
      emit('buscarPorCoordenadas', latitude.value!, longitude.value!);
  }

  if(props.contexto === "consulta" && tipoBusca.value === 'cidade'){
      emit('buscarPorCidade', nomeCidade.value);
  }
};
</script>

<template>
  <Card class="busca-card">
    <template #title>
      <div class="card-header-section">
        <div class="header-top">
          <div class="badge-desafio">
            <i class="pi pi-code"></i>
            Desafio Fullstack C#
          </div>
          <div class="badge-api">
            <i class="pi pi-cloud"></i>
            API de Clima em Tempo Real
          </div>
        </div>
        <div class="card-title">
          <i class="pi pi-filter"></i>
          Consultar Por...
        </div>
      </div>
    </template>
    <template #content>
      <div class="radio-group" >
        
        <div class="radio-option">
          <RadioButton v-model="tipoBusca" inputId="cidade" value="cidade" />
          <label for="cidade">Cidade</label>
        </div>
        
        <div class="radio-option">
          <RadioButton v-model="tipoBusca" inputId="coordenadas" value="coordenadas" />
          <label for="coordenadas">Coordenadas</label>
        </div>
      </div>

      <div v-if="tipoBusca === 'cidade'" class="input-section">
          <InputText 
            id="nomeCidade" 
            v-model="nomeCidade" 
            class="w-full"
            @keyup.enter="handleBuscar"
            placeholder="Nome Da Cidade"
          />
      </div>

      <div v-else class="coordenadas-group">
          <InputNumber
            id="latitude" 
            v-model.number="latitude" 
            class="w-full"
            @keyup.enter="handleBuscar"
            placeholder="Latitude"
          />
        
          <InputNumber 
            id="longitude" 
            v-model.number="longitude" 
            type="number"
            class="w-full"
            @keyup.enter="handleBuscar"
            placeholder="Longitude"
          />
      </div>

      <Button 
        label="Buscar" 
        icon="pi pi-search" 
        @click="handleBuscar" 
        class="buscar-btn"
        severity="success"
      />
    </template>
  </Card>
</template>

<style scoped>
.busca-card {
  background-color: var(--surface-color);
  border: 1px solid var(--border-color);
  border-radius: var(--radius, 8px);
  box-shadow: none;
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.header-top {
  display: flex;
  gap: 12px;
  margin-bottom: 1rem;
  flex-wrap: wrap;
}

.badge-desafio, .badge-api {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 6px 12px; 
  border: 1px solid var(--primary);
  border-radius: var(--radius, 16px);
  background-color: rgba(0, 187, 213, 0.1);
  color: var(--primary);
  font-size: 0.875rem;
  font-weight: 600;
}

.badge-desafio i, .badge-api i {
  font-size: 1rem;
}

.radio-group {
  display: flex;
  justify-content: space-around;
  gap: 24px;
  margin-bottom: 16px;
}

.radio-option {
  display: flex;
  align-items: center;
  gap: 8px;
}

.radio-option label {
  color: var(--text-main);
  font-size: 1.25rem;
  cursor: pointer;
  transition: color 0.3s ease;
}

.radio-option label:hover {
  color: var(--primary);
}

.p-radiobutton-box {
  border: 1px solid var(--border-color);
  background-color: var(--surface-color);
  transition: border-color 0.3s ease, box-shadow 0.3s ease;
}

.p-radiobutton-box.p-highlight {
  border-color: var(--primary);
  background-color: var(--primary);
}

.input-section, .coordenadas-group {
  display: flex;
  flex-direction: column;
  gap: 16px;
  margin-bottom: 16px;
}

.input-section .w-full, .coordenadas-group .w-full {
  width: 100%;
  padding: 8px;
  border: 1px solid var(--border-color);
  border-radius: var(--radius, 4px);
  background-color: var(--bg-input);
  color: var(--text-main);
  font-size: 1rem;
  transition: border-color 0.3s ease, box-shadow 0.3s ease;
}

.input-section .w-full::placeholder, .coordenadas-group .w-full::placeholder {
  color: var(--text-muted);
}

.input-section .w-full:focus, .coordenadas-group .w-full:focus {
  outline: none;
  border-color: var(--primary);
  box-shadow: 0 0 0 3px var(--primary-transparent);
}

.buscar-btn {
  background-color: var(--primary);
  color: var(--text-main);
  border: none;
  border-radius: var(--radius, 4px);
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  margin-top: 16px;
}

.buscar-btn:hover {
  transform: scale(1.05);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.buscar-btn:active {
  transform: scale(0.95);
}
</style>