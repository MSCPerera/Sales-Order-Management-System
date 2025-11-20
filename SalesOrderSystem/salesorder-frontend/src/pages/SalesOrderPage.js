import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useDispatch } from 'react-redux';
import { createOrder, updateOrder, fetchOrderById } from '../redux/slices/salesOrderSlice';
import { clientService } from '../services/clientService';
import { itemService } from '../services/itemService';

const SalesOrderPage = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { id } = useParams();
  const isEditMode = !!id;

  const [clients, setClients] = useState([]);
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);

  const [formData, setFormData] = useState({
    clientId: '',
    orderDate: new Date().toISOString().split('T')[0],
    deliveryAddress: '',
    city: '',
    postalCode: '',
    lines: [],
  });

  const [currentLine, setCurrentLine] = useState({
    itemId: '',
    note: '',
    quantity: 1,
    taxRate: 0,
  });

  useEffect(() => {
    loadData();
  }, [id]);

  const loadData = async () => {
    try {
      const [clientsData, itemsData] = await Promise.all([
        clientService.getAllClients(),
        itemService.getAllItems(),
      ]);

      setClients(clientsData);
      setItems(itemsData);

      if (isEditMode) {
        const orderData = await dispatch(fetchOrderById(id)).unwrap();
        setFormData({
          clientId: orderData.clientId,
          orderDate: orderData.orderDate.split('T')[0],
          deliveryAddress: orderData.deliveryAddress,
          city: orderData.city,
          postalCode: orderData.postalCode,
          lines: orderData.lines.map(line => ({
            itemId: line.itemId,
            note: line.note,
            quantity: line.quantity,
            taxRate: line.taxRate,
            price: line.price,
            exclAmount: line.exclAmount,
            taxAmount: line.taxAmount,
            inclAmount: line.inclAmount,
          })),
        });
      }

      setLoading(false);
    } catch (error) {
      console.error('Error loading data:', error);
      setLoading(false);
    }
  };

  const handleClientChange = (e) => {
    const clientId = parseInt(e.target.value);
    const selectedClient = clients.find(c => c.id === clientId);

    if (selectedClient) {
      setFormData({
        ...formData,
        clientId: clientId,
        deliveryAddress: selectedClient.address,
        city: selectedClient.city,
        postalCode: selectedClient.postalCode,
      });
    } else {
      setFormData({
        ...formData,
        clientId: '',
        deliveryAddress: '',
        city: '',
        postalCode: '',
      });
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleLineChange = (e) => {
    const { name, value } = e.target;
    setCurrentLine({ ...currentLine, [name]: value });
  };

  const handleItemSelect = (e) => {
    const itemId = parseInt(e.target.value);
    setCurrentLine({ ...currentLine, itemId });
  };

  const calculateLineAmounts = (itemId, quantity, taxRate) => {
    const item = items.find(i => i.id === itemId);
    if (!item) return { exclAmount: 0, taxAmount: 0, inclAmount: 0 };

    const exclAmount = quantity * item.price;
    const taxAmount = (exclAmount * taxRate) / 100;
    const inclAmount = exclAmount + taxAmount;

    return { exclAmount, taxAmount, inclAmount, price: item.price };
  };

  const addLine = () => {
    if (!currentLine.itemId || currentLine.quantity <= 0) {
      alert('Please select an item and enter a valid quantity');
      return;
    }

    const amounts = calculateLineAmounts(
      currentLine.itemId,
      parseInt(currentLine.quantity),
      parseFloat(currentLine.taxRate)
    );

    const newLine = {
      ...currentLine,
      itemId: parseInt(currentLine.itemId),
      quantity: parseInt(currentLine.quantity),
      taxRate: parseFloat(currentLine.taxRate),
      ...amounts,
    };

    setFormData({
      ...formData,
      lines: [...formData.lines, newLine],
    });

    setCurrentLine({
      itemId: '',
      note: '',
      quantity: 1,
      taxRate: 0,
    });
  };

  const removeLine = (index) => {
    const newLines = formData.lines.filter((_, i) => i !== index);
    setFormData({ ...formData, lines: newLines });
  };

  const calculateTotals = () => {
    const totalExcl = formData.lines.reduce((sum, line) => sum + line.exclAmount, 0);
    const totalTax = formData.lines.reduce((sum, line) => sum + line.taxAmount, 0);
    const totalIncl = formData.lines.reduce((sum, line) => sum + line.inclAmount, 0);

    return { totalExcl, totalTax, totalIncl };
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!formData.clientId) {
      alert('Please select a customer');
      return;
    }

    if (formData.lines.length === 0) {
      alert('Please add at least one item');
      return;
    }

    try {
      const orderData = {
        clientId: parseInt(formData.clientId),
        orderDate: new Date(formData.orderDate).toISOString(),
        deliveryAddress: formData.deliveryAddress,
        city: formData.city,
        postalCode: formData.postalCode,
        lines: formData.lines.map(line => ({
          itemId: line.itemId,
          note: line.note,
          quantity: line.quantity,
          taxRate: line.taxRate,
        })),
      };

      if (isEditMode) {
        await dispatch(updateOrder({ id: parseInt(id), orderData })).unwrap();
        alert('Order updated successfully!');
      } else {
        await dispatch(createOrder(orderData)).unwrap();
        alert('Order created successfully!');
      }

      navigate('/');
    } catch (error) {
      console.error('Error saving order:', error);
      alert('Error saving order. Please try again.');
    }
  };

  const getItemDetails = (itemId) => {
    return items.find(i => i.id === itemId);
  };

  const { totalExcl, totalTax, totalIncl } = calculateTotals();

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen">
        <div className="text-xl">Loading...</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="bg-white rounded-lg shadow-md p-6">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold text-gray-800">
            {isEditMode ? 'Edit Sales Order' : 'New Sales Order'}
          </h1>
          <button
            onClick={() => navigate('/')}
            className="bg-gray-500 hover:bg-gray-600 text-white font-semibold px-4 py-2 rounded-lg"
          >
            Back to Home
          </button>
        </div>

        <form onSubmit={handleSubmit}>
          {/* Customer Information */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Customer Name *
              </label>
              <select
                value={formData.clientId}
                onChange={handleClientChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                required
              >
                <option value="">Select Customer</option>
                {clients.map(client => (
                  <option key={client.id} value={client.id}>
                    {client.customerName}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Order Date *
              </label>
              <input
                type="date"
                name="orderDate"
                value={formData.orderDate}
                onChange={handleInputChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                required
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Delivery Address
              </label>
              <input
                type="text"
                name="deliveryAddress"
                value={formData.deliveryAddress}
                onChange={handleInputChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                City
              </label>
              <input
                type="text"
                name="city"
                value={formData.city}
                onChange={handleInputChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Postal Code
              </label>
              <input
                type="text"
                name="postalCode"
                value={formData.postalCode}
                onChange={handleInputChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>
          </div>

          {/* Add Item Section */}
          <div className="border-t pt-6 mb-6">
            <h2 className="text-xl font-semibold text-gray-800 mb-4">Add Items</h2>
            
            <div className="grid grid-cols-1 md:grid-cols-6 gap-4 mb-4">
              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Item
                </label>
                <select
                  value={currentLine.itemId}
                  onChange={handleItemSelect}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                >
                  <option value="">Select Item</option>
                  {items.map(item => (
                    <option key={item.id} value={item.id}>
                      {item.itemCode} - {item.description}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Quantity
                </label>
                <input
                  type="number"
                  name="quantity"
                  value={currentLine.quantity}
                  onChange={handleLineChange}
                  min="1"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Tax Rate (%)
                </label>
                <input
                  type="number"
                  name="taxRate"
                  value={currentLine.taxRate}
                  onChange={handleLineChange}
                  min="0"
                  step="0.01"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Note
                </label>
                <input
                  type="text"
                  name="note"
                  value={currentLine.note}
                  onChange={handleLineChange}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>
            </div>

            <button
              type="button"
              onClick={addLine}
              className="bg-green-600 hover:bg-green-700 text-white font-semibold px-6 py-2 rounded-lg"
            >
              Add Item
            </button>
          </div>

          {/* Order Lines Table */}
          {formData.lines.length > 0 && (
            <div className="mb-6 overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Item Code</th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Description</th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Note</th>
                    <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Qty</th>
                    <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Price</th>
                    <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Tax %</th>
                    <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Excl Amt</th>
                    <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Tax Amt</th>
                    <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Incl Amt</th>
                    <th className="px-4 py-3"></th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {formData.lines.map((line, index) => {
                    const item = getItemDetails(line.itemId);
                    return (
                      <tr key={index}>
                        <td className="px-4 py-3 text-sm">{item?.itemCode}</td>
                        <td className="px-4 py-3 text-sm">{item?.description}</td>
                        <td className="px-4 py-3 text-sm">{line.note}</td>
                        <td className="px-4 py-3 text-sm text-right">{line.quantity}</td>
                        <td className="px-4 py-3 text-sm text-right">{line.price.toFixed(2)}</td>
                        <td className="px-4 py-3 text-sm text-right">{line.taxRate.toFixed(2)}</td>
                        <td className="px-4 py-3 text-sm text-right">{line.exclAmount.toFixed(2)}</td>
                        <td className="px-4 py-3 text-sm text-right">{line.taxAmount.toFixed(2)}</td>
                        <td className="px-4 py-3 text-sm text-right font-semibold">{line.inclAmount.toFixed(2)}</td>
                        <td className="px-4 py-3 text-sm">
                          <button
                            type="button"
                            onClick={() => removeLine(index)}
                            className="text-red-600 hover:text-red-800"
                          >
                            Remove
                          </button>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          )}

          {/* Totals */}
          <div className="border-t pt-4 mb-6">
            <div className="flex justify-end">
              <div className="w-full md:w-1/3">
                <div className="flex justify-between mb-2">
                  <span className="font-medium">Total Excl Amount:</span>
                  <span>Rs. {totalExcl.toFixed(2)}</span>
                </div>
                <div className="flex justify-between mb-2">
                  <span className="font-medium">Total Tax Amount:</span>
                  <span>Rs. {totalTax.toFixed(2)}</span>
                </div>
                <div className="flex justify-between text-lg font-bold border-t pt-2">
                  <span>Total Incl Amount:</span>
                  <span>Rs. {totalIncl.toFixed(2)}</span>
                </div>
              </div>
            </div>
          </div>

          {/* Submit Button */}
          <div className="flex justify-end gap-4">
            <button
              type="button"
              onClick={() => navigate('/')}
              className="bg-gray-500 hover:bg-gray-600 text-white font-semibold px-6 py-2 rounded-lg"
            >
              Cancel
            </button>
            <button
              type="submit"
              className="bg-blue-600 hover:bg-blue-700 text-white font-semibold px-6 py-2 rounded-lg"
            >
              {isEditMode ? 'Update Order' : 'Save Order'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default SalesOrderPage;