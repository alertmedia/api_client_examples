from auth import Authenticate
from api.user import User
from api.group import Group
from network import Network

class Client(object):
    auth = ""
    customer_id = 0

    def __init__(self, client_id, client_key, server="https://api.alertmedia.com"):
        self.auth = Authenticate(client_id, client_key, server)
        customer = Network(self.auth).get("/api/customers2", {}).data
        self.customer_id = customer[0]['id']

    @property
    def user(self):
        return User(auth=self.auth)

    @property
    def group(self):
        return Group(auth=self.auth)
