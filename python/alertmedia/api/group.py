class Group(object):
    API_LIST_GROUPS = "/api/groups"
    API_CREATE_GROUP = "/api/groups"
    API_UPDATE_GROUP = "/api/groups/{}"
    API_MOVE_GROUP = "/api/v2/groups/{}/move"

    API_ADD_USER_TO_GROUP = "/api/groups/{}/users?id={}"
    API_DELETE_USER_FROM_GROUP = "/api/groups/{}/users?id={}"
    API_LIST_USERS_IN_GROUP = "/api/users?groups={}"

    def __init__(self, client):
        self.client = client
        self.net = client.net

    def get(self, group_id):
        payload = {}
        url = self.API_UPDATE_GROUP.format(group_id)
        return self.net.get(url, payload)

    def list(self, start=None, end=None, **kwargs):
        url = self.API_LIST_GROUPS
        return self.net.get(url, payload=None, start=start, end=end, query_params=kwargs)

    def create(self, customer, name, description=""):
        return self.net.post(self.API_CREATE_GROUP, {
            'name'       : name,
            'customer'   : customer,
            'description': description
        })

    def update(self, group_id, **kwargs):
        return self.net.put(self.API_UPDATE_GROUP.format(group_id), kwargs)

    def delete(self, group_id):
        return self.net.delete(self.API_UPDATE_GROUP.format(group_id))

    def move(self, group_id, parent_id):
        return self.net.post(self.API_MOVE_GROUP.format(group_id), {
            'id': parent_id
        })

    def add_users(self, group_id, users):
        users = ",".join(str(user) for user in users)
        url = self.API_ADD_USER_TO_GROUP.format(group_id, users)
        return self.net.post(url, {})

    def delete_user(self, group_id, users):
        users = ",".join(str(user) for user in users)
        url = self.API_DELETE_USER_FROM_GROUP.format(group_id, users)
        return self.net.delete(url)

    def list_users(self, group_ids, start=None, end=None):
        groups = ",".join(str(group) for group in group_ids)
        url = self.API_LIST_USERS_IN_GROUP.format(groups)
        return self.net.get(url, start, end)
