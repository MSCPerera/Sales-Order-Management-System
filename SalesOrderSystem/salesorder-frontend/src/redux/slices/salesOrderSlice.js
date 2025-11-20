import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { salesOrderService } from '../../services/salesOrderService';

export const fetchOrders = createAsyncThunk(
  'salesOrder/fetchOrders',
  async () => {
    const response = await salesOrderService.getAllOrders();
    return response;
  }
);

export const fetchOrderById = createAsyncThunk(
  'salesOrder/fetchOrderById',
  async (id) => {
    const response = await salesOrderService.getOrderById(id);
    return response;
  }
);

export const createOrder = createAsyncThunk(
  'salesOrder/createOrder',
  async (orderData) => {
    const response = await salesOrderService.createOrder(orderData);
    return response;
  }
);

export const updateOrder = createAsyncThunk(
  'salesOrder/updateOrder',
  async ({ id, orderData }) => {
    const response = await salesOrderService.updateOrder(id, orderData);
    return response;
  }
);

const salesOrderSlice = createSlice({
  name: 'salesOrder',
  initialState: {
    orders: [],
    currentOrder: null,
    loading: false,
    error: null,
  },
  reducers: {
    clearCurrentOrder: (state) => {
      state.currentOrder = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchOrders.pending, (state) => {
        state.loading = true;
      })
      .addCase(fetchOrders.fulfilled, (state, action) => {
        state.loading = false;
        state.orders = action.payload;
      })
      .addCase(fetchOrders.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message;
      })
      .addCase(fetchOrderById.fulfilled, (state, action) => {
        state.currentOrder = action.payload;
      })
      .addCase(createOrder.fulfilled, (state, action) => {
        state.orders.unshift(action.payload);
      })
      .addCase(updateOrder.fulfilled, (state, action) => {
        const index = state.orders.findIndex(o => o.id === action.payload.id);
        if (index !== -1) {
          state.orders[index] = action.payload;
        }
      });
  },
});

export const { clearCurrentOrder } = salesOrderSlice.actions;
export default salesOrderSlice.reducer;