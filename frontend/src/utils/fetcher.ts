export const fetcher = (url: string, token: string) => {
  return fetch(url, {
    headers: {
      Authorization: token,
    },
  }).then((res) => res.json());
};
