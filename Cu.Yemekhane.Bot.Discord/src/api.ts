import axios from "axios";

export const api = axios.create({
  baseURL: "https://cu-yemekhane.herokuapp.com",
});
