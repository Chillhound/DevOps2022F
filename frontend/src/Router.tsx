import { BrowserRouter, Route, Routes } from "react-router-dom";
import Layout from "./Layout";
import Login from "./Login";
import Register from "./Register";
import Timeline from "./Timeline";

const Router = () => (
  <BrowserRouter>
    <Layout>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/" element={<Timeline />} />
        <Route path="/public" element={<Timeline />} />
        <Route path="/:userName" element={<Timeline />} />
      </Routes>
    </Layout>
  </BrowserRouter>
);

export default Router;
