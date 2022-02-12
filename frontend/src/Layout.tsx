import React from "react";
import { Link } from "react-router-dom";
import "./Layout.css";

const Layout: React.FC = ({ children }) => (
  <>
    <div className="page">
      <h1>MiniTwit</h1>
      <div className="navigation">
        <Link to="/">my timeline</Link> |
        <Link to="/public">public timeline</Link> |
        <Link to="/login">sign out </Link> |
        {/* TODO: lav basmand om til knap */}
        <Link to="/register">sign up</Link> |<Link to="/login">sign in</Link>
      </div>
      <ul className="flashes">
        <li>Test</li>
      </ul>
      <div className="body">{children}</div>
      <div className="footer">
        MiniTwit &mdash; A{" "}
        <i style={{ textDecoration: "line-through" }}>!Flask</i> Application
      </div>
    </div>
  </>
);

export default Layout;
