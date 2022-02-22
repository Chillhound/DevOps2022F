import React from "react";
import { SubmitErrorHandler, SubmitHandler, useForm } from "react-hook-form";
import { baseUrl } from "./utils/config";

interface FormValues {
  username: string;
  email: string;
  password: string;
  password2: string;
}

const Register: React.FC = () => {
  const [error, setError] = React.useState<string | null>(null);
  const onSubmit: SubmitHandler<FormValues> = React.useCallback((data) => {
    setError(null);

    fetch(`${baseUrl}/Account/Register`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        userName: data.username,
        email: data.email,
        pwd: data.password,
      }),
    })
      .then(() => (window.location.href = "http://localhost:3000/login"))
      .catch((err) => setError(err));
  }, []);

  const onError: SubmitErrorHandler<FormValues> = React.useCallback(
    (errors) => {
      setError(null);
      if (Object.values(errors).length > 0) {
        setError(Object.values(errors)[0].message ?? null);
      }
    },
    []
  );

  const { register, handleSubmit, getValues } = useForm<FormValues>();

  return (
    <>
      <h2>Sign Up</h2>
      {error !== null ? (
        <div className="error">
          <strong>Error:</strong>
          {error}
        </div>
      ) : null}
      <form action="" onSubmit={handleSubmit(onSubmit, onError)}>
        <dl>
          <dt>Username:</dt>
          <dd>
            <input
              type="text"
              {...register("username", {
                required: "You have to enter a username",
                maxLength: 30,
              })}
            />
          </dd>
          <dt>E-Mail:</dt>
          <dd>
            <input
              type="text"
              {...register("email", {
                required: "You have to enter a valid email address",
                maxLength: 30,
              })}
            />
          </dd>
          <dt>Password:</dt>
          <dd>
            <input
              type="password"
              {...register("password", {
                required: "You have to enter a password",
                maxLength: 30,
              })}
            />
          </dd>
          <dt>
            Password <small>(repeat)</small>:
          </dt>
          <dd>
            <input
              type="password"
              {...register("password2", {
                validate: (value) =>
                  // TODO: Make sure password are equal
                  getValues("password") !== value
                    ? "The two passwords do not match"
                    : undefined,
                maxLength: 30,
              })}
            />
          </dd>
        </dl>
        <div className="actions">
          <input type="submit" value="Sign Up" />
        </div>
      </form>
    </>
  );
};

export default Register;
