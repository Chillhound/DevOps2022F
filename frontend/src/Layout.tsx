import { setDefaultResultOrder } from "dns";
import React from "react";
import { Link } from "react-router-dom";
import "./Layout.css";
import userContext from "./utils/userContext";

const Layout: React.FC = ({ children }) => {
  const { user, setUser } = React.useContext(userContext);

  return (
    <>
      <div className="page">
        <h1>MiniTwit</h1>
        <div className="navigation">
          {user !== null ? (
            <>
              <Link to="/">my timeline</Link> |
            </>
          ) : null}
          <Link to="/public">public timeline</Link> |
          {user !== null ? (
            <>
              <Link to="/login" onClick={() => setUser(null)}>
                sign out
              </Link>
              |
            </>
          ) : (
            <>
              <Link to="/register">sign up</Link> |
              <Link to="/login">sign in</Link>
            </>
          )}
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
};

export default Layout;
