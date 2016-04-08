from alertmedia.network import Network


class Group(object):
    auth = ""
    net = ""
    API_LIST_GROUPS = "/api/groups"
    API_CREATE_GROUP = "/api/groups"
    API_UPDATE_GROUP = "/api/groups/{}"
    API_ADD_USER_TO_GROUP = "/api/groups/{}/users?id={}"
    API_DELETE_USER_FROM_GROUP = "/api/groups/{}/users?id={}"
    API_LIST_USERS_IN_GROUP = "/api/users?groups={}"

    def __init__(self, auth):
        self.auth = auth
        self.net = Network(self.auth)

    def get(self, group_id):
        payload = {}
        url = self.API_UPDATE_GROUP.format(group_id)
        return self.net.get(url, payload)

    def list(self, start=None, end=None):
        url = self.API_LIST_GROUPS
        return self.net.get(url, payload=None, start=start, end=end)

    def create(self, customer, name, description=""):
        payload = {}
        payload['name'] = name
        payload['customer'] = customer
        payload['description'] = description
        return self.net.post(self.API_CREATE_GROUP, payload)

    def update(self, group_id, name=None, description=None):
        payload = {}
        if name is not None:
            payload['name'] = name
        if description is not None:
            payload['description'] = description
        url = self.API_UPDATE_GROUP.format(group_id)
        return self.net.put(url, payload)

    def delete(self, group_id):
        url = self.API_UPDATE_GROUP.format(group_id)
        return self.net.delete(url)

    def add_users(self, group_id, users=[]):
        users = ",".join(str(user) for user in users)
        url = self.API_ADD_USER_TO_GROUP.format(group_id, users)
        return self.net.post(url, {})

    def delete_user(self, group_id, users=[]):
        users = ",".join(str(user) for user in users)
        url = self.API_DELETE_USER_FROM_GROUP.format(group_id, users)
        return self.net.delete(url)

    def list_users(self, group_ids, start=None, end=None):
        groups = ",".join(str(group) for group in group_ids)
        url = self.API_LIST_USERS_IN_GROUP.format(groups)
        return self.net.get(url, start, end)