import React from "react";

export interface User {
  userName: string;
  userId: string;
  token: string;
}

interface UserContext {
  user: User | null;
  setUser: (user: User | null) => void;
}

const userContext = React.createContext<UserContext>({
  user: null,
  setUser: () => ({}),
});

const UserContextProxy = userContext;

export const UserContextProvider: React.FC = ({ children }) => {
  const [user, setUser] = React.useState<User | null>(null);

  return (
    <UserContextProxy.Provider value={{ user, setUser }}>
      {children}
    </UserContextProxy.Provider>
  );
};

export default userContext;
