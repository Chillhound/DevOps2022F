import React from "react";
import TimelineMessages from "./TimelineMessages";
import { baseUrl } from "./utils/config";

const PublicTimeline: React.FC = () => {
  const [timelineItems, setTimelineItems] = React.useState<any[]>([]);

  React.useEffect(() => {
    fetch(`${baseUrl}/Message/PublicTimeline?limit=100`)
      .then((res) => res.json())
      .then((data) => setTimelineItems(data));
  }, []);

  return (
    <>
      <h2>Public Timeline</h2>
      <TimelineMessages messages={timelineItems} />
    </>
  );
};

export default PublicTimeline;
