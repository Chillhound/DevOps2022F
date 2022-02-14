import React from "react";
import { useParams } from "react-router";
import useSWR from "swr";
import TimelineMessages from "./TimelineMessages";
import TwitBox from "./Twitbox";
import { baseUrl } from "./utils/config";
import { fetcher } from "./utils/fetcher";
import userContext from "./utils/userContext";

const UserTimeline: React.FC = () => {
  const { user } = React.useContext(userContext);
  const params = useParams();
  const { data, mutate } = useSWR(
    user
      ? [
          `${baseUrl}/User/UserMessages?userId=${params.userId || 0}`,
          user.token,
        ]
      : null,
    fetcher
  );

  const handleFollowChange = React.useCallback(
    (shouldFollow: boolean) => {
      if (!user || !data || data?.messages.length === 0) {
        return;
      }

      fetch(
        `${baseUrl}/User/${shouldFollow ? "Follow" : "Unfollow"}?username=${
          data.messages[0].userName
        }`,
        {
          headers: {
            Authorization: user.token,
          },
        }
      ).then(() => mutate());
    },
    [data, mutate, user]
  );

  return (
    <>
      <h2>User Timeline</h2>
      {user && data ? (
        <div className="followstatus">
          {Number(params.userId) === user.userId ? (
            "This is you!"
          ) : (
            <>
              {data.isFollowing ? (
                <>
                  You are currently following this user.
                  <button
                    className="unfollow"
                    onClick={() => handleFollowChange(false)}
                  >
                    Unfollow user
                  </button>
                  .
                </>
              ) : (
                <>
                  You are not yet following this user.
                  <button
                    className="follow"
                    onClick={() => handleFollowChange(true)}
                  >
                    Follow user
                  </button>
                  .
                </>
              )}
            </>
          )}
        </div>
      ) : null}
      {user !== null && Number(params.userId) === user.userId ? (
        <TwitBox user={user} />
      ) : null}
      <TimelineMessages messages={data?.messages || []} />
    </>
  );
};

export default UserTimeline;
