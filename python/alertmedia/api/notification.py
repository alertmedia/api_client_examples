class Notification(object):
    API_LIST_NOTIFICATIONS = "/api/v2/notifications?customer={}"
    API_CREATE_NOTIFICATION = "/api/v2/notifications"
    API_UPDATE_NOTIFICATION = "/api/v2/notifications/{}"

    API_LIST_NOTIFICATION_PLANS = "/api/v2/notification_plans?ordering=title&customer={}"
    API_GET_NOTIFICATION_PLAN = "/api/v2/notification_plans/{}"

    def __init__(self, client):
        self.client = client
        self.net = client.net

    def get(self, notification_id):
        return self.net.get(self.API_UPDATE_NOTIFICATION.format(notification_id))

    def list(self, customer_id, start=None, end=None, user_id=None):
        """
        List of Notifications for the customer (requires Admin user)
        If user_id is passed in, lists Notifications only send to that User
        """
        url = self.API_LIST_NOTIFICATIONS.format(customer_id)
        if user_id:
            url += "?user={}".format(user_id)

        return self.net.get(url, start=start, end=end)

    def create(self, params={}):
        return self.net.post(self.API_CREATE_NOTIFICATION, params)

    def update(self, notification_id, params={}):
        return self.net.put(self.API_UPDATE_NOTIFICATION.format(notification_id), params)

    def get_plan(self, notification_plan_id):
        return self.net.get(self.API_GET_NOTIFICATION_PLAN.format(notification_plan_id))

    def list_plans(self, customer_id, start=None, end=None):
        return self.net.get(
            self.API_LIST_NOTIFICATION_PLANS.format(customer_id),
            start=start,
            end=end
        )
