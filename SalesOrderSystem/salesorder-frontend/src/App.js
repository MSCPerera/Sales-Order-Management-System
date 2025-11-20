import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Provider } from 'react-redux';
import { store } from './redux/store';
import HomePage from './pages/HomePage';
import SalesOrderPage from './pages/SalesOrderPage';

function App() {
  return (
    <Provider store={store}>
      <Router>
        <div className="min-h-screen bg-gray-100">
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/sales-order" element={<SalesOrderPage />} />
            <Route path="/sales-order/:id" element={<SalesOrderPage />} />
          </Routes>
        </div>
      </Router>
    </Provider>
  );
}

export default App;