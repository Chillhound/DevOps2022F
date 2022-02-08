import logo from "./logo.svg";
import "./App.css";
import React from "react";

function App() {
  const [counter, setCounter] = React.useState(0);

  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.js</code> and save to reload. It has been done
        </p>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>

        <p>{counter}</p>
        <button onClick={() => setCounter((x) => x + 1)}>Increment</button>
      </header>
    </div>
  );
}

export default App;
