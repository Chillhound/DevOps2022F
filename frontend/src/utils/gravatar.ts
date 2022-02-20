import { MD5 } from "crypto-js";

export const gravatarUrl = (email: string, size = 80) =>
  `http://www.gravatar.com/avatar/${MD5(
    email.trim().toLowerCase()
  )}?d=identicon&s=${size}`;
