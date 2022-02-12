import React from "react";

const Login: React.FC = () => {
  return (
    <>
      <h2>Sign In</h2>
      <div className="error">
        <strong>Error:</strong>smut
      </div>
      <form action="" method="post">
        <dl>
          <dt>Username:</dt>
          <dd>
            <input type="text" name="username" size={30} />
          </dd>
          <dt>Password:</dt>
          <dd>
            <input type="password" name="password" size={30} />
          </dd>
        </dl>
        <div className="actions">
          <input type="submit" value="Sign In" />
        </div>
      </form>
    </>
  );
};

export default Login;
