<script setup lang="ts">
import { ref, computed } from 'vue';
import { useWeatherApi } from '@/composables/useWeatherApi';
import ConsultaToggle from '@/components/ConsultaToggle.vue';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Button from 'primevue/button';
import Chart from 'primevue/chart';
import type { ClimaResponseDTO } from '@/interfaces/ClimaResponseDTO';
import { HistoricoRequestDTO } from '@/interfaces/HistoricoRequestDTO';
import { useToast } from 'primevue/usetoast';
const toast = useToast();

const { buscarHistorico } = useWeatherApi();
const historicoResponse = ref<ClimaResponseDTO[]>([]);
const loading = ref(false);
const visualizacao = ref<'lista' | 'grafico'>('lista');

const handleBuscarPorHistorico = async (historicoRequest: HistoricoRequestDTO) => {
  loading.value = true;
  let summary = '';
  let detail = '';

  try {

    const nomeCidade = historicoRequest?.NomeCidade;
    const latitude = historicoRequest?.Coordenadas.latitude;
    const longitude = historicoRequest?.Coordenadas.longitude;

    const temCidade = nomeCidade;
    const temCoordenadas = latitude || longitude ;
    
    console.log("tem corrdenada", temCoordenadas);
    console.log("tem cidade", temCidade);

    if (temCidade) {
       if (!/^[A-Za-zÀ-ÖØ-öø-ÿ\s]+$/.test(nomeCidade)) {
        summary = 'Nome inválido';
        detail = 'O nome da cidade não pode conter números ou caracteres especiais.';
      }
      else if (nomeCidade.length < 3) {
        summary = 'Nome muito curto';
        detail = 'O nome da cidade deve ter pelo menos 3 caracteres.';
      }
      else if (nomeCidade.length > 100) {
        summary = 'Nome muito longo';
        detail = 'O nome da cidade deve ter no máximo 100 caracteres.';
      }
    }

     if (temCoordenadas) {

      if (latitude === null || latitude === undefined ||
          longitude === null || longitude === undefined) {
        summary = 'Campos obrigatórios';
        detail = 'Latitude e longitude devem ser informadas.';
      }
      else if (isNaN(latitude) || isNaN(longitude)) {
        summary = 'Valor inválido';
        detail = 'Latitude e longitude devem ser números válidos.';
      }
      else if (latitude < -90 || latitude > 90) {
        summary = 'Latitude inválida';
        detail = 'A latitude deve estar entre -90 e 90.';
      }
      else if (longitude < -180 || longitude > 180) {
        summary = 'Longitude inválida';
        detail = 'A longitude deve estar entre -180 e 180.';
      }
    }

    if (summary) {
      toast.add({
        severity: 'warn',
        summary,
        detail,
        life: 3000
      });
      return;
    }

    historicoResponse.value = await buscarHistorico(historicoRequest);

    toast.add({
      severity: 'success',
      summary: 'Sucesso',
      detail: 'Histórico consultado com sucesso.',
      life: 3000
    });

  } catch (error: any) {

    toast.add({
      severity: 'error',
      summary: 'Erro na consulta',
      detail: error?.message || 'Não foi possível buscar o histórico.',
      life: 3000
    });

  } finally {
    loading.value = false;
  }
};

const chartData = computed(() => {
  if (historicoResponse.value.length === 0) return null;

  return {
    labels: historicoResponse.value.map(item => 
      new Date(item.dataHora).toLocaleString('pt-BR', {
        day: '2-digit',
        month: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
      })
    ),
    datasets: [
      {
        label: 'Temperatura (°C)',
        data: historicoResponse.value.map(item => item.temperatura),
        fill: false,
        borderColor: '#42A5F5',
        tension: 0.4,
        backgroundColor: '#42A5F5'
      },
      {
        label: 'Temperatura Mínima (°C)',
        data: historicoResponse.value.map(item => item.temperaturaMin),
        fill: false,
        borderColor: '#66BB6A',
        tension: 0.4,
        backgroundColor: '#66BB6A'
      },
      {
        label: 'Temperatura Máxima (°C)',
        data: historicoResponse.value.map(item => item.temperaturaMax),
        fill: false,
        borderColor: '#FFA726',
        tension: 0.4,
        backgroundColor: '#FFA726'
      }
    ]
  };
});

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      labels: {
        color: '#000000'
      }
    }
  },
  scales: {
    x: {
      ticks: {
        color: '#000000'
      },
      grid: {
        color: 'rgba(0, 0, 0, 0.1)'
      }
    },
    y: {
      ticks: {
        color: '#000000'
      },
      grid: {
        color: 'rgba(0, 0, 0, 0.1)'
      }
    }
  }
};

const formatarData = (data: string) => {
  return new Date(data).toLocaleString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
};
</script>

