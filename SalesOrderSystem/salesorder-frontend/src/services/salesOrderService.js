import { api } from './api';

export const salesOrderService = {
  getAllOrders: async () => {
    const response = await api.get('/salesorders');
    return response.data;
  },

  getOrderById: async (id) => {
    const response = await api.get(`/salesorders/${id}`);
    return response.data;
  },

  createOrder: async (orderData) => {
    const response = await api.post('/salesorders', orderData);
    return response.data;
  },

  updateOrder: async (id, orderData) => {
    const response = await api.put(`/salesorders/${id}`, orderData);
    return response.data;
  },

  deleteOrder: async (id) => {
    await api.delete(`/salesorders/${id}`);
  },
};