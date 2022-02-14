import React from "react";
import useSWR from "swr";
import TimelineMessages from "./TimelineMessages";
import TwitBox from "./Twitbox";
import { baseUrl } from "./utils/config";
import { fetcher } from "./utils/fetcher";
import userContext from "./utils/userContext";

const PublicTimeline: React.FC = () => {
  const { user } = React.useContext(userContext);

  const { data, mutate } = useSWR(
    [`${baseUrl}/Message/PublicTimeline?limit=100`, ""],
    fetcher
  );

  return (
    <>
      <h2>Public Timeline</h2>
      {user !== null ? <TwitBox user={user} onUpdate={mutate} /> : null}
      <TimelineMessages messages={data || []} />
    </>
  );
};

export default PublicTimeline;
