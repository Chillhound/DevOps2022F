import React from "react";
import { baseUrl } from "./utils/config";

const TimelineMessages = () => {
  const [timelineItems, setTimelineItems] = React.useState<any[]>([]);

  React.useEffect(() => {
    fetch(`${baseUrl}/Message/PublicTimeline?limit=100`)
      .then((res) => res.json())
      .then((data) => setTimelineItems(data));
  }, []);

  console.log(timelineItems);

  return (
    <>
      <ul className="messages">
        {timelineItems.map((item) => (
          <li key={item.messageId}>
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
              {item.text}
              <small>&mdash; {new Date(item.pubDate).toISOString()}</small>
            </p>
          </li>
        ))}
        {timelineItems.length === 0 ? (
          <li>
            <em>There's no message so far.</em>
          </li>
        ) : null}
      </ul>
    </>
  );
};

export default TimelineMessages;
