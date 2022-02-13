import React from "react";
import { useParams } from "react-router";
import TimelineMessages from "./TimelineMessages";
import TwitBox from "./Twitbox";
import { baseUrl } from "./utils/config";
import userContext from "./utils/userContext";

const UserTimeline: React.FC = () => {
  const { user } = React.useContext(userContext);
  const [timelineMessages, setTimelineMessages] = React.useState<any[]>([]);
  const [isFollowing, setIsFollowing] = React.useState(false);
  const params = useParams();

  console.log(params.userId);
  React.useEffect(() => {
    if (!user) {
      return;
    }
    fetch(`${baseUrl}/User/UserMessages?userId=${params.userId || 0}`, {
      headers: {
        "Content-Type": "application/json",
        Authorization: user.token,
      },
    })
      .then((res) => res.json())
      .then((data) => {
        setTimelineMessages(data.messages);
        setIsFollowing(data.isFollowing);
      });
  }, [params.userId, user]);

  const handleFollowChange = React.useCallback(
    (shouldFollow: boolean) => {
      if (!user) {
        return;
      }

      fetch(`${baseUrl}/User/${shouldFollow ? "Follow" : "Unfollow"}`, {
        headers: {
          Authorization: user.token,
        },
      });
    },
    [user]
  );

  return (
    <>
      <h2>User Timeline</h2>
      {String(isFollowing)}
      {user ? (
        <div className="followstatus">
          {params.userId === user.userId ? "This is you!" : null}
          {isFollowing ? (
            <>
              You are currently following this user.
              <a className="unfollow" onClick={() => handleFollowChange(false)}>
                Unfollow user
              </a>
              .
            </>
          ) : (
            <>
              You are not yet following this user.
              <a className="follow" onClick={() => handleFollowChange(true)}>
                Follow user
              </a>
              .
            </>
          )}
        </div>
      ) : null}
      {user !== null ? <TwitBox user={user} /> : null}
      <TimelineMessages messages={timelineMessages} />
    </>
  );
};

export default UserTimeline;
