import os
import json
import base64
import sqlite3
import requests
from contextlib import closing


BASE_URL = 'https://localhost:7290'
#BASE_URL = 'https://benz.requestcatcher.com/test'
DATABASE = "MiniTwitDB.db"
USERNAME = 'simulator'
PWD = 'super_safe!'
CREDENTIALS = ':'.join([USERNAME, PWD]).encode('ascii')
ENCODED_CREDENTIALS = base64.b64encode(CREDENTIALS).decode()
HEADERS = {'Connection': 'close',
           'Content-Type': 'application/json',
           f'Authorization': f'Basic {ENCODED_CREDENTIALS}'}


def init_db():
    """Creates the database tables."""
    with closing(sqlite3.connect(DATABASE)) as db:
        with open("schema.sql") as fp:
            db.cursor().executescript(fp.read())
        db.commit()


# Empty the database and initialize the schema again
os.system(f'rm {DATABASE}')
init_db()

# We've given all request verify=False to accomodate a missing certificate and not throw ssl exceptions

def test_latest():
    # post something to updaet LATEST
    url = f"{BASE_URL}/register"
    data = {'username': 'test', 'email': 'test@test', 'pwd': 'foo'}
    params = {'latest': 1337}
    response = requests.post(url, data=json.dumps(data),
                             params=params, headers=HEADERS, verify=False)
    assert response.ok

    # verify that latest was updated
    url = f'{BASE_URL}/latest'
    response = requests.get(url, headers=HEADERS, verify =False)
    assert response.ok
    assert response.json()['latest'] == 1337


def test_register():
    username = 'a'
    email = 'a@a.a'
    pwd = 'a'
    data = {'username': username, 'email': email, 'pwd': pwd}
    params = {'latest': 1}
    response = requests.post(f'{BASE_URL}/register',
                             data=json.dumps(data), headers=HEADERS, params=params, verify =False)
    assert response.ok
    # TODO: add another assertion that it is really there

    # verify that latest was updated
    response = requests.get(f'{BASE_URL}/latest', headers=HEADERS, verify =False)
    assert response.json()['latest'] == 1


def test_create_msg():
    username = 'a'
    data = {'content': 'Blub!'}
    url = f'{BASE_URL}/msgs/{username}'
    params = {'latest': 2}
    response = requests.post(url, data=json.dumps(data),
                             headers=HEADERS, params=params, verify =False)
    assert response.ok

    # verify that latest was updated
    response = requests.get(f'{BASE_URL}/latest', headers=HEADERS, verify =False)
    assert response.json()['latest'] == 2


def test_get_latest_user_msgs():
    username = 'a'

    query = {'no': 20, 'latest': 3}
    url = f'{BASE_URL}/msgs/{username}'
    response = requests.get(url, headers=HEADERS, params=query, verify =False)
    assert response.status_code == 200

    got_it_earlier = False
    for msg in response.json():
        if msg['content'] == 'Blub!' and msg['user'] == username:
            got_it_earlier = True

    assert got_it_earlier

    # verify that latest was updated
    response = requests.get(f'{BASE_URL}/latest', headers=HEADERS, verify =False)
    assert response.json()['latest'] == 3


def test_get_latest_msgs():
    username = 'a'
    query = {'no': 20, 'latest': 4}
    url = f'{BASE_URL}/msgs'
    response = requests.get(url, headers=HEADERS, params=query, verify =False)
    assert response.status_code == 200

    got_it_earlier = False
    for msg in response.json():
        if msg['content'] == 'Blub!' and msg['user'] == username:
            got_it_earlier = True

    assert got_it_earlier

    # verify that latest was updated
    response = requests.get(f'{BASE_URL}/latest', headers=HEADERS, verify =False)
    assert response.json()['latest'] == 4


def test_register_b():
    username = 'b'
    email = 'b@b.b'
    pwd = 'b'
    data = {'username': username, 'email': email, 'pwd': pwd}
    params = {'latest': 5}
    response = requests.post(f'{BASE_URL}/register', data=json.dumps(data),
                             headers=HEADERS, params=params, verify =False)
    assert response.ok
    # TODO: add another assertion that it is really there

    # verify that latest was updated
    response = requests.get(f'{BASE_URL}/latest', headers=HEADERS, verify =False)
    assert response.json()['latest'] == 5


def test_register_c():
    username = 'c'
    email = 'c@c.c'
    pwd = 'c'
    data = {'username': username, 'email': email, 'pwd': pwd}
    params = {'latest': 6}
    response = requests.post(f'{BASE_URL}/register', data=json.dumps(data),
                             headers=HEADERS, params=params, verify =False)
    assert response.ok

    # verify that latest was updated
    response = requests.get(f'{BASE_URL}/latest', headers=HEADERS, verify =False)
    assert response.json()['latest'] == 6


def test_follow_user():
    username = 'a'
    url = f'{BASE_URL}/fllws/{username}'
    data = {'follow': 'b'}
    params = {'latest': 7}
    response = requests.post(url, data=json.dumps(data),
                             headers=HEADERS, params=params, verify =False)
    assert response.ok

    data = {'follow': 'c'}
    params = {'latest': 8}
    response = requests.post(url, data=json.dumps(data),
                             headers=HEADERS, params=params, verify =False)
    assert response.ok

    query = {'no': 20, 'latest': 9}
    response = requests.get(url, headers=HEADERS, params=query, verify =False)
    assert response.ok

    json_data = response.json()
    assert "b" in json_data["follows"]
    assert "c" in json_data["follows"]

    # verify that latest was updated
    response = requests.get(f'{BASE_URL}/latest', headers=HEADERS, verify =False)
    assert response.json()['latest'] == 9


def test_a_unfollows_b():
    username = 'a'
    url = f'{BASE_URL}/fllws/{username}'

    #  first send unfollow command
    data = {'unfollow': 'b'}
    params = {'latest': 10}
    response = requests.post(url, data=json.dumps(data),
                             headers=HEADERS, params=params, verify =False)
    assert response.ok

    # then verify that b is no longer in follows list
    query = {'no': 20, 'latest': 11}
    response = requests.get(url, params=query, headers=HEADERS, verify =False)
    assert response.ok
    assert 'b' not in response.json()['follows']

    # verify that latest was updated
    response = requests.get(f'{BASE_URL}/latest', headers=HEADERS, verify =False)
    assert response.json()['latest'] == 11