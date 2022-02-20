import React from "react";

interface FlashContext {
  addFlash: (flashText: string) => void;
  currentFlash: string | undefined;
}

const flashContext = React.createContext<FlashContext>({
  addFlash: () => ({}),
  currentFlash: undefined,
});

const FlashContextProxy = flashContext;

export const FlashContextProvider: React.FC = ({ children }) => {
  const [queuedFlashes, setQueuedFlashes] = React.useState<string[]>([]);
  const [activeFlash, setActiveFlash] = React.useState<string | undefined>(
    undefined
  );

  const addFlash = React.useCallback((flashText: string) => {
    setQueuedFlashes((x) => [...x, flashText]);
  }, []);

  React.useEffect(() => {
    if (queuedFlashes.length > 0 && activeFlash === undefined) {
      setActiveFlash(queuedFlashes[0]);
      setQueuedFlashes((x) => x.slice(1));
    }
  }, [activeFlash, queuedFlashes]);

  React.useEffect(() => {
    if (activeFlash) {
      setTimeout(() => {
        setActiveFlash(undefined);
      }, 5000);
    }
  }, [activeFlash]);

  return (
    <FlashContextProxy.Provider value={{ currentFlash: activeFlash, addFlash }}>
      {children}
    </FlashContextProxy.Provider>
  );
};

export default flashContext;
