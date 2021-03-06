import React from "react";
import { SubmitHandler, useForm } from "react-hook-form";
import { useNavigate } from "react-router";
import { baseUrl } from "./utils/config";
import flashContext from "./utils/flashContext";
import userContext from "./utils/userContext";

interface FormValues {
  userName: string;
  password: string;
}

const Login: React.FC = () => {
  const { register, handleSubmit } = useForm<FormValues>();
  const { setUser } = React.useContext(userContext);
  const tokenRef = React.useRef("");
  const navigate = useNavigate();
  const { addFlash } = React.useContext(flashContext);
  const [error, setError] = React.useState("");

  const onSubmit: SubmitHandler<FormValues> = React.useCallback(
    ({ userName, password }) => {
      setError("");
      fetch(`${baseUrl}/Account/Login`, {
        headers: {
          "Content-Type": "application/json",
        },
        method: "POST",
        body: JSON.stringify({ userName, password }),
      })
        .then((res) => {
          if (res.status !== 200) {
            throw new Error("Wrong password");
          }
          return res.text();
        })
        .then((token) => {
          tokenRef.current = token;
          fetch(`${baseUrl}/User/Me`, {
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`,
            },
          })
            .then((res) => res.json())
            .then((data) => {
              setUser({
                userName,
                userId: data.userId,
                token: `Bearer ${tokenRef.current}`,
              });
              addFlash("You were logged in!");
              navigate("/");
            });
        })
        .catch(() => setError("Bad username/password combination"));
    },
    [addFlash, navigate, setUser]
  );

  return (
    <>
      <h2>Sign In</h2>
      {error ? (
        <div className="error">
          <strong>Error:</strong> {error}
        </div>
      ) : null}
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
