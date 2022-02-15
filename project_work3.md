# Simulation API Requirements
- [ ] Change Authentication to Basic
- [ ] Change API names to reflect simulator needs

## /latest

Returns the latest element

```bash
return jsonify({"latest": LATEST})
```

## /register

Registers a new user

```bash
if error:
		return jsonify({"status": 400, "error_msg": error}), 400
else:
		return "", 204
```

## /msgs

returns all messages that is not flagged

```bash
return jsonify(filtered_msgs)
```

## /msgs/<username>

GET: returns all messages by a specific user that is not flagged

```bash
return jsonify(filtered_msgs)
```

POST: adds the message to the db and returns 

```bash
return "", 204
```

## /fllws/<username>

GET: 

POST: starts following or unfollowing the user

```bash
if error:
		return "", 204	
else:
		return jsonify(followers_response)
```
