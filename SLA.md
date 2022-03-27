# Service Level Agreement

Our Monthly Uptime Percentage (hereafter abbreviated as MUP) describes the percentage of time where the server responds to requests.

| Covered Service | MUP |
| ----------- | ----------- |
| Group F Minitwit | 98% |

If Group F does not adhere to the SLA's target for MUP or any other qualifying metric, the client can send an inquiry formulated as a GitHub issue with relevant title, description and labels.

## Definitions
**Back-off requirement**: For each failed request, wait 30 seconds before sending the next one.

**Inquiry response time**: The Inquiry response time is the amount of time in hours that a client can expect to wait. The time period is determined by the weekday of the request and follow this scheme:
| Day | Time period |
| ----------- | ----------- |
| Mon-Thu* | 24 hours |
| Fri-Sun* | 72 hours |

\* Excluding Danish holidays

## SLA Exclusions
The SLA does not cover: Client hardware or software (or both) errors in relation to our product; errors caused by factors outside of Group F’s reasonable control; Errors caused by the client not adhering to the documentation, e.g. invalid request, unauthorized or unrecognized users, or inaccessible data
