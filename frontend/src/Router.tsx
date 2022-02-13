import { BrowserRouter, Route, Routes } from "react-router-dom";
import Layout from "./Layout";
import Login from "./Login";
import PersonalTimeline from "./PersonalTimeline";
import PublicTimeline from "./PublicTimeline";
import Register from "./Register";
import UserTimeline from "./UserTimeline";

const Router = () => (
  <BrowserRouter>
    <Layout>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/" element={<PersonalTimeline />} />
        <Route path="/public" element={<PublicTimeline />} />
        <Route path="/:userName" element={<UserTimeline />} />
      </Routes>
    </Layout>
  </BrowserRouter>
);

export default Router;