<template>
  <div class="history-view">
    
    <ConsultaToggle 
      contexto="historico"
      @buscar-por-historico="handleBuscarPorHistorico"
    />

    <div v-if="loading" class="loading">
      <i class="pi pi-spin pi-spinner" style="font-size: 2rem"></i>
      <p>Carregando histórico...</p>
    </div>

    <div v-else-if="historicoResponse.length > 0" class="historico-container">
      <div class="toggle-buttons" style="margin-bottom: 20px;">
        <Button 
          label="Lista" 
          :class="{ active: visualizacao === 'lista' }"
          @click="visualizacao = 'lista'"
        />
        <Button 
          label="Gráfico" 
          :class="{ active: visualizacao === 'grafico' }"
          @click="visualizacao = 'grafico'"
        />
      </div>

      <div v-if="visualizacao === 'lista'" class="lista-view">
        <DataTable 
          :value="historicoResponse" 
          stripedRows 
          responsiveLayout="scroll"
          class="tabla-bonita"
          :rows="10"
          paginator
        >
          <Column field="cidade.nome" header="Cidade" class="col-cidade" />
          <Column field="cidade.pais" header="País" class="col-pais" />
          <Column field="temperatura" header="Temperatura" class="col-temperatura" headerStyle="text-align: center;" />
          <Column field="temperaturaMin" header="Mínima" class="col-temp-min" headerStyle="text-align: center;">
            <template #body="slotProps">
              {{ slotProps.data.temperaturaMin }}°C
            </template> 
          </Column>
          <Column field="temperaturaMax" header="Máxima" class="col-temp-max" headerStyle="text-align: center;">
            <template #body="slotProps">  
              {{ slotProps.data.temperaturaMax }}°C
            </template>
          </Column>
          <Column field="latitude" header="Latitude" class="col-latitude" headerStyle="text-align: center;" />
          <Column field="longitude" header="Longitude" class="col-longitude" headerStyle="text-align: center;" />
          <Column field="condicao" header="Condição" class="col-condicao" />
          <Column field="dataHora" header="Data Registro" class="col-data" headerStyle="text-align: center;">
            <template #body="slotProps">
              {{ formatarData(slotProps.data.dataHora) }}
            </template>
          </Column>
        </DataTable>
      </div>

      <div v-else class="grafico-view">
        <Chart type="line" :data="chartData" :options="chartOptions" class="chart" />
      </div>
    </div>

    <div v-else-if="!loading && historicoResponse.length === 0" class="empty-state">
      <i class="pi pi-inbox" style="font-size: 3rem; color: var(--text-muted);"></i>
      <p>Nenhum histórico encontrado</p>
      <small>Faça uma consulta para visualizar dados</small>
    </div>
  </div>
</template>

<style scoped>

/* Scoped CSS for HistoryView.vue */

/* ========================= */
/*     History Container     */
/* ========================= */
.history-view {
  padding: 24px;
  background-color: var(--bg-color);
  display: flex;
  flex-direction: column;
  gap: 16px;
}

/* ========================= */
/*      Loading State        */
/* ========================= */
.loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: var(--text-muted);
  gap: 8px;
}

.loading i {
  color: var(--primary);
}

/* ========================= */
/*     Toggle Buttons        */
/* ========================= */
.toggle-buttons {
  display: flex;
  justify-content: center;
  gap: 8px;
}

.toggle-buttons .p-button {
  border-radius: var(--radius, 4px);
  transition: background-color 0.3s ease, color 0.3s ease;
}

.toggle-buttons .p-button.active {
  background-color: var(--primary);
  color: var(--text-main);
}

.toggle-buttons .p-button:not(.active) {
  background-color: var(--surface-color);
  color: var(--text-muted);
}

/* ========================= */
/*       DataTable           */
/* ========================= */
.tabla-bonita :deep(.p-datatable-thead > tr > th) {
  color: var(--primary-color);
  padding: 1.25rem 1rem; /* Aumenta o padding do cabeçalho */
  font-size: 1rem;
  border-bottom: 2px solid var(--border-color);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.tabla-bonita :deep(.p-datatable-tbody > tr > td) {
  padding: 1.25rem 1rem; 
  font-size: 0.95rem;     
  color: var(--text-main);
}

.tabla-bonita :deep(.p-datatable-tbody > tr:hover) {
  background-color: var(--primary-low) !important;
  transition: background-color 0.2s ease;
}

/* ========================= */
/*        Chart View         */
/* ========================= */
.grafico-view {
  background-color: #8fc0ff;
  border: 1px solid var(--border-color);
  border-radius: var(--radius, 4px);
  box-shadow: var(--shadow, 0 4px 8px rgba(0, 0, 0, 0.1));
  padding: 16px;
}

.chart {
  width: 100%;
  height: 400px;
}
</style>