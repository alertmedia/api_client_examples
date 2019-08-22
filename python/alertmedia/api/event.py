class Event(object):
    API_LIST_EVENTS = "/api/events"
    API_CREATE_EVENT = "/api/events"
    API_UPDATE_EVENT = "/api/events/{}"

    API_LIST_EVENT_TYPES = "/api/event_types?hidden=false&customer={}"

    def __init__(self, client):
        self.client = client
        self.net = client.net

    def get(self, notification_id):
        payload = {}
        url = self.API_UPDATE_EVENT.format(notification_id)
        return self.net.get(url, payload)

    def list(self, start=None, end=None, user_id=None):
        """
        List of Events for the customer (requires Admin user)
        If user_id is passed in, lists Events only send to that User
        """
        url = self.API_LIST_EVENTS
        if user_id:
            url += "?user={}".format(user_id)
        return self.net.get(url, payload=None, start=start, end=end)

    def create(self, params=None):
        return self.net.post(self.API_CREATE_EVENT, params or dict())

    def update(self, event_id, params=None):
        url = self.API_UPDATE_EVENT.format(event_id)
        return self.net.put(url, params or dict())

    def list_types(self, customer_id, default_only=False, start=None, end=None):
        """
        List of Events for the customer (requires customer id)
        """
        url = self.API_LIST_EVENT_TYPES.format(customer_id)
        if default_only:
            url += "&default=true"

        return self.net.get(url, payload=None, start=start, end=end)
