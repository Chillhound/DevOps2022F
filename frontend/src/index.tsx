import React from "react";
import ReactDOM from "react-dom";
import "./index.css";
import reportWebVitals from "./reportWebVitals";
import Router from "./Router";
import { FlashContextProvider } from "./utils/flashContext";
import { UserContextProvider } from "./utils/userContext";

ReactDOM.render(
  <React.StrictMode>
    <UserContextProvider>
      <FlashContextProvider>
        <Router />
      </FlashContextProvider>
    </UserContextProvider>
  </React.StrictMode>,
  document.getElementById("root")
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
