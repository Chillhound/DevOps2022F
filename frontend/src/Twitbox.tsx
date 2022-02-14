import React, { FormEventHandler } from "react";
import { baseUrl } from "./utils/config";
import { User } from "./utils/userContext";

const TwitBox: React.FC<{ user: User; onUpdate?: () => void }> = ({
  user,
  onUpdate,
}) => {
  const [text, setText] = React.useState("");

  const handleSubmit: FormEventHandler<HTMLFormElement> = React.useCallback(
    (e) => {
      e.preventDefault();
      if (text === "") {
        return;
      }

      const url = new URL(`${baseUrl}/Message`);
      url.searchParams.append("messageText", text);

      fetch(url.toString(), {
        method: "POST",
        headers: {
          Authorization: user.token,
        },
      }).then(() => {
        if (onUpdate) {
          onUpdate();
        }
        setText("");
      });
    },
    [onUpdate, text, user.token]
  );

  return (
    <>
      <div className="twitbox">
        <h3>What's on your mind {user.userName}</h3>
        <form onSubmit={handleSubmit}>
          <p>
            <input
              type="text"
              name="text"
              size={60}
              value={text}
              onChange={(e) => setText(e.target.value)}
            />
            <input type="submit" value="Share" />
          </p>
        </form>
      </div>
    </>
  );
};

export default TwitBox;
