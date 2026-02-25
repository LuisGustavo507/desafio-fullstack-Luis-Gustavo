import type { CidadeResponseDTO } from './CidadeResponseDTO';

export interface ClimaResponseDTO {
  cidade: CidadeResponseDTO;
  latitude: number;
  longitude: number;
  temperatura: number;
  temperaturaMin: number;
  temperaturaMax: number;
  condicao: string;
  dataHora: string;
}