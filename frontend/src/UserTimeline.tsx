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
      if (!user || timelineMessages.length === 0) {
        return;
      }

      fetch(
        `${baseUrl}/User/${shouldFollow ? "Follow" : "Unfollow"}?username=${
          timelineMessages[0].userName
        }`,
        {
          headers: {
            Authorization: user.token,
          },
        }
      );
    },
    [timelineMessages, user]
  );

  return (
    <>
      <h2>User Timeline</h2>
      {user ? (
        <div className="followstatus">
          {Number(params.userId) === user.userId ? (
            "This is you!"
          ) : (
            <>
              {isFollowing ? (
                <>
                  You are currently following this user.
                  <a
                    className="unfollow"
                    onClick={() => handleFollowChange(false)}
                  >
                    Unfollow user
                  </a>
                  .
                </>
              ) : (
                <>
                  You are not yet following this user.
                  <a
                    className="follow"
                    onClick={() => handleFollowChange(true)}
                  >
                    Follow user
                  </a>
                  .
                </>
              )}
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
