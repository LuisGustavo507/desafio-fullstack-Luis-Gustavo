
import { useToast } from 'primevue/usetoast';
import type { CidadeRequestDTO } from '@/interfaces/CidadeRequestDTO';
import type { CoordenadasRequestDTO } from '@/interfaces/CoordenadasRequestDTO';
import type { HistoricoRequestDTO } from '@/interfaces/HistoricoRequestDTO';
import type { ClimaResponseDTO } from '@/interfaces/ClimaResponseDTO';
import type { ErrorResponse } from '@/interfaces/ErrorResponse';
import type { UserRequestDTO } from '@/interfaces/UserRequestDTO';
import axios from 'axios';
import { LoginResponseDTO } from '@/interfaces/LoginResponseDTO';
import router from '@/router/router';

export const useWeatherApi = () => {
  const toast = useToast();
  
  const apiClient = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
    headers: {
      'Content-Type': 'application/json'
    }
  });

  const token = localStorage.getItem('token');
  if (token) {
    apiClient.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  }

  // Interceptor de resposta para tratamento de erros
  const errorInterceptor = (error: any) => {
    const errorData: ErrorResponse = {
      mensagem: error.response.data.erro || error.response.request.statusText,
      status:error.response.status,
    };
    console.error('Dados de erro da API:', error.response);

    toast.add({ 
      severity: 'error', 
      summary: errorData.mensagem, 
      detail: errorData.status, 
      life: 3000 
    });
    return Promise.reject(error);
  };

  apiClient.interceptors.response.use(
    (response) => response,
    errorInterceptor
  );

  const logarUsuario = async (user: UserRequestDTO): Promise<string> => {
    const response = await apiClient.post<LoginResponseDTO>('/login', user);
    
    if (response.status === 200 && response.data.token) {
      const token = response.data.token;
      localStorage.setItem('token', token);
      apiClient.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      
      toast.add({
        severity: 'success',
        summary: 'Sucesso',
        detail: 'Login realizado com sucesso!',
        life: 5000
      });     
      return token;
    } 
    throw new Error('Token não recebido do servidor');
  };

  const buscarClimaPorCidade = async (cidade: CidadeRequestDTO): Promise<ClimaResponseDTO> => {
    const response = await apiClient.get<ClimaResponseDTO>('/cidade', {
      params: { nome: cidade.nomeCidade }
    });
    return response.data;
  };

  const buscarClimaPorCoordenadas = async (coordenadas: CoordenadasRequestDTO): Promise<ClimaResponseDTO> => {
    const response = await apiClient.get<ClimaResponseDTO>('/coordenadas', {
      params: {
        latitude: coordenadas.latitude,
        longitude: coordenadas.longitude
      }
    });
    return response.data;
  };
  
  const buscarHistorico = async (request: HistoricoRequestDTO): Promise<ClimaResponseDTO[]> => {
    const params = {
      NomeCidade: request?.NomeCidade,
      latitude: request.Coordenadas?.latitude,
      longitude: request.Coordenadas?.longitude
    };

    const response = await apiClient.get<ClimaResponseDTO[]>('/historico', { params });
    return response.data;
  };

  const registrarUsuario = async (user: UserRequestDTO): Promise<boolean> => {
    const params = {
      Nome: user.nome,
      Senha: user.senha
    };

    const response = await apiClient.post('/registro', user);

    if (response.status === 200 || response.status === 201) {
      toast.add({
        severity: 'success',
        summary: 'Sucesso',
        detail: 'Usuário registrado com sucesso!',
        life: 5000
      });
      return true;
    }
    return false;
  };

  return {
    logarUsuario,
    buscarClimaPorCidade,
    buscarClimaPorCoordenadas,
    buscarHistorico,
    registrarUsuario
  };
};