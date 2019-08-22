class CustomerAdmin(object):
    API_ADMIN_LIST = "/api/customer_admins"
    API_ADMIN = "/api/customer_admins/{}"

    def __init__(self, client):
        self.client = client
        self.net = client.net

    def list(self):
        return self.net.list(self.API_ADMIN_LIST)

    def get(self, admin_id):
        return self.net.get(self.API_ADMIN.format(admin_id))

    def put(self, admin_id, **kwargs):
        if self.client.api_admin_id == admin_id:
            raise Exception("Cannot modify admin status for API key owner")
        return self.net.put(self.API_ADMIN.format(admin_id), **kwargs)

    def set_notify_only(self, admin_id, allowed_groups):
        if not isinstance(allowed_groups, list):
            allowed_groups = list(allowed_groups)
        payload = self.get(admin_id).data
        payload.update(type="messenger", allowed_groups=allowed_groups)
        return self.put(admin_id, payload=payload)

    def set_full_admin(self, admin_id):
        payload = self.get(admin_id).data
        payload.update(type="admin")
        return self.put(admin_id, payload=payload)
