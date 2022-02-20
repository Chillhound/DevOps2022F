import React from "react";
import { useNavigate } from "react-router";
import useSWR from "swr";
import TimelineMessages from "./TimelineMessages";
import TwitBox from "./Twitbox";
import { baseUrl } from "./utils/config";
import { fetcher } from "./utils/fetcher";
import userContext from "./utils/userContext";

const PersonalTimeline: React.FC = () => {
  const { user } = React.useContext(userContext);
  const navigate = useNavigate();
  const { data, mutate } = useSWR(
    user ? [`${baseUrl}/User/Timeline?limit=100`, user.token] : null,
    fetcher
  );

  React.useEffect(() => {
    if (!user) {
      navigate("/public");
    }
  }, [navigate, user]);

  return (
    <>
      <h2>Your timeline</h2>
      {user ? <TwitBox user={user} onUpdate={mutate} /> : null}
      <TimelineMessages messages={data || []} />
    </>
  );
};

export default PersonalTimeline;
