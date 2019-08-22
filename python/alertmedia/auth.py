import base64


class Authenticate(object):
    client_id = ""
    client_key = ""
    server = ""
    authorization = ""

    def __init__(self, client_id, client_key, server):
        self.client_id = client_id
        self.client_key = client_key
        self.server = server
        self.authorization = "Basic " + base64.b64encode(bytes(self.client_id + ":" + self.client_key, 'utf-8')).decode()
