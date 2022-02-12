import React from "react";

const Register: React.FC = () => (
  <>
    <h2>Sign Up</h2>
    <div className="error">
      <strong>Error:</strong>sut
    </div>
    <form action="" method="post">
      <dl>
        <dt>Username:</dt>
        <dd>
          <input type="text" name="username" size={30} />
        </dd>
        <dt>E-Mail:</dt>
        <dd>
          <input type="text" name="email" size={30} />
        </dd>
        <dt>Password:</dt>
        <dd>
          <input type="password" name="password" size={30} />
        </dd>
        <dt>
          Password <small>(repeat)</small>:
        </dt>
        <dd>
          <input type="password" name="password2" size={30} />
        </dd>
      </dl>
      <div className="actions">
        <input type="submit" value="Sign Up" />
      </div>
    </form>
  </>
);

export default Register;
