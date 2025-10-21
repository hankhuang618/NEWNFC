import axios from 'axios';
import type { ProductionBoardFilters, ProductionBoardRecord } from '../types';

const client = axios.create({
  baseURL: '/api'
});

export const fetchProductionBoard = async (filters: ProductionBoardFilters): Promise<ProductionBoardRecord[]> => {
  const response = await client.get<ProductionBoardRecord[]>('/ProductionBoard', {
    params: {
      plant: filters.plant,
      division: filters.division,
      section: filters.section,
      classNumber: filters.classNumber
    }
  });

  return response.data;
};
