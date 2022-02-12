import React from "react";

const Timeline: React.FC = () => (
  <>
    <h2>New Title</h2>
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
    <div className="twitbox">
      <h3>What's on your mind g.user.username?</h3>
      <form action="{{ url_for('add_message') }}" method="post">
        <p>
          <input type="text" name="text" size={60} />
          <input type="submit" value="Share" />
        </p>
      </form>
    </div>
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

export default Timeline;
