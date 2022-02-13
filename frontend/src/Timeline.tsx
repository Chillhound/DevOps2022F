import React from "react";
import TimelineMessages from "./TimelineMessages";
import TwitBox from "./Twitbox";
import userContext from "./utils/userContext";

enum TimelineTypes {
  PUBLIC = "public",
  PRIVATE = "private",
  USER = "user",
}

interface Props {
  timelineType?: TimelineTypes;
}

const Timeline: React.FC<Props> = ({ timelineType = TimelineTypes.PUBLIC }) => {
  const { user } = React.useContext(userContext);
  return (
    <>
      <h2>New Title</h2>
      {timelineType === TimelineTypes.USER ? (
        <div className="followstatus">
          This is you! You are currently following this user.
          <a
            className="unfollow"
            href="{{ url_for('unfollow_user', username=profile_user.username)
          }}"
          >
            Unfollow user
          </a>
          . You are not yet following this user.
          <a
            className="follow"
            href="{{ url_for('follow_user', username=profile_user.username)
          }}"
          >
            Follow user
          </a>
          .
        </div>
      ) : null}
      {user !== null ? <TwitBox user={user} /> : null}
      <TimelineMessages />
      <ul className="messages">
        <li>
          <img src="{{ message.email|gravatar(size=48) }}" />
          <p>
            <strong>
              <a
                href="{{ url_for('user_timeline', username=message.username)
      }}"
              >
                {" "}
                message.username{" "}
              </a>
            </strong>
            message.text
            <small>&mdash; message.pub_date|datetimeformat </small>
          </p>
        </li>
        <li>
          <em>There's no message so far.</em>
        </li>
      </ul>
    </>
  );
};

export default Timeline;
