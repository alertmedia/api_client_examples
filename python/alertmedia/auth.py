import base64


class Authenticate(object):
    client_id = ""
    client_key = ""
    server = ""
    authorization = ""


    def __init__(self, client_id, client_key, server="https://api.alertmedia.com"):
        self.client_id = client_id
        self.client_key = client_key
        self.server = server
        self.authorization = "Basic " + base64.b64encode(self.client_id + ":" + self.client_key)
