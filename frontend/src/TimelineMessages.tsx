import React from "react";
import { Link } from "react-router-dom";
import { gravatarUrl } from "./utils/gravatar";

const TimelineMessages: React.FC<{ messages: any[] }> = ({ messages }) => {
  return (
    <>
      <ul className="messages">
        {messages.map((item) => (
          <li key={item.messageId}>
            <img src={gravatarUrl(item.email, 40)} alt="" />
            <p>
              <strong>
                <Link to={`/${item.userId}`}>{item.userName}</Link>
              </strong>
              <br />
              {item.text}
              <small>&mdash; {new Date(item.pubDate).toISOString()}</small>
            </p>
          </li>
        ))}
        {messages.length === 0 ? (
          <li>
            <em>There&apos;s no message so far.</em>
          </li>
        ) : null}
      </ul>
    </>
  );
};

export default TimelineMessages;
