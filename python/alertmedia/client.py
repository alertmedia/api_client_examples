import os
from alertmedia.api import User, Group, Notification, Event, CustomerAdmin
from alertmedia.auth import Authenticate
from alertmedia.network import Network


class Client(object):
    def __init__(self, client_id=None, client_key=None, server="https://dashboard.alertmedia.com"):
        client_id = client_id or os.environ.get("AM_CLIENT_ID")
        client_key = client_key or os.environ.get("AM_CLIENT_SECRET_KEY")

        self.auth = Authenticate(client_id, client_key, server)
        self.net = Network(self.auth)
        # get customer ID for this key
        try:
            login = self.net.get("/api/v2/login")
            login.response.raise_for_status()
            self.api_customer_id = login.get('customer')['id']
            self.api_user_id = login.get('user')['id']
            self.api_admin_id = login.get('admin')['id']
        except Exception as ex:
            raise Exception("Error getting customer ID: {}".format(ex))

    @property
    def user(self):
        return User(client=self)

    @property
    def admin(self):
        return CustomerAdmin(client=self)

    @property
    def group(self):
        return Group(client=self)

    @property
    def notification(self):
        return Notification(client=self)

    @property
    def event(self):
        return Event(client=self)
