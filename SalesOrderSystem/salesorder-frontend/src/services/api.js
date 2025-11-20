import axios from 'axios';

// API runs from the ASP.NET Core project (see launchSettings.json)
// update the port to match the backend (http://localhost:5056)
const API_BASE_URL = 'http://localhost:5056/api';

export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});