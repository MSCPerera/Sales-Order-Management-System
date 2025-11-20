import { api } from './api';

export const clientService = {
  getAllClients: async () => {
    const response = await api.get('/clients');
    return response.data;
  },

  getClientById: async (id) => {
    const response = await api.get(`/clients/${id}`);
    return response.data;
  },
};