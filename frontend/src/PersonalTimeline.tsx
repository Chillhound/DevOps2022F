import React from "react";
import { useNavigate } from "react-router";
import TimelineMessages from "./TimelineMessages";
import TwitBox from "./Twitbox";
import { baseUrl } from "./utils/config";
import userContext from "./utils/userContext";

const PersonalTimeline: React.FC = () => {
  const { user } = React.useContext(userContext);
  const [timelineMessages, setTimelineMessages] = React.useState<any[]>([]);
  const navigate = useNavigate();

  React.useEffect(() => {
    fetch(`${baseUrl}/User/Timeline?limit=100`)
      .then((res) => res.json())
      .then((data) => setTimelineMessages(data));
  }, []);

  React.useEffect(() => {
    if (!user) {
      navigate("/public");
    }
  }, [navigate, user]);

  return (
    <>
      <h2>Your timeline</h2>
      {user ? <TwitBox user={user} /> : null}
      <TimelineMessages messages={timelineMessages} />
    </>
  );
};

export default PersonalTimeline;
