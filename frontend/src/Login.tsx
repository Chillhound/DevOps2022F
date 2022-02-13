import React from "react";
import { SubmitHandler, useForm } from "react-hook-form";
import { useNavigate } from "react-router";
import { baseUrl } from "./utils/config";
import userContext from "./utils/userContext";

interface FormValues {
  userName: string;
  password: string;
}

const Login: React.FC = () => {
  const { register, handleSubmit } = useForm<FormValues>();
  const { setUser } = React.useContext(userContext);
  const navigate = useNavigate();

  const onSubmit: SubmitHandler<FormValues> = React.useCallback(
    ({ userName, password }) => {
      fetch(`${baseUrl}/Account/Login`, {
        headers: {
          "Content-Type": "application/json",
        },
        method: "POST",
        body: JSON.stringify({ userName, password }),
      })
        .then((res) => res.text())
        .then((data) => {
          setUser({
            userName,
            token: `Bearer ${data}`,
          });
          // set flash context
          navigate("/");
        });
    },
    [navigate, setUser]
  );

  return (
    <>
      <h2>Sign In</h2>
      <div className="error">
        <strong>Error:</strong>smut
      </div>
      <form action="" onSubmit={handleSubmit(onSubmit)}>
        <dl>
          <dt>Username:</dt>
          <dd>
            <input
              type="text"
              {...register("userName", {
                required: "Invalid username",
                maxLength: 30,
              })}
            />
          </dd>
          <dt>Password:</dt>
          <dd>
            <input
              type="password"
              {...register("password", { required: true, maxLength: 30 })}
            />
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
