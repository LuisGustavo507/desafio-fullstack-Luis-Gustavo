import { CoordenadasRequestDTO } from "./CoordenadasRequestDTO";

export interface HistoricoRequestDTO {
  NomeCidade?: string;
  Coordenadas?: CoordenadasRequestDTO;
}